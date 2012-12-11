using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adventure.Net.Utility;

namespace Adventure.Net
{
    public static class L //: ContextObject
    {
        static L()
        {
            CantSeeObject = "You can't see any such thing.";
            DoNotUnderstand = "I beg your pardon?";
            VerbNotRecognized = "That's not a verb I recognize.";
        }

        public static void Banner()
        {
            Context.Output.Bold(Context.Story.Story);
            Context.Output.Print(Context.Story.Headline);
            Context.Output.PrintLine();
        }

        public static string DoNotUnderstand { get; set; }
        public static string CantSeeObject { get; set; }
        public static string VerbNotRecognized { get; set; }

        public static void Look(bool showFull)
        {
            Context.Output.PrintLine();

            Room room = IsLit() ? Context.Story.Location : Rooms.Get<Darkness>();

            Context.Output.Bold(room.Name);

            if (showFull || !Location.Visited)
                Print(room.Description);

            DisplayRoomObjects();

        }

        public static bool IsLit()
        {
            if (Location.HasLight)
                return true;

            foreach(var obj in Context.Inventory.Objects)
            {
                if (obj.HasLight)
                    return true;
                Container container = obj as Container;
                if (container != null && (container.IsOpen || container.IsTransparent))
                {
                    foreach(var containedObj in container.Contents)
                    {
                        if (containedObj.HasLight)
                            return true;
                    }
                }
                
            }

            return false;
        }

        private static void DisplayRoomObjects()
        {
            var ordinary = new List<Object>();
            int total = 0;

            foreach(var obj in Location.Objects)
            {
                if (obj.IsScenery && obj.Describe == null)
                    continue;

                if (obj.IsStatic && obj.Describe == null)
                    continue;

                total++;

                if (!obj.IsTouched && !String.IsNullOrEmpty(obj.InitialDescription))
                {
                    PrintLine();
                    Print(obj.InitialDescription);
                }
                else if (obj.Describe != null && (obj as Container) == null)
                {
                    PrintLine();
                    Print(obj.Describe());
                }
                else
                    ordinary.Add(obj);
            }

            var group = new StringBuilder();

            if (total > ordinary.Count)
                group.Append("You can also see ");
            else
                group.Append("You can see ");

            for(int i = 0; i < ordinary.Count; i++)
            {
                Object obj = ordinary[i];

                if (i == ordinary.Count - 1 && i > 0)
                    group.Append(" and ");
                else if (i > 0)
                    group.Append(", ");

                var container = obj as Container;
                if (container != null)
                {
                    if (container.Contents.Count > 0)
                    {
                        Object child = container.Contents[0];
                        group.AppendFormat("{0} {1} (which contains {2} {3})", obj.Article, obj.Name, child.Article, child.Name);
                    }
                    else
                        group.AppendFormat("{0} {1} (which is empty)", obj.Article, obj.Name);
                }
                else
                {
                    group.AppendFormat("{0} {1}", obj.Article, obj.Name);
                }
                    
            }

            group.Append(" here.");

            if (ordinary.Count > 0)
            {
                PrintLine();
                Print(group.ToString());
            }
            
    
        }

        public static Room CurrentLocation
        {
            get { return Context.Story.Location; }
        }

        public static void Quit()
        {
            if (YesOrNo("Are you sure you want to quit?"))
            {
                Story.IsDone = true;
            }
        }

        public static bool YesOrNo(string question)
        {
            Print(question);

            while (true)
            {
                string[] affirmative = new[]{"y", "yes", "yep", "yeah"};
                string[] negative = new[]{"n", "no", "nope", "nah", "naw"};
                string response = CommandPrompt.GetInput();
                if (!response.In(affirmative) && !response.In(negative))
                    Print("Please answer yes or no.");
                else
                    return (response.In(affirmative));
            }
        }

        public static void MoveObjectToInventory(Object obj)
        {
            //obj.Parent.Remove(obj);
            Location.Objects.Remove(Object);
            Inventory.Add(Object);
        }

        public static void MovePlayerTo<T>() where T : Room
        {
            var room = Rooms.Get<T>();
            MovePlayerTo(room);
        }

        public static void MovePlayerTo(Room room)
        {
            Room real = room;

            if (!IsLit())
                room = Rooms.Get<Darkness>();
            
            Story.Location = room;

            if (!room.Visited && room.Initial != null)
                room.Initial();
            else
            {
                if (!IsLit() && room.Visited)
                    real.DarkToDark();
                Look(false);
            }
                
            
            room.Visited = true;

            Story.Location = real;
        }

        public static void RunDaemons()
        {
            IList<Object> objectsWithDaemons = Objects.WithRunningDaemons();
            foreach (var obj in objectsWithDaemons)
                obj.Daemon();
        }

        public static Object GetObjectByName(string name)
        {
            var objects = from x in ObjectsInScope() where x.Name == name || x.Synonyms.Contains(name) select x;
            if (objects.Count() > 1)
            {
                //TODO: more than one object in scope with the same name
                //throw new Exception("There is more than one object in scope with the same name - Need to implement!!!!");
                foreach(var obj in objects)
                {
                    if (Inventory.Contains(obj))
                        return obj;
                }
            }

            return objects.FirstOrDefault();
        }

        public static IList<Object> ObjectsInScope()
        {
            var result = new List<Object>();
            result.AddRange(Location.Objects);
            result.AddRange(Inventory.Objects);

            var contained = new List<Object>();
            foreach(var obj in result.Where(x => x.IsOpen))
            {
                Container container = obj as Container;
                if (container != null)
                {
                    contained.AddRange(container.Contents);
                }
            }

            result.AddRange(contained);

            return result;
        }

        //protected CommandPrompt CommandPrompt
        //{
        //    get { return Context.CommandPrompt; }
        //}

        //protected Output Output
        //{
        //    get { return Context.Output; }
        //}

        protected static IStory Story
        {
            get { return Context.Story; }
        }

        protected Verb Verb
        {
            get { return Context.Verb; }
            set { Context.Verb = value; }
        }

        protected Object Object
        {
            get { return Context.Object; }
            set { Context.Object = value; }
        }

        protected Object IndirectObject
        {
            get { return Context.IndirectObject; }
            set { Context.IndirectObject = value; }
        }

        protected Inventory Inventory
        {
            get { return Context.Inventory; }
            set { Context.Inventory = value; }
        }

        protected Room Location
        {
            get { return Context.Story.Location; }
        }

        public void Print(string msg)
        {
            Output.Print(msg);
        }

        public void Print(string format, params object[] arg)
        {
            Output.Print(format, arg);
        }

        public void PrintLine()
        {
            Output.PrintLine();
        }

        public void Write(string msg)
        {
            Output.Write(msg);
        }

        public bool IsHolding(Object obj)
        {
            return Context.Inventory.Contains(obj);
        }
    }
}


