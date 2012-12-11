using System;
using System.Collections.Generic;
using Adventure.Net.Verbs;

namespace Adventure.Net
{
    public class CommandBuilder
    {
        private readonly InputResult inputResult;

        public CommandBuilder(InputResult inputResult)
        {
            this.inputResult = inputResult;
        }

        public IList<Command> Build()
        {
            var result = new List<Command>();

            if (inputResult.Objects.Count == 0)
            {
                if (inputResult.IsSingleAction)
                {
                    result.Add(GetCommand(null));
                }
                else
                {
                    throw new Exception("Implement objectnotspecifed!!!!");
                }
            }
            else if (inputResult.Objects.Count == 1)
            {
                var obj = inputResult.Objects[0];

                if (inputResult.ObjectsMustBeHeld && !Inventory.Contains(obj))
                {
                    bool canTakeObject = obj.AtLocation && obj.InScope && !obj.IsScenery && !obj.IsStatic && !obj.IsAnimate;
                    if (canTakeObject)
                    {
                        var takeCommand = GenerateTakeFirstCommand(obj);
                        result.Add(takeCommand);
                    }
                    else
                    {
                        var command = GetCommand(obj);
                        command.Action = () =>
                        {
                            Context.Parser.Print("You aren't holding that.");
                            return true;
                        };

                        result.Add(command);
                    }
                }
                else
                {
                    result.Add(GetCommand(obj));
                }
            }
            else
            {
                foreach (var obj in inputResult.Objects)
                {
                    result.Add(GetCommand(obj));
                }
            }

            return result;
        }

        private Command GetCommand(Object obj)
        {
            return new Command()
            {
                Verb = inputResult.Verb,
                Action = inputResult.Action,
                Object = obj,
                IndirectObject = inputResult.IndirectObject
            };
        }

        private static Command GenerateTakeFirstCommand(Object obj)
        {
            var result = new Command
            {
                Verb = new Take(),
                Object = obj,
                Action = () =>
                {
                    var take = new Take {SupressMessages = true};
                    take.TakeObject(obj);
                    Context.Parser.Print(String.Format("(first taking the {0})", obj.Name));
                    return false;
                }
            };

            return result;
        }
    }
   
}
