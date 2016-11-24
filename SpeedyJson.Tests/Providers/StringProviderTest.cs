using NUnit.Framework;
using Ploeh.AutoFixture.NUnit3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Tests.Providers
{
    public class StringProviderTest
    {
        public class StringProperty
        {
            public string Value { get; set; }
        }

        [Test]
        public void DeserializeString()
        {
            // Arrange
            var json = @"{ ""Value"": ""abcd"" }";

            // Act
            var result = JsonDeserializer.Deserialize<StringProperty>(json);

            // Assert
            Assert.That(result.Value, Is.EqualTo("abcd"));
        }
    }
}
