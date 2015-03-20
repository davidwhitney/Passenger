using System.Linq;
using Ariane.Attributes;
using Ariane.ModelInterception;
using NUnit.Framework;

namespace Ariane.Test.Unit.ModelInterception
{
    [TestFixture]
    public class InvocationExtensionsTests
    {
        [TestCase("AComponent", true)]
        [TestCase("NotAComponent", false)]
        public void IsPageComponent_PassedComponent_Detects(string propName, bool result)
        {
            var propertyInfoOfComponent = typeof (SomePom).GetProperty(propName);

            var isComponent = propertyInfoOfComponent.IsPageComponent();

            Assert.That(isComponent, Is.EqualTo(result));
        }

        [TestCase("get_AProperty", true)]
        [TestCase("NotAProperty", false)]
        public void IsProperty_GivenPropertiesAndMethods_Detects(string memberName, bool result)
        {
            var getProp = typeof (SomePom).GetMembers().Single(x=> x.Name == memberName);

            var isComponent = getProp.IsProperty();

            Assert.That(isComponent, Is.EqualTo(result));
        }

        [TestCase("set_AProperty", true)]
        [TestCase("get_AProperty", false)]
        public void IsSetProperty_GivenAutoPropertyMethods_DetectsSet(string memberName, bool result)
        {
            var getProp = typeof (SomePom).GetMembers().Single(x=> x.Name == memberName);

            var isSet = getProp.IsSetProperty();

            Assert.That(isSet, Is.EqualTo(result));
        }
        
        [Test]
        public void ToPropertyInfo_GivenAutoPropertyMethod_ReturnsPropertyInfo()
        {
            var getProp = typeof(SomePom).GetMethods().Single(x => x.Name == "get_AProperty");

            var propertyInfo = getProp.ToPropertyInfo();

            Assert.That(propertyInfo, Is.Not.Null);
            Assert.That(propertyInfo.Name, Is.EqualTo("AProperty"));
        }

        public class SomePom
        {
            public virtual SomeComponent AComponent { get; set; }
            public virtual string NotAComponent { get; set; }
            public virtual string AProperty { get; set; }
            public void NotAProperty() { }
        }

        [PageComponent]
        public class SomeComponent
        {
        }
    }
}
