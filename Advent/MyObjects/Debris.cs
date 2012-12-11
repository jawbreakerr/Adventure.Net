using Advent.MyRooms;

namespace Advent.MyObjects
{
    public class Debris : Scenic
    {
        public override void Initialize()
        {
            Name = "debris";
            Synonyms.Are("debris", "stuff", "mud");
            Description = "Yuck.";
        }
    }
}
