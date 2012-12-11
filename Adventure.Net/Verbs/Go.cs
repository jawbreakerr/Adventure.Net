namespace Adventure.Net.Verbs
{
    public class Go : Verb
    {
        public Go()
        {
            Name = "go";
            Synonyms.Are("g", "walk", "run");
            Grammars.Add(Grammar.Empty,VagueGo);
            Grammars.Add("<direction>", ()=> false);
            Grammars.Add("<noun>", EnterIt);
        }

        private bool VagueGo()
        {
            Print("You'll have to say which compass direction to go in.");
            return true;
        }

        private bool EnterIt()
        {
            return RedirectTo<Enter>("<noun>");
        }
        

    }
}
