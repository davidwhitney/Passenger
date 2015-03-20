using Ariane.CommandHandlers;
using Ariane.Drivers;
using Ariane.Test.Unit.ModelInterception;
using NUnit.Framework;

namespace Ariane.Test.Unit.CommandHandlers
{
    [TestFixture]
    public class TypeSubstitutionHandlerTests
    {
        private TypeSubstitutionHandler _handler;
        private TotallyFakeWebDriver _fakeDriver;

        [SetUp]
        public void Setup()
        {
            _fakeDriver = new TotallyFakeWebDriver();
            _handler = new TypeSubstitutionHandler(_fakeDriver);
        }

        [Test]
        public void FindSubstituteFor_GivenTypeWithSub_SubReturned()
        {
            _fakeDriver.Substitutes.Add(new DriverBindings.TypeSubstitution(typeof (string), () => "hi"));
            var propertyType = typeof (string);

            var substitute = _handler.FindSubstituteFor(propertyType);

            Assert.That(substitute, Is.TypeOf<DriverBindings.TypeSubstitution>());
            Assert.That(substitute.GetInstance(), Is.EqualTo("hi"));
        }

        [Test]
        public void FindSubstituteFor_GivenTypeWithoutSub_ReturnsNull()
        {
            var propertyType = typeof (string);

            var substitute = _handler.FindSubstituteFor(propertyType);

            Assert.That(substitute, Is.Null);
        }

        [Test]
        public void FindSubstituteFor_DriverNull_ReturnsNull()
        {
            _handler = new TypeSubstitutionHandler(null);

            var sub = _handler.FindSubstituteFor(typeof (string));

            Assert.That(sub, Is.Null);
        }
    }
}
