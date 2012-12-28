using System;
using System.Collections.Generic;
using System.Linq;
using Adventure.Net.Verbs;

namespace Adventure.Net
{
    public class InputResult
    {
        public IList<Object> Objects { get; set; }
        public Object IndirectObject { get; set; }
        public IList<Object> Exceptions { get; private set; }
        public string Pregrammar { get; set; }
        public Func<bool> Action { get; set; }
        public bool IsAll { get; set; }
        public string Preposition { get; set; }
        public bool IsAskingQuestion { get; set; }
        public bool ObjectsMustBeHeld { get; set; }
        
        private Verb _verb;

        public InputResult()
        {
            Objects = new List<Object>();
            Exceptions = new List<Object>();
            Verb = new NullVerb();
        }

        public Verb Verb
        {
            get { return _verb; }
            set { _verb = value ?? new NullVerb(); }
        }

        public bool IsSingleAction
        {
            get
            {
                return ((Verb.IsNull && Action != null) ||
                (Verb is DirectionalVerb) ||
                (GetGrammar("") != null));
            }
        }

        private Grammar GetGrammar(string format)
        {
            return Verb.Grammars.SingleOrDefault(x => x.Format == format);
        }
    }

}
