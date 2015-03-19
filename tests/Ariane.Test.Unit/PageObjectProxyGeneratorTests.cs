using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Castle.DynamicProxy;
using NUnit.Framework;
using OpenQA.Selenium.Remote;

namespace Ariane.Test.Unit
{
    [TestFixture]
    public class PageObjectProxyGeneratorTests
    {
        private RemoteWebDriver _fakeDriver;

        [SetUp]
        public void Setup()
        {
            _fakeDriver = (RemoteWebDriver)FormatterServices.GetUninitializedObject(typeof (RemoteWebDriver));
        }

        [Test]
        public void Generate_GivenTypeAndDriver_GeneratesProxyThatPassesTypeCheck()
        {
            var proxy = PageObjectProxyGenerator.Generate<PopgTestObject>(_fakeDriver);

            Assert.That(proxy, Is.InstanceOf<PopgTestObject>());
        }

        [Test]
        public void Generate_PassedNullDriver_Throws()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => PageObjectProxyGenerator.Generate<PopgTestObject>(null));

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: driver"));
        }

        [Test]
        public void Generate_GivenTypeAndDriver_InheritsFromPom()
        {
            var proxy = PageObjectProxyGenerator.Generate<PopgTestObject>(_fakeDriver);

            Assert.That(proxy.GetType().BaseType, Is.EqualTo(typeof(PopgTestObject)));
        }

        [Test]
        public void Generate_GivenTypeAndDriver_ReturnsInterceptedType()
        {
            var proxy = PageObjectProxyGenerator.Generate<PopgTestObject>(_fakeDriver);

            var interceptor = ((IInterceptor[])proxy.GetType().GetFields().Single(x => x.Name == "__interceptors").GetValue(proxy)).Single();

            Assert.That(interceptor, Is.TypeOf<PageObjectProxy>());
        }

        public class PopgTestObject { }
    }
}
