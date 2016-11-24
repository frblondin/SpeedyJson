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
    public class ListProviderTest
    {
        public class ListProperty
        {
            public List<string> Values { get; set; }
        }
        public class IListProperty
        {
            public IList<string> Values { get; set; }
        }

        [Test]
        public void DeserializeList()
        {
            // Arrange
            var json = @"{ ""Values"": [ ""a"", ""b"" ] }";

            // Act
            var result = JsonDeserializer.Deserialize<ListProperty>(json);

            // Assert
            Assert.That(result.Values, Is.Not.Null.And.Count.EqualTo(2));
            Assert.That(result.Values[0], Is.EqualTo("a"));
            Assert.That(result.Values[1], Is.EqualTo("b"));
        }

        [Test]
        public void DeserializeIList()
        {
            // Arrange
            var json = @"{ ""Values"": [ ""a"", ""b"" ] }";

            // Act
            var result = JsonDeserializer.Deserialize<IListProperty>(json);

            // Assert
            Assert.That(result.Values, Is.Not.Null.And.Count.EqualTo(2));
            Assert.That(result.Values[0], Is.EqualTo("a"));
            Assert.That(result.Values[1], Is.EqualTo("b"));
        }
    }
}
