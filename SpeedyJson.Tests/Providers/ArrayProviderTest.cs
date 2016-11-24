using NUnit.Framework;
using SpeedyJson;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Tests.Providers
{
    public class ArrayProviderTest
    {
        public class ArrayProperty
        {
            public string[] Values { get; set; }
        }

        [Test]
        public void DeserializeArray()
        {
            // Arrange
            var json = @"{ ""Values"": [ ""a"", ""b"" ] }";

            // Act
            var result = JsonDeserializer.Deserialize<ArrayProperty>(json);

            // Assert
            Assert.That(result.Values, Is.Not.Null.And.Length.EqualTo(2));
            Assert.That(result.Values[0], Is.EqualTo("a"));
            Assert.That(result.Values[1], Is.EqualTo("b"));
        }
    }
}
