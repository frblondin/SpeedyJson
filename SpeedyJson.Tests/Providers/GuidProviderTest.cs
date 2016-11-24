using NUnit.Framework;
using Ploeh.AutoFixture.NUnit3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyJson.Tests.Providers
{
    public class GuidProviderTest
    {
        public class GuidProperty
        {
            public Guid? Value { get; set; }
        }
        public class NullableGuidProperty
        {
            public Guid? Value { get; set; }
        }

        [Test, AutoData]
        public void DeserializeGuid(Guid guid)
        {
            // Arrange
            var json = $@"{{ ""Value"": ""{guid.ToString()}"" }}";

            // Act
            var result = JsonDeserializer.Deserialize<GuidProperty>(json);

            // Assert
            Assert.That(result.Value, Is.EqualTo(guid));
        }

        [Test, AutoData]
        public void DeserializeNullableGuid(Guid guid)
        {
            // Arrange
            var json = $@"{{ ""Value"": ""{guid.ToString()}"" }}";

            // Act
            var result = JsonDeserializer.Deserialize<NullableGuidProperty>(json);

            // Assert
            Assert.That(result.Value, Is.EqualTo(guid));
        }

        [Test]
        public void DeserializeNullableGuidNull()
        {
            // Arrange
            var json = @"{{ ""Value"": """" }}";

            // Act
            var result = JsonDeserializer.Deserialize<NullableGuidProperty>(json);

            // Assert
            Assert.That(result.Value, Is.Null);
        }
    }
}
