using System;
using System.Linq;
using Adventure.Net.Verbs;

namespace Adventure.Net
{
    public abstract class Verb : ContextObject
    {
        public string Name { get; protected set; }
        public Synonyms Synonyms = new Synonyms();
        
        public Grammars Grammars = new Grammars();
        
        protected bool RedirectTo<T>(string format) where T : Verb, new()
        {
            bool result = false;
            var verb = new T();
            var g = verb.Grammars.Where(x => x.Format == format).SingleOrDefault();

            if (g != null)
            {
                var command =
                    new Command
                        {
                            Object = Context.Object,
                            IndirectObject = Context.IndirectObject,
                            Verb = verb,
                            Action = g.Action
                        };

                result = Context.Parser.ExecuteCommand(command);
            }

            return result;
        }

        public bool IsNull
        {
            get { return this.GetType() == typeof (NullVerb); }
        }
    }
}