using Advent.MyRooms;
using Adventure.Net;

namespace Advent
{
    public class ColossalCave : StoryBase
    {
        public ColossalCave()
        {
            Story = "ADVENTURE";
            Headline = "By Will Crowther (1976) and Don Woods (1977)\n" +
                       "Reconstructed in three steps by:\n" +
                       "Donald Ekman, David M. Baggett (1993) and Graham Nelson (1994)\n" +
                       "Ported from Inform 6 to Adventure.Net (2009) by J.P. Hamilton";
        }

        protected override void OnInitialize()
        {
            Location = Rooms.Get<EndOfRoad>();
           
        }

    
    }
}

