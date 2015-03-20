using Ariane.Attributes;
using NUnit.Framework;

namespace Ariane.Test.Unit.Attributes
{
    [TestFixture]
    public class UriAttributeTests
    {
        [Test]
        public void Ctor_GivenRelativePath_UriKnowsItsRelative()
        {
            var uriAttrib = new UriAttribute("/relative");

            Assert.That(uriAttrib.Uri.IsAbsoluteUri, Is.False);
        }

        [Test]
        public void Ctor_GivenAbsolutePath_UriKnowsItsAbsolute()
        {
            var uriAttrib = new UriAttribute("http://www.tempuri.org/relative");

            Assert.That(uriAttrib.Uri.IsAbsoluteUri, Is.True);
        }
    }
}
