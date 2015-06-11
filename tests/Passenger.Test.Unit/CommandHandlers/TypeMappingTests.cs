using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using Passenger.Attributes;
using Passenger.CommandHandlers;

namespace Passenger.Test.Unit.CommandHandlers
{
    [TestFixture]
    public class TypeMappingTests
    {
        [Test]
        public void ReturnOrMap_SourceIsNull_ReturnsNull()
        {
            var result = TypeMapping.ReturnOrMap(null, typeof(PassengerTestElement));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ReturnOrMap_SourceAndTypeAreTheSame_ReturnsSource()
        {
            var webElement = new PhantomJSWebElement(null, null);

            var result = TypeMapping.ReturnOrMap(webElement, typeof(IWebElement));

            Assert.That(result, Is.EqualTo(webElement));
        }

        [Test]
        public void ReturnOrMap_TypeIsAPassengerElement_Maps()
        {
            var webElement = new PhantomJSWebElement(null, null);

            var result = TypeMapping.ReturnOrMap(webElement, typeof (PassengerTestElement));

            Assert.That(result, Is.TypeOf<PassengerTestElement>());
            Assert.That(((IPassengerElement)result).Inner, Is.EqualTo(webElement));
        }

        [Test]
        public void ReturnOrMap_TypeIsACollectionOfWebElementsAndTargetIsColOfPassengerElements_Maps()
        {
            var ele = new List<IWebElement> {new PhantomJSWebElement(null, null)};

            var result = TypeMapping.ReturnOrMap(ele, typeof (List<PassengerTestElement>));

            Assert.That(result, Is.TypeOf<List<PassengerTestElement>>());
            Assert.That(((List<PassengerTestElement>)result)[0], Is.TypeOf<PassengerTestElement>());
            Assert.That(((List<PassengerTestElement>)result)[0].Inner, Is.EqualTo(ele[0]));
        }

        [Test]
        public void ReturnOrMap_UserAttemptsToUseACollectionThatWontWork_ThrowsNotSupportedException()
        {
            var ele = new List<IWebElement> {new PhantomJSWebElement(null, null)};

            var ex = Assert.Throws<NotSupportedException>(() => TypeMapping.ReturnOrMap(ele, typeof (ICollection)));

            Assert.That(ex.Message, Is.StringContaining("try using List<T>"));
        }

        public class PassengerTestElement : IPassengerElement
        {
            public IWebElement Inner { get; set; }
        }
    }
}
