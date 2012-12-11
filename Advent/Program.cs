using Adventure.Net;

namespace Advent
{
    class Program
    {
        static void Main(string[] args)
        {
            StoryController controller = new StoryController(new ColossalCave());
            controller.Run();
        }
    }
}
