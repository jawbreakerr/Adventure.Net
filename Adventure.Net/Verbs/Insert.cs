using System;

namespace Adventure.Net.Verbs
{

    public class Insert : Verb
    {
        public Insert()
        {
            Name = "insert";
            Grammars.Add("<multiheld> [in,into] <noun>", InsertObject);
        }

        private bool InsertObject()
        {

            if (!Inventory.Contains(Object))
            {
                Print(String.Format("You need to be holding the {0} before you can put it into something else.", Object.Name));
                return true;
            }

            Func<bool> beforeReceive = IndirectObject.Before<Receive>();
            if (beforeReceive != null)
            {
                return beforeReceive();
            }

            Container c = IndirectObject as Container;
            if (c == null)
            {
                Print("That can't contain things.");
                return true;
            }

            if (!c.IsOpen)
            {
                Print(String.Format("The {0} is closed.", c.Name));
                return true;
            }

            Inventory.Remove(Object);
            c.Add(Object);

            Func<bool> afterReceive = IndirectObject.After<Receive>();
            if (afterReceive != null)
            {
                return afterReceive();
            }

            Print(String.Format("You put the {0} into the {1}.", Object.Name, IndirectObject.Name));
            return true;
        }
    }
}
