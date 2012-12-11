using System;
using System.Collections.Generic;
using System.Linq;
using Adventure.Net.Verbs;

namespace Adventure.Net
{
    public class UserInput
    {
        private Library L = new Library();

        private InputResult result;
        private Grammar grammar;

        public InputResult Parse(string input)
        {
            grammar = null;
            result = new InputResult();

            var tokenizer = new InputTokenizer();
            var tokens = tokenizer.Tokenize(input);

            if (tokens.Count == 0)
            {
                result.Action = () =>
                {
                    Context.Parser.Print(L.DoNotUnderstand);
                    return true;
                };
                return result;
            }

            // there can be more than one match for verbs like "switch"
            // which has one class that handles "switch on" and another 
            // class that handles "switch off"
            IList<Verb> possibleVerbs = VerbList.GetVerbsByName(tokens[0]);

            if (possibleVerbs.Count == 0)
            {
                result.Verb = new NullVerb();
                result.Action = ErrorAction(L.VerbNotRecognized);
                return result;
            }

            // remove verb token
            tokens.RemoveAt(0);

            var grammarTokens = new List<string>();
            bool hasPreposition = false;
            bool isException = false;
            bool isPartial = false;

            foreach (string token in tokens)
            {
                bool hasObject = result.Objects.Count > 0;

                var objects = Objects.WithName(token);
                
                if (!hasObject)
                {
                    var rooms = Rooms.WithName(token);
                    foreach (var room in rooms)
                    {
                        objects.Add(room);
                    }
                }
                
                if (objects.Count == 0)
                {
                    bool isDirection = possibleVerbs.Count == 1 && 
                                       Compass.Directions.Contains(token) &&
                                       result.Objects.Count == 0;
                    bool isPreposition = Prepositions.Contains(token);
                
                    if (isDirection)
                    {
                        possibleVerbs.Clear();
                        possibleVerbs.Add(VerbList.GetVerbByName(token));
                    }
                    else if (isPreposition)
                    {
                        hasPreposition = true;
                        grammarTokens.Add(token);
                        result.Preposition = token;
                    }
                    else if (token == K.ALL)
                    {
                        ((List<Object>)result.Objects).AddRange(L.ObjectsInScope());
                        grammarTokens.Add(token);
                        result.IsAll = true;
                    }
                    else if (token == K.EXCEPT)
                    {
                        isException = true;
                    }
                    else
                    {
                        if (isPartial)
                        {
                            string partial = String.Format("I only understood you as far as wanting to {0} the {1}.", possibleVerbs[0].Name, result.Objects[0].Name);
                            result.Action = ErrorAction(partial);
                            return result;
                        }
                        else
                        {
                            result.Action = ErrorAction(L.CantSeeObject);
                            return result;
                        }
                    }
                }
                else
                {
                    // need to implement "Which do you mean, the red cape or the black cape?" type behavior
                    // here
                    Object obj = null;
                    var ofInterest = objects.Where(x => x.InScope);
                    if (ofInterest.Count() > 1)
                    {
                        obj = ofInterest.Where(x => x.InInventory).FirstOrDefault();
                    }
                    else
                    {
                        obj = ofInterest.FirstOrDefault();
                    }
                    //-------------------------------------------------------------------------------------
                    
                    bool isIndirectObject = hasPreposition && hasObject;
                    
                    if (obj == null)
                    {
                        result.Action = ErrorAction(L.CantSeeObject);
                        return result;
                    }
                    else if (isIndirectObject)
                    {
                        grammarTokens.Add(K.INDIRECT_OBJECT_TOKEN);
                        result.IndirectObject = obj;
                    }
                    else if (isException)
                    {
                        result.Objects.Remove(obj);
                        result.Exceptions.Add(obj);
                    }
                    else
                    {
                        if (!grammarTokens.Contains(K.OBJECT_TOKEN))
                            grammarTokens.Add(K.OBJECT_TOKEN);
                        if (!result.Objects.Contains(obj))
                            result.Objects.Add(obj);
                        isPartial = true;
                    }    
                }

               
            }

            result.Pregrammar = string.Join(" ", grammarTokens.ToArray());

            var grammarBuilder = new GrammarBuilder(grammarTokens);
            var grammars = grammarBuilder.Build();

            FindVerb(possibleVerbs, grammars);

            if (grammar == null)
            {
                // Create action for "What do you want to <<verb>> the <<noun>> with???"
                if (isPartial && possibleVerbs.Count > 0)
                {
                    result.Verb = possibleVerbs[0];
                    grammar = result.Verb.Grammars[0];
                    
                    if (string.IsNullOrEmpty(result.Preposition))
                    {
                        string[] ts = grammar.Format.Split(' ');
                        foreach (string t in ts)
                        {
                            if (t.IsPreposition())
                            {
                                result.Preposition = t;
                                break;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(result.Preposition))
                    {
                        //string msg = String.Format("What do you want to {0} the {1} {2}?", result.Verb.Name, result.Objects[0].Name, result.Preposition);
                        //result.Error = msg;
                        //result.IsAskingQuestion = true;
                        //return result;
                    }
                }

                result.Action = ErrorAction(L.DoNotUnderstand);
                
                return result;
            }
            
            if (result.IsAll && result.ObjectsMustBeHeld)
            {
                result.Objects = result.Objects.Where(x => x.InInventory).ToList();   
            }

            return result;
        }

        private void FindVerb(IEnumerable<Verb> possibleVerbs, IEnumerable<string> grammars)
        {
            foreach (var verb in possibleVerbs)
            {
                if (FindGrammar(verb, grammars))
                {
                    result.Verb = verb;
                    return;
                }
            }
        }

        private bool FindGrammar(Verb verb, IEnumerable<string> grammars)
        {
            foreach (var possibleGrammar in grammars)
            {
                Grammar matchedGrammar = verb.Grammars.Where(x => x.Format == possibleGrammar).FirstOrDefault();

                if (matchedGrammar != null)
                {
                    grammar = matchedGrammar;
                    result.ObjectsMustBeHeld = ObjectsMustBeHeld(grammar);
                    result.Action = matchedGrammar.Action;
                    return true;
                }
            }

            return false;
        }

        private static bool ObjectsMustBeHeld(Grammar grammar)
        {
            if (grammar == null || string.IsNullOrEmpty(grammar.Format))
                return false;
            var tags = grammar.Format.Tags();
            return tags.Count > 0 && (tags[0] == K.HELD_TOKEN || tags[0] == K.MULTIHELD_TOKEN);
        }

        private static Func<bool> ErrorAction(string error)
        {
            return () =>
                {
                    Context.Parser.Print(error);
                    return true;
                };
        }
    }
}
