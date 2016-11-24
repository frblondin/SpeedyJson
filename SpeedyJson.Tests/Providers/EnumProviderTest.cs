using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Tests.Providers
{
    public class EnumProviderTest
    {
        public class EnumProperty
        {
            public BindingFlags Flags { get; set; }
        }

        [Test]
        public void DeserializeEnumAsString()
        {
            // Arrange
            var json = @"{ ""Flags"": ""DeclaredOnly"" }";

            // Act
            var result = JsonDeserializer.Deserialize<EnumProperty>(json);

            // Assert
            Assert.That(result.Flags, Is.EqualTo(BindingFlags.DeclaredOnly));
        }

        [Test]
        public void DeserializeEnumAsInt()
        {
            // Arrange
            var json = @"{ ""Flags"": 3 }";

            // Act
            var result = JsonDeserializer.Deserialize<EnumProperty>(json);

            // Assert
            Assert.That(result.Flags, Is.EqualTo(BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly));
        }
    }
}
