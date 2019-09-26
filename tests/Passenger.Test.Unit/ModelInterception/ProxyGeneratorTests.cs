using System;
using System.Linq;
using System.Reflection;
using Passenger.Drivers;
using Passenger.ModelInterception;
using Castle.DynamicProxy;
using Moq;
using NUnit.Framework;
using ProxyGenerator = Passenger.ModelInterception.ProxyGenerator;

namespace Passenger.Test.Unit.ModelInterception
{
    [TestFixture]
    public class ProxyGeneratorTests
    {
        private PassengerConfiguration _fakeDriver;

        [SetUp]
        public void Setup()
        {
            _fakeDriver = new PassengerConfiguration {Driver = new Mock<IDriverBindings>().Object};
        }

        [Test]
        public void Generate_GivenTypeAndDriver_GeneratesProxyThatPassesTypeCheck()
        {
            var proxy = ProxyGenerator.Generate<PopgTestObject>(_fakeDriver);

            Assert.That(proxy, Is.InstanceOf<PopgTestObject>());
        }

        [Test]
        public void Generate_PassedNullDriverBindings_Throws()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => ProxyGenerator.Generate<PopgTestObject>(null));

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: configuration"));
        }

        [Test]
        public void Generate_GivenTypeAndDriver_InheritsFromPom()
        {
            var proxy = ProxyGenerator.Generate<PopgTestObject>(_fakeDriver);

            Assert.That(proxy.GetType().BaseType, Is.EqualTo(typeof(PopgTestObject)));
        }

        [Test]
        public void Generate_GivenTypeAndDriver_ReturnsInterceptedType()
        {
            var proxy = ProxyGenerator.Generate<PopgTestObject>(_fakeDriver);

            var interceptor = ((IInterceptor[]) proxy.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(x => x.Name == "__interceptors")
                .GetValue(proxy)).Single();

            Assert.That(interceptor, Is.TypeOf<PageObjectProxy>());
        }

        public class PopgTestObject { }
    }
}
