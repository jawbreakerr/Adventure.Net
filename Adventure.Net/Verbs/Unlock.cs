using System;
using System.Collections.Generic;
using System.Linq;

namespace Adventure.Net.Verbs
{
    public class Unlock : Verb
    {
        public Unlock()
        {
            Name = "unlock";
            Grammars.Add("<noun> with <held>", UnlockObject);
            //Grammars.Add("<noun>", UnlockObject);
        }

        private bool UnlockObject()
        {
            if (!Object.IsLockable)
                Print("That doesn't seem to be something you can unlock.");
            else if (!Object.IsLocked)
                Print("It's unlocked at the moment.");
            else if (Object is Door) // need to refactor so this works for all lockable things
            {
                Door door = Object as Door;

                UnlockDoor(door);

            }

            return true;
        }

        private void UnlockDoor(Door door)
        {
            if (!Inventory.Contains(door.Key))
            {
                Print("You have nothing to unlock that with.");
                return;
            }

            if (IndirectObject == null)
            {
                if (Inventory.Items.Count == 1 && Inventory.Items[0] == door.Key)
                {
                    Print("(with the {0})", door.Key.Name);
                    Print("You unlock the {0}.", Object.Name);
                    Object.IsLocked = false;
                    return;
                }

                ObjectNotSpecified();
            }
            else if (IndirectObject == door.Key)
            {
                Print("You unlock the {0}.", Object.Name);
                Object.IsLocked = false;
            }

        }

        // this method basically shows a serious design flaw. because the parser does not
        // currently remember the last command
        private void ObjectNotSpecified()
        {
            //Print("What do you want to unlock the {0} with?", Object.Name);
                //string input = Context.CommandPrompt.GetInput();
                //if (string.IsNullOrEmpty(input))
                //{
                //    Print(L.DoNotUnderstand);
                //    return;
                //}

            //InputTokenizer tokenizer = new InputTokenizer();
            //IList<string> tokens = tokenizer.Tokenize(input);

            //// player is answering the question (no verb specified) vs. re-typing
            //// a complete sentence
            //if (!tokens.StartsWithVerb())
            //{
            //    // this is very simplistic right now
            //    input = "unlock " + Object.Synonyms[0] + " with " + String.Join(" ", tokens.ToArray());
            //}

            //// player re-typed input with verb
            //var parser = new Parser3();
            //parser.Parse(input);
           
        }
    }
}
