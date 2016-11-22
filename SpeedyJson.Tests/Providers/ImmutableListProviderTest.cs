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
    public class ImmutableListProviderTest
    {
        public class ImmutableListProperty
        {
            public ImmutableList<string> Values { get; set; }
        }
        public class IImmutableListProperty
        {
            public IImmutableList<string> Values { get; set; }
        }

        [Test]
        public void DeserializeImmutableList()
        {
            // Arrange
            var json = @"{ ""Values"": [ ""a"", ""b"" ] }";

            // Act
            var result = JsonDeserializer.Deserialize<ImmutableListProperty>(json);

            // Assert
            Assert.That(result.Values, Is.Not.Null.And.Count.EqualTo(2));
            Assert.That(result.Values[0], Is.EqualTo("a"));
            Assert.That(result.Values[1], Is.EqualTo("b"));
        }

        [Test]
        public void DeserializeIImmutableList()
        {
            // Arrange
            var json = @"{ ""Values"": [ ""a"", ""b"" ] }";

            // Act
            var result = JsonDeserializer.Deserialize<IImmutableListProperty>(json);

            // Assert
            Assert.That(result.Values, Is.Not.Null.And.Count.EqualTo(2));
            Assert.That(result.Values[0], Is.EqualTo("a"));
            Assert.That(result.Values[1], Is.EqualTo("b"));
        }
    }
}
