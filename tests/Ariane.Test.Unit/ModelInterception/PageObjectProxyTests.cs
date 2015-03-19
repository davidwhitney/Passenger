using System;
using System.Collections.Generic;
using Ariane.Attributes;
using Ariane.Drivers;
using Ariane.ModelInterception;
using Moq;
using NUnit.Framework;

namespace Ariane.Test.Unit.ModelInterception
{
    [TestFixture]
    public class PageObjectProxyTests
    {
        private InterceptedType _proxy;

        [SetUp]
        public void Setup()
        {
            // This sucks but it's realistic - intercept is called deep down.
            var fakeDriver = new Mock<IDriverBindings>();
            fakeDriver.Setup(x => x.Substitutes).Returns(new List<DriverBindings.TypeSubstitution>
            {
                new DriverBindings.TypeSubstitution(typeof (InterceptedType.SubbedType),
                    () => new InterceptedType.SubbedType {Val = "Hi"})
            });

            _proxy = PageObjectProxyGenerator.Generate<InterceptedType>(fakeDriver.Object);
        }

        [Test]
        public void Intercept_ToPropertyWithoutAttribute_RegularValueReturned()
        {
            _proxy.AString = "abc";

            var value = _proxy.AString;

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
            var value = _proxy.AMethod();

            Assert.That(value, Is.EqualTo("method return"));
        }

        [Test]
        public void Intercept_SetFieldWithIdAttribute_Throws()
        {
            var ex = Assert.Throws<Exception>(() => _proxy.IdAttributed = "here's a value");

            Assert.That(ex.Message, Is.EqualTo("You can't set a property that has a selection attribute."));
        }

        [Test]
        public void Intercept_GetFieldWithMultipleSelectionAttributes_Throws()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                var temp =_proxy.MultiAttributed;
            });

            Assert.That(ex.Message, Is.EqualTo("Only one selection attribute is valid per property."));
        }

        [Test]
        public void Intercept_FieldIsForRegisteredSubstitutedType_TypeIsSubbed()
        {
            var value = _proxy.ASubstitutedType;

            Assert.That(value.Val, Is.EqualTo("Hi"));
        }

        [Test]
        public void Intercept_FieldHasMappedNvaigationHandler_ReturnsValueFromHandler()
        {
            var fakeDriver = new TotallyFakeWebDriver();
            fakeDriver.NavigationHandlers.Add(new DriverBindings.Handle<IdAttribute>(attribute => null, (s, bindings) => "value"));
            var proxy = PageObjectProxyGenerator.Generate<InterceptedType>(fakeDriver);

            var selectorFromId = proxy.IdAttributed;

            Assert.That(selectorFromId, Is.EqualTo("value"));
        }

        public class InterceptedType
        {
            [Id]
            public virtual string IdAttributed { get; set; }

            [Id]
            [CssSelector]
            public virtual string MultiAttributed { get; set; }

            public virtual string AString { get; set; }

            private string _fieldBacked;
            public virtual string FieldBacked
            {
                get { return _fieldBacked; }
                set { _fieldBacked = value; }
            }

            public virtual string AMethod()
            {
                return "method return";
            }

            public virtual SubbedType ASubstitutedType { get; set; }

            public class SubbedType
            {
                public string Val { get; set; }
            }
        }
    }
}
