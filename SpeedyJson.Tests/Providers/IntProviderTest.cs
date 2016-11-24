using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Tests.Providers
{
    public class IntProviderTest
    {
        public class IntProperty
        {
            public int Value { get; set; }
        }
        public class NullableIntProperty
        {
            public int? Value { get; set; }
        }

        [Test]
        public void DeserializeInt()
        {
            // Arrange
            var json = @"{ ""Value"": 123 }";

            // Act
            var result = JsonDeserializer.Deserialize<IntProperty>(json);

            // Assert
            Assert.That(result.Value, Is.EqualTo(123));
        }

        [Test]
        public void DeserializeIntQuoted()
        {
            // Arrange
            var json = @"{ ""Value"": ""123"" }";

            // Act
            var result = JsonDeserializer.Deserialize<IntProperty>(json);

            // Assert
            Assert.That(result.Value, Is.EqualTo(123));
        }

        [Test]
        public void DeserializeNullableInt()
        {
            // Arrange
            var json = @"{ ""Value"": 123 }";

            // Act
            var result = JsonDeserializer.Deserialize<NullableIntProperty>(json);

            // Assert
            Assert.That(result.Value, Is.EqualTo(123));
        }

        [Test]
        public void DeserializeNullableIntNull()
        {
            // Arrange
            var json = @"{ ""Value"": """" }";

            // Act
            var result = JsonDeserializer.Deserialize<NullableIntProperty>(json);

            // Assert
            Assert.That(result.Value, Is.Null);
        }
    }
}
