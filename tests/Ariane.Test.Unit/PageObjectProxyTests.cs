using System.Linq;
using System.Runtime.Serialization;
using Castle.DynamicProxy;
using NUnit.Framework;
using OpenQA.Selenium.Remote;

namespace Ariane.Test.Unit
{
    [TestFixture]
    public class PageObjectProxyTests
    {
        private IInterceptor _interceptor;
        private InterceptedType _proxy;

        [SetUp]
        public void Setup()
        {
            // This sucks but it's realistic - intercept is called deep down.
            var fakeDriver = (RemoteWebDriver)FormatterServices.GetUninitializedObject(typeof(RemoteWebDriver));
            _proxy = PageObjectProxyGenerator.Generate<InterceptedType>(fakeDriver);
            _interceptor = ((IInterceptor[])_proxy.GetType().GetFields().Single(x => x.Name == "__interceptors").GetValue(_proxy)).Single();
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

        public class InterceptedType
        {
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
        }
    }
}
