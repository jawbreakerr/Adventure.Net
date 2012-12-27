using ColossalCave.MyObjects;
using Adventure.Net;
using NUnit.Framework;

namespace Advent.Tests.ObjectTests
{
    [TestFixture]
    public class WickerCageTests : AdventTestFixture
    {
        [Test]
        public void bird_should_fly_out()
        {
            var bird = Objects.Get<LittleBird>();
            var cage = Objects.Get<WickerCage>();
            cage.Add(bird);
            cage.IsOpen = false;
            Inventory.Add(cage);

            var results = parser.Parse("open cage");
            Assert.AreEqual("(releasing the little bird)", results[0]);

            Assert.IsFalse(cage.Contains<LittleBird>());
            Assert.IsTrue(cage.IsOpen);
            Assert.IsTrue(bird.AtLocation);

        }

      
    }
}
