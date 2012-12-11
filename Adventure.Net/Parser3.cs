using System;
using System.Collections.Generic;
using System.Linq;

namespace Adventure.Net
{
    public class Parser3 : IParser
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
        private bool isAll;

        public void Print(string msg)
        {
            if (isAll)
            {
                msg = Context.Object.Name + ": " + msg;
            }   
 
            switch(currentState)
            {
                case State.Before:
                    beforeMessages.Add(msg);
                    break;
                case State.During:
                    duringMessages.Add(msg);
                    break;
                case State.After:
                    afterMessages.Add(msg);
                    break;
            }
        }

        public void Print(string format, params object[] arg)
        {
            string msg = String.Format(format, arg);
            Print(msg);
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
            var inputResult = userInput.Parse(input);
            isAll = inputResult.IsAll;

            if (inputResult.HasError)
            {
                parserResults.Add(inputResult.Error);
            }
            else
            {
                HandleInputResult(inputResult);
            }

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

        private void HandleInputResult(InputResult inputResult)
        {
            CommandBuilder builder = new CommandBuilder(inputResult);
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

        private static bool Before(Command command, Object obj)
        {
            if (obj != null)
            {
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
            return command.Action();
        }

        private bool After(Command command)
        {
            currentState = State.After;
            return After(command, command.IndirectObject) || (After(command, command.Object) || After(command, Context.Story.Location));
        }

        private static bool After(Command command, Object obj)
        {
            if (obj != null)
            {
                Func<bool> after = obj.After(command.Verb.GetType());
                if (after != null)
                {
                    return after();
                }
            }

            return false;
        }

        //private void ExecuteCommand(Command command)
        //{
        //    bool result = command.Execute();
        //    if (result == false)
        //        return;

        //    //if (result.IsAction())
        //    //    ExecuteAction(command.Hint, result);
        //    //else
        //    //    parserResults.Add(result);
        //}

        //private void ExecuteAction(string prefix, string action)
        //{
        //    action = action.Replace("<<", "").Replace(">>", "");
        //    Parser3 parser = new Parser3();
        //    IList<string> results = parser.Parse(action, false);
        //    parserResults.Add(prefix + results[0]);
        //}

    }

}



