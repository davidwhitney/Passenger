using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Passenger.Attributes;
using Passenger.CommandHandlers;

namespace Passenger.Test.Unit.CommandHandlers
{
    [TestFixture]
    public class TypeMappingTests
    {
        private PassengerConfiguration _cfg = new PassengerConfiguration();

        [Test]
        public void ReturnOrMap_SourceIsNull_ReturnsNull()
        {
            var result = TypeMapping.ReturnOrMap(null, typeof(PassengerTestElement), _cfg);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ReturnOrMap_SourceAndTypeAreTheSame_ReturnsSource()
        {
            var webElement = new ChromeWebElement(null, null);

            var result = TypeMapping.ReturnOrMap(webElement, typeof(IWebElement), _cfg);

            Assert.That(result, Is.EqualTo(webElement));
        }

        [Test]
        public void ReturnOrMap_TypeIsAPassengerElement_Maps()
        {
            var webElement = new ChromeWebElement(null, null);

            var result = TypeMapping.ReturnOrMap(webElement, typeof (PassengerTestElement), _cfg);

            Assert.That(result, Is.TypeOf<PassengerTestElement>());
            Assert.That(((IPassengerElement)result).Inner, Is.EqualTo(webElement));
        }

        [Test]
        public void ReturnOrMap_TypeIsACollectionOfWebElementsAndTargetIsColOfPassengerElements_Maps()
        {
            var ele = new List<IWebElement> {new ChromeWebElement(null, null)};

            var result = TypeMapping.ReturnOrMap(ele, typeof (List<PassengerTestElement>), _cfg);

            Assert.That(result, Is.TypeOf<List<PassengerTestElement>>());
            Assert.That(((List<PassengerTestElement>)result)[0], Is.TypeOf<PassengerTestElement>());
            Assert.That(((List<PassengerTestElement>)result)[0].Inner, Is.EqualTo(ele[0]));
        }

        [Test]
        public void ReturnOrMap_UserAttemptsToUseACollectionThatWontWork_ThrowsNotSupportedException()
        {
            var ele = new List<IWebElement> {new ChromeWebElement(null, null)};

            var ex = Assert.Throws<NotSupportedException>(() => TypeMapping.ReturnOrMap(ele, typeof (ICollection), _cfg));

            Assert.That(ex.Message, Does.Contain("try using List<T>"));
        }

        public class PassengerTestElement : IPassengerElement
        {
            public IWebElement Inner { get; set; }
        }
    }
}
