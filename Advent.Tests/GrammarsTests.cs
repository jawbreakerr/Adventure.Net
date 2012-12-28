using System.Linq;
using Adventure.Net;
using NUnit.Framework;

namespace Advent.Tests
{
    public class GrammarsTests
    {
        [Test]
        public void METHOD_NAME()
        {
            var grammars = new Grammars
                {
                    {"<multi> [in,inside,into] <noun>", () => true}
                };

            var grammar = grammars.First();

        } 
    }
}