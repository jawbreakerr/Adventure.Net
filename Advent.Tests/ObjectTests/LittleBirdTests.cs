using Advent.MyObjects;
using Adventure.Net;
using NUnit.Framework;

namespace Advent.Tests.ObjectTests
{
    [TestFixture]
    public class LittleBirdTests : AdventTestFixture
    {
        [Test]
        public void cannot_release_bird_when_its_not_in_the_cage()
        {
            var bird = Objects.Get<LittleBird>();
            Location.Objects.Add(bird);

            var results = parser.Parse("release bird");
            Assert.AreEqual("The bird is not caged now.", results[0]);
        }

        [Test]
        public void finish_bird_tests()
        {
            Assert.Fail();
        }
    }
}
