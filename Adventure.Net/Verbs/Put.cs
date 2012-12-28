using System;

namespace Adventure.Net.Verbs
{

    public class Put : Verb
    {
        public Put()
        {
            Name = "put";
            Grammars.Add("<multi> [in,inside,into] <noun>", InsertObject);
            Grammars.Add("<multiheld> [on,onto] <noun>", PutOnObject);
            Grammars.Add("on <held>", WearObject);
            Grammars.Add("down <multiheld>", DropObject);
            Grammars.Add("<multiheld> down", DropObject);
        }

        private bool InsertObject()
        {
            return RedirectTo<Insert>("<multi> in <noun>");
        }
        
        private bool PutOnObject()
        {
            throw new Exception("This is not implemented!!!!!");
        }

        private bool WearObject()
        {
            throw new Exception("This is not implemented!!!!!");
        }

        private bool DropObject()
        {
            return RedirectTo<Drop>("<multiheld>");
        }

        
    }
}
