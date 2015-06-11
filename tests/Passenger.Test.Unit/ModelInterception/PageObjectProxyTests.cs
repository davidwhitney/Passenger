using System;
using System.Collections.Generic;
using Passenger.Attributes;
using Passenger.Drivers;
using Passenger.ModelInterception;
using Moq;
using NUnit.Framework;

namespace Passenger.Test.Unit.ModelInterception
{
    [TestFixture]
    public class PageObjectProxyTests
    {
        private InterceptedType _proxy;
        private Mock<IDriverBindings> _fakeDriver;

        [SetUp]
        public void Setup()
        {
            // It's hard to hand construct a proxy - so we'll go via the proxy generator.
            _fakeDriver = new Mock<IDriverBindings>();
            _fakeDriver.Setup(x => x.Substitutes).Returns(new List<DriverBindings.TypeSubstitution>
            {
                new DriverBindings.TypeSubstitution(typeof (InterceptedType.SubbedType),
                    () => new InterceptedType.SubbedType {Val = "Hi"})
            });
            _fakeDriver.Setup(x => x.NavigationHandlers).Returns(new List<DriverBindings.IHandle>
            {
                new DriverBindings.Handle<IdAttribute>((s, d) => "an id string"),
                new DriverBindings.Handle<LinkTextAttribute>((s, d) => "a text string")
            });

            _proxy = ProxyGenerator.Generate<InterceptedType>(_fakeDriver.Object);
        }

        [Test]
        public void Intercept_ToPropertyWithoutAttribute_RegularValueReturned()
        {
            _proxy.RegularOldString = "abc";

            var value = _proxy.RegularOldString;

            Assert.That(value, Is.EqualTo("abc"));
        }

        [Test]
        public void Intercept_FieldBackedPropertyWithoutAttribute_ReturnsRegularValue()
        {
            _proxy.FieldBacked = "123";

            var value = _proxy.FieldBacked;

            Assert.That(value, Is.EqualTo("123"));
        }

        [Test]
        public void Intercept_ToMethod_RegularValueReturned()
        {
            var value = _proxy.RegularUnremarkableMethod();

            Assert.That(value, Is.EqualTo("method return"));
        }

        [Test]
        public void Intercept_SetFieldWithIdAttribute_Throws()
        {
            var ex = Assert.Throws<Exception>(() => _proxy.PropertyWithIdAttribute = "here's a value");

            Assert.That(ex.Message, Is.EqualTo("You can't set a property that has a selection attribute."));
        }

        [Test]
        public void Intercept_GetFieldWithMultipleSelectionAttributes_Throws()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                var temp =_proxy.PropertyThatThrowsOnAccessDueToMultipleAttributes;
            });

            Assert.That(ex.Message, Is.EqualTo("Only one selection attribute is valid per property."));
        }

        [Test]
        public void Intercept_FieldIsForRegisteredSubstitutedType_TypeIsSubbed()
        {
            var value = _proxy.ATypeThatIsRegisteredForSubstitution;

            Assert.That(value.Val, Is.EqualTo("Hi"));
        }

        [Test]
        public void Intercept_FieldHasMappedNavigationHandler_ReturnsValueFromHandler()
        {
            var selectorFromId = _proxy.PropertyWithIdAttribute;

            Assert.That(selectorFromId, Is.EqualTo("an id string"));
        }

        [Test]
        public void Intercept_ClassIsPageComponent_ProxyGenerated()
        {
            var component = _proxy.TotallyAPageComponent;

            Assert.That(component, Is.Not.Null);
            Assert.That(component.GetType().BaseType, Is.EqualTo(typeof(MenuBar)));
        }

        [Test]
        public void Intercept_OnAPageComponent_ProxyFeaturesWork()
        {
            var selectorFromId = _proxy.TotallyAPageComponent.StringOnComponentThatIsInterceptedToo;

            Assert.That(selectorFromId, Is.EqualTo("a text string"));
        }

        [Test]
        public void Intercept_MethodThatTransitionsCalled_CorrectPageObjectReturned()
        {
            var newPageObject = _proxy.MethodThatTransitions();

            Assert.That(newPageObject.Driver, Is.EqualTo(_fakeDriver.Object));
        }

        [Test]
        public void Intercept_MethodThatTransitionsRawCalled_ReturnsProxiedObjectForChaining()
        {
            var newPageObject = _proxy.MethodThatTransitionsRaw();

            Assert.That(newPageObject.GetType().Name, Is.EqualTo("SomethingElseProxy"));
        }

        public class InterceptedType
        {
            [Id]
            public virtual string PropertyWithIdAttribute { get; set; }

            [Id]
            [CssSelector]
            public virtual string PropertyThatThrowsOnAccessDueToMultipleAttributes { get; set; }

            public virtual string RegularOldString { get; set; }

            private string _fieldBacked;
            public virtual string FieldBacked
            {
                get { return _fieldBacked; }
                set { _fieldBacked = value; }
            }

            public virtual string RegularUnremarkableMethod()
            {
                return "method return";
            }

            public virtual MenuBar TotallyAPageComponent { get; set; }
            public virtual SubbedType ATypeThatIsRegisteredForSubstitution { get; set; }

            public virtual PageObject<SomethingElse> MethodThatTransitions()
            {
                return Arrives.AtPageObject<SomethingElse>();
            }

            public virtual SomethingElse MethodThatTransitionsRaw()
            {
                return Arrives.At<SomethingElse>();
            } 

            public class SubbedType
            {
                public string Val { get; set; }
            }
        }

        public class SomethingElse
        {
            public string Id { get; set; }

            public virtual string Method()
            {
                return "abc";
            }
        }

        [PageComponent]
        public class MenuBar
        {
            [LinkText]
            public virtual string StringOnComponentThatIsInterceptedToo { get; set; }
        }
    }
}
