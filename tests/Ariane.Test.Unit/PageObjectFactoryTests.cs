using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Ariane.Test.Unit
{
    [TestFixture]
    public class PageObjectFactoryTests
    {
        private PageObjectFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new PageObjectFactory();
        }

        [Test]
        public void CanGenerateAProxy()
        {
            var instance = _factory.GetPage<GoogleHomepage>();

            instance.SearchBox.Click();
        }
    }

    public class GoogleHomepage
    {
        [ById("search")]
        public virtual IWebElement SearchBox { get; set; }
    }

}
