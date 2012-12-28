namespace Adventure.Net.Verbs
{
    public class Ask : Verb
    {
        public Ask()
        {
            Name = "ask";
            //Grammars.Add("<creature> 'about' <topic>", AttackObject);
            Grammars.Add("<noun> 'about' <topic>", OnAsk);
            Grammars.Add("<noun> 'for' <noun>", OnAskFor);

        }

        private bool OnAsk()
        {
            if (!Object.IsAnimate)
            {
                Print("You can only do that to something animate.");
                return true;
            }

            return false;
        }

        private bool OnAskFor()
        {
            if (!Object.IsAnimate)
            {
                Print("You can only do that to something animate.");
                return true;
            }

            return false;
        }

    }
}