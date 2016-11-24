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
    public class DictionaryProviderTest
    {
        public class DictionaryProperty
        {
            public Dictionary<string, int> Values { get; set; }
        }
        public class IDictionaryProperty
        {
            public IDictionary<string, int> Values { get; set; }
        }

        [Test]
        public void DeserializeDictionary()
        {
            // Arrange
            var json = @"{ ""Values"": { ""a"": 1, ""b"": 2 } }";

            // Act
            var result = JsonDeserializer.Deserialize<DictionaryProperty>(json);

            // Assert
            Assert.That(result.Values, Is.Not.Null.And.Count.EqualTo(2));
            Assert.That(result.Values["a"], Is.EqualTo(1));
            Assert.That(result.Values["b"], Is.EqualTo(2));
        }

        [Test]
        public void DeserializeIDictionary()
        {
            // Arrange
            var json = @"{ ""Values"": { ""a"": 1, ""b"": 2 } }";

            // Act
            var result = JsonDeserializer.Deserialize<IDictionaryProperty>(json);

            // Assert
            Assert.That(result.Values, Is.Not.Null.And.Count.EqualTo(2));
            Assert.That(result.Values["a"], Is.EqualTo(1));
            Assert.That(result.Values["b"], Is.EqualTo(2));
        }
    }
}
