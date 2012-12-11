namespace Adventure.Net.Verbs
{
    public class Enter : Verb
    {
        public Enter()
        {
            Name = "enter";
            Grammars.Add("<noun>", EnterObject);
        }

        public bool EnterObject()
        {
            Print("That's not something you can enter");
            return true;
        }
    }
}
