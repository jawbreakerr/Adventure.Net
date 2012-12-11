using System;
using System.Collections.Generic;
using System.Linq;
using Adventure.Net.Verbs;

namespace Adventure.Net
{
    public class Parser : IParser
    {
        private List<string> parserResults;
        private State currentState;

        private enum State
        {
            Before,
            During,
            After
        }
        
        private List<string> beforeMessages;
        private List<string> duringMessages;
        private List<string> afterMessages;

        private InputResult inputResult;
        private Object objectInPlay;

        public void Print(string msg)
        {
            IList<string> messages = GetMessageList();

            if (inputResult.IsAll || inputResult.Objects.Count > 1)
            {
                if (inputResult.Objects.Count > 1)
                    messages.Add(objectInPlay.Name + ": " + msg);
                else if (inputResult.Objects.Count == 1)
                {
                    messages.Add("(the " + objectInPlay.Name + ")");
                    messages.Add(msg);
                }
            }
            else
            {
                messages.Add(msg);
            }
        }

        public void Print(string format, params object[] arg)
        {
            string msg = String.Format(format, arg);
            Print(msg);
        }

        private IList<string> GetMessageList()
        {
            if (currentState == State.Before)
                return beforeMessages;
            if (currentState == State.After)
                return afterMessages;
            return duringMessages;
        }

        public IList<string> Parse(string input)
        {
            return Parse(input, true);
        }

        public IList<String> Parse(string input, bool showOutput)
        {
            Context.Parser = this;
            Context.Object = null;
            Context.IndirectObject = null;
            
            parserResults = new List<string>();

            Library L = new Library();
            bool wasLit = L.IsLit();

            var userInput = new UserInput();

            if (inputResult != null && inputResult.IsAskingQuestion)
            {
                var tokenizer = new InputTokenizer();
                var tokens = tokenizer.Tokenize(input);

                if (!tokens.StartsWithVerb())
                {
                    string temp = null;
                    if (inputResult.Verb.IsNull == false)
                        temp += inputResult.Verb.Name + " ";
                    if (inputResult.Objects.Count > 0)
                        temp += inputResult.Objects[0].Synonyms[0] + " ";
                    if (!string.IsNullOrEmpty(inputResult.Preposition))
                        temp += inputResult.Preposition + " ";
                    if (inputResult.IndirectObject != null)
                        temp += inputResult.IndirectObject.Name + " ";
                    if (temp != null)
                        input = temp + input;
                }

            }

            inputResult = userInput.Parse(input);

            //if (inputResult.HasError)
            //{
            //    parserResults.Add(inputResult.Error);
            //}
            //else
            //{
            //    HandleInputResult();
            //}
            HandleInputResult();

            if (!wasLit && L.IsLit())
                L.Look(true);

            return GetResults(showOutput);
        }

        private IList<string> GetResults(bool showOutput)
        {
            var results = new List<string>();

            foreach (string value in parserResults.Where(x => !String.IsNullOrEmpty(x)).Distinct())
            {
                string[] lines = value.Split('\n');

                foreach (string line in lines)
                {
                    results.Add(line);
                    if (showOutput)
                    {
                        Context.Output.Print(line);
                    }
                }

            }

            return results;
        }

        private void HandleInputResult()
        {
            var builder = new CommandBuilder(inputResult);
            var commands = builder.Build();

            foreach(var command in commands)
            {
                ExecuteCommand(command);
            }
            
        }

        public bool ExecuteCommand(Command command)
        {
            Context.Object = command.Object;
            Context.IndirectObject = command.IndirectObject;

            beforeMessages = new List<string>();
            duringMessages = new List<string>();
            afterMessages = new List<string>();

            // after, before, and during needs to be modified to return the object
            // that handled the message

            bool result = Before(command);

            if (!result)
            {
                result = During(command);
                if (result)
                {
                    if (After(command))
                    {
                        parserResults.AddRange(afterMessages);
                    }
                    else
                    {
                        parserResults.AddRange(afterMessages);
                        parserResults.AddRange(duringMessages);
                    }
                }
                else
                {
                    parserResults.AddRange(duringMessages);
                }
            }
            else
            {
                parserResults.AddRange(beforeMessages);
            }

            return result;
        }

        private bool Before(Command command)
        {
            currentState = State.Before;
            return Before(command, command.IndirectObject) || (Before(command, command.Object) || Before(command, Context.Story.Location));
        }

        private bool Before(Command command, Object obj)
        {
            if (obj != null)
            {
                objectInPlay = obj;
                Func<bool> before = obj.Before(command.Verb.GetType());
                if (before != null)
                {
                    return before();
                }
            }

            return false;
        }

        private bool During(Command command)
        {
            currentState = State.During;
            objectInPlay = command.Object;
            return command.Action();
        }

        private bool After(Command command)
        {
            currentState = State.After;
            return After(command, command.IndirectObject) || (After(command, command.Object) || After(command, Context.Story.Location));
        }

        private bool After(Command command, Object obj)
        {
            if (obj != null)
            {
                objectInPlay = obj;
                Func<bool> after = obj.After(command.Verb.GetType());
                if (after != null)
                {
                    return after();
                }
            }

            return false;
        }


    }

}



