using System;
using System.Linq;
using NUnit.Framework;
using Passenger.CommandHandlers;

namespace Passenger.Test.Unit.CommandHandlers
{
    [TestFixture]
    public class PropertySelectionFailureExceptionTests
    {
        public string SomeRandomProperty { get; set; }

        [Test]
        public void Ctor_ProvidesUsefulException()
        {
            var propInfo = typeof (PropertySelectionFailureExceptionTests).GetProperties().Single();

            var ex = new PropertySelectionFailureException(propInfo, new Exception("Inner"));

            Assert.That(ex.Message, Is.StringContaining("SomeRandomProperty"));
        }
    }
}
