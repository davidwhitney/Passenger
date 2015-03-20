using System.Collections.Generic;
using System.Reflection;
using Ariane.Attributes;
using Ariane.CommandHandlers;
using Ariane.Drivers;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Ariane.Test.Unit.CommandHandlers
{
    [TestFixture]
    public class ElementSelectionHandlerTests
    {
        private Mock<IDriverBindings> _mockDriver;
        private ElementSelectionHandler _handler;
        private PropertyInfo _someDivPropertyInfo;
        private IdAttribute _idAttribute;
        private List<DriverBindings.IHandle> _navHandlers;

        [SetUp]
        public void Setup()
        {
            _someDivPropertyInfo = typeof (SelectionTestClass).GetProperty("SomeDiv");
            _idAttribute = _someDivPropertyInfo.GetCustomAttribute<IdAttribute>();
            _navHandlers = new List<DriverBindings.IHandle>();

            _mockDriver = new Mock<IDriverBindings>();
            _mockDriver.Setup(x => x.NavigationHandlers).Returns(_navHandlers);
            _handler = new ElementSelectionHandler(_mockDriver.Object);
        }

        [Test]
        public void SelectElement_AttributeNull_ReturnsNull()
        {
            var result =_handler.SelectElement(null, _someDivPropertyInfo);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void SelectElement_PropertyInfoNull_ReturnsNull()
        {
            var result = _handler.SelectElement(_idAttribute, null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void SelectElement_NoTextValueProvidedByAttribute_PropertyNamePassedToAttributeHandler()
        {
            string namePassedIn = "";
            _navHandlers.Add(new DriverBindings.Handle<IdAttribute>((s, bindings) => { namePassedIn = s; return "empty"; }));

            _handler.SelectElement(_idAttribute, _someDivPropertyInfo);

            Assert.That(namePassedIn, Is.EqualTo("SomeDiv"));
        }

        [Test]
        public void SelectElement_NoHandlerAvailableForNavigationAttribute_ThrowsExceptionWithImportantInformationInMessage()
        {
            var ex =
                Assert.Throws<NavigationTypeNotSupportedException>(
                    () => _handler.SelectElement(_idAttribute, _someDivPropertyInfo));

            Assert.That(ex.Message, Is.StringContaining("IdAttribute"));
            Assert.That(ex.Message, Is.StringContaining("SomeDiv"));
        }

        public class SelectionTestClass
        {
            [Id]
            public virtual IWebElement SomeDiv { get; set; }
        }
    }
}
