using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Passenger.Attributes;
using Passenger.CommandHandlers;
using Passenger.Drivers;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.PhantomJS;
using Passenger.ModelInterception;

namespace Passenger.Test.Unit.CommandHandlers
{
    [TestFixture]
    public class ElementSelectionHandlerTests
    {
        private Mock<IDriverBindings> _mockDriver;
        private ElementSelectionHandler _handler;
        private PropertyInfo _someDivPropertyInfo;
        private IdAttribute _idAttribute;
        private List<DriverBindings.IHandle> _navHandlers;
        private List<string> _collection;

       [SetUp]
        public void Setup()
        {
            _someDivPropertyInfo = typeof (SelectionTestClass).GetProperty("SomeDiv");
            _idAttribute = _someDivPropertyInfo.GetCustomAttribute<IdAttribute>();
            _navHandlers = new List<DriverBindings.IHandle>();

            _mockDriver = new Mock<IDriverBindings>();
            _mockDriver.Setup(x => x.NavigationHandlers).Returns(_navHandlers);
            var cfg = new PassengerConfiguration { Driver = _mockDriver.Object };
            _handler = new ElementSelectionHandler(cfg);
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

        [Test]
        public void SelectElement_TargetTypeIsACollection_CollectionReturned()
        {
            var stringsPi = typeof(SelectionTestClass).GetProperty("Strings");
            var idFromStrings = stringsPi.GetCustomAttribute<IdAttribute>();
            _collection = new List<string> {"1", "2", "3"};

            _navHandlers.Add(new DriverBindings.Handle<IdAttribute>((s, bindings) => _collection));

            var selected = _handler.SelectElement(idFromStrings, stringsPi);

            Assert.That(selected, Is.EqualTo(_collection));
        }

        [Test]
        public void SelectElement_TargetTypeIsACollectionOfIWebElements_CollectionReturned()
        {
            var divs = typeof(SelectionTestClass).GetProperty("ListOfSomeDiv");
            var idFromDivs = divs.GetCustomAttribute<IdAttribute>();
            var collectionOfWebElements = new List<IWebElement>();

            _navHandlers.Add(new DriverBindings.Handle<IdAttribute>((s, bindings) => collectionOfWebElements));

            var selected = _handler.SelectElement(idFromDivs, divs);

            Assert.That(selected, Is.EqualTo(collectionOfWebElements));
        }

        [Test]
        public void SelectElement_TargetTypeIsACollectionOfIWebElementsDriverReturnsAReadOnlyCollection_CollectionReturned()
        {
            var divs = typeof(SelectionTestClass).GetProperty("ListOfSomeDiv");
            var idFromDivs = divs.GetCustomAttribute<IdAttribute>();
            var collectionOfWebElements = new List<IWebElement>();
            var roCollectionOfWebElements = new ReadOnlyCollection<IWebElement>(collectionOfWebElements);

            _navHandlers.Add(new DriverBindings.Handle<IdAttribute>((s, bindings) => roCollectionOfWebElements));

            var selected = _handler.SelectElement(idFromDivs, divs);

            Assert.That(selected, Is.EqualTo(collectionOfWebElements));
        }

        [Test]
        public void SelectElement_TargetTypeIsASingleElementAndCollectionFound_FirstItemReturned()
        {
            var stringsPi = typeof(SelectionTestClass).GetProperty("String");
            var idFromStrings = stringsPi.GetCustomAttribute<IdAttribute>();
            _collection = new List<string> {"1", "2", "3"};

            _navHandlers.Add(new DriverBindings.Handle<IdAttribute>((s, bindings) => _collection));

            var selected = _handler.SelectElement(idFromStrings, stringsPi);

            Assert.That(selected, Is.EqualTo(_collection[0]));
        }
        
        public class SelectionTestClass
        {
            [Id]
            public virtual IWebElement SomeDiv { get; set; }

            [Id]
            public virtual List<string> Strings { get; set; } 

            [Id]
            public virtual string String { get; set; } 

            [Id]
            public virtual MyButton Button { get; set; }

            [Id]
            public virtual List<MyButton> Buttons { get; set; }

            [Id]
            public virtual MyButton[] ArrayOfButtons { get; set; }

            [Id]
            public virtual List<IWebElement> ListOfSomeDiv { get; set; }
        }

        public class MyButton : IPassengerElement
        {
            public IWebElement Inner { get; set; }
        }
    }
}
