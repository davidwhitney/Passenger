using System.Collections.Generic;
using NUnit.Framework;
using OpenQA.Selenium;
using Passenger.ModelInterception;
using Passenger.Test.Unit.CommandHandlers;
using Passenger.Test.Unit.Fakes;

namespace Passenger.Test.Unit.ModelInterception
{
    [TestFixture]
    public class ReturnValuePostProcessorTests
    {
        private ReturnValuePostProcessor _pp;
        private PassengerConfiguration _cfg;

        [SetUp]
        public void SetUp()
        {
            _cfg = new PassengerConfiguration();
            _pp = new ReturnValuePostProcessor(_cfg);
        }

        [Test]
        public void PostProcessReturnValue_TargetTypeImplementsWrapperInterfaceAndWebElementReturned_WrappedInstanceCreated()
        {
            var ctx = new PropertySelectionContext
            {
                TargetProperty = typeof(TestTargetType).GetProperty("Button"),
                RawSelectedElement = new FakeWebElement("abc")
            };

            var returned = _pp.PostProcessReturnValue(ctx.RawSelectedElement, ctx, _cfg);
            
            Assert.That(returned, Is.InstanceOf<ElementSelectionHandlerTests.MyButton>());
            Assert.That(((ElementSelectionHandlerTests.MyButton)returned).Inner, Is.EqualTo(ctx.RawSelectedElement));
        }

        [Test]
        public void PostProcessReturnValue_TargetTypeImplementsWrapperInterfaceAndMultipleWebElementReturned_WrappedInstancesCreated()
        {
            var domElements = new List<IWebElement>
            {
                new FakeWebElement("1"),
                new FakeWebElement("2"),
                new FakeWebElement("3")
            };

            var ctx = new PropertySelectionContext
            {
                TargetProperty = typeof(TestTargetType).GetProperty("Buttons"),
                RawSelectedElement = domElements
            };

            var returned = _pp.PostProcessReturnValue(ctx.RawSelectedElement, ctx, _cfg);

            Assert.That(returned, Is.InstanceOf<List<ElementSelectionHandlerTests.MyButton>>());
            Assert.That(((List<ElementSelectionHandlerTests.MyButton>)returned)[0].Inner, Is.EqualTo(domElements[0]));
            Assert.That(((List<ElementSelectionHandlerTests.MyButton>)returned)[1].Inner, Is.EqualTo(domElements[1]));
            Assert.That(((List<ElementSelectionHandlerTests.MyButton>)returned)[2].Inner, Is.EqualTo(domElements[2]));
        }

        [Test]
        public void PostProcessReturnValue_TargetTypeImplementsWrapperInterfaceAndMultipleWebElementReturned_WrappedInstancesCreatedForArray()
        {
            var domElements = new List<IWebElement>
            {
                new FakeWebElement("1"),
                new FakeWebElement("2"),
                new FakeWebElement("3")
            };

            var ctx = new PropertySelectionContext
            {
                TargetProperty = typeof(TestTargetType).GetProperty("ArrayOfButtons"),
                RawSelectedElement = domElements
            };

            var returned = _pp.PostProcessReturnValue(ctx.RawSelectedElement, ctx, _cfg);

            Assert.That(returned, Is.InstanceOf<ElementSelectionHandlerTests.MyButton[]>());
            var buttonArray = (ElementSelectionHandlerTests.MyButton[])returned;
            Assert.That(buttonArray[0].Inner, Is.EqualTo(domElements[0]));
            Assert.That(buttonArray[1].Inner, Is.EqualTo(domElements[1]));
            Assert.That(buttonArray[2].Inner, Is.EqualTo(domElements[2]));
        }

        public class TestTargetType
        {
            public ElementSelectionHandlerTests.MyButton Button { get; set; }
            public virtual List<ElementSelectionHandlerTests.MyButton> Buttons { get; set; }
            public virtual ElementSelectionHandlerTests.MyButton[] ArrayOfButtons { get; set; }
        }
    }
}