using Adventure.Net;
using ColossalCave.MyObjects;
using NUnit.Framework;

namespace Advent.Tests.Verbs
{
    public class PutTests : AdventTestFixture
    {
        [Test]
        public void what_do_you_want_to_put_the_bottle_in()
        {
            var bottle = Objects.Get<Bottle>();
            Location.Objects.Add(bottle);
            var results = parser.Parse("put bottle");
            Assert.AreEqual("What do you want to put the bottle in?", results[0]);
        }

        [Test]
        public void what_do_you_want_to_put_the_bottle_on()
        {
            var bottle = Objects.Get<Bottle>();
            Location.Objects.Add(bottle);
            var results = parser.Parse("put bottle on");
            Assert.AreEqual("What do you want to put the bottle on?", results[0]);
        }
    }
}