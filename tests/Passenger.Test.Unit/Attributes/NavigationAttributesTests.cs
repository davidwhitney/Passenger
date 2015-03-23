using System;
using System.Collections.Generic;
using System.Linq;
using Passenger.Attributes;
using NUnit.Framework;

namespace Passenger.Test.Unit.Attributes
{
    [TestFixture]
    public class NavigationAttributesTests
    {
        public List<Type> Types
        {
            get { return typeof (NavigationAttributeBase).Assembly.GetTypes().Where(x => x.BaseType == typeof (NavigationAttributeBase)).ToList(); }
        }

        [Test]
        public void AllAttributesThatDescendFromNavigationMustTakeAStringParam()
        {
            foreach (var type in Types)
            {
                var instance = Activator.CreateInstance(type, "");

                Assert.That(instance, Is.Not.Null);
            }
        }

        [Test]
        public void AlAttributes_ToString_ReturnsValuePassedIntoCtor()
        {
            foreach (var type in Types)
            {
                var value = Guid.NewGuid().ToString();

                var instance = Activator.CreateInstance(type, value);

                Assert.That(instance.ToString(), Is.EqualTo(value));
            }
        }
    }
}
