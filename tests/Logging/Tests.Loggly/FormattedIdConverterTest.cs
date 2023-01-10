using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json;
using Moq;

namespace Tests
{
    [TestFixture]
    public class FormattedIdConverterTest
    {

        [Test, AutoMoqData]
        public void CanConvert_returns_true_for_integer_types(FormattedIdConverter sut)
        {
            // Assert
            foreach (var intType in integerTypesList)
            {
                Assert.That(sut.CanConvert(intType), Is.EqualTo(true));
            }
        }

        [Test, AutoMoqData]
        public void CanConvert_returns_false_for_non_integer_primitive_types(FormattedIdConverter sut)
        {
            // Arrange
            var frameworkTypes = typeof(Type).Assembly.GetTypes()
                .Where(x => x.IsPrimitive).ToList();

            var nonIntegerTypesList = frameworkTypes.Except(integerTypesList);

            // Assert
            foreach (var intType in nonIntegerTypesList)
            {
                Assert.That(sut.CanConvert(intType), Is.EqualTo(false));
            }
        }

        readonly List<Type> integerTypesList = new List<Type>
            {
                typeof(byte), typeof(short), typeof(int), typeof(long),
                typeof(sbyte), typeof(ushort), typeof(uint), typeof(ulong),
            };


        [Test, AutoMoqData]
        public void WriteJson_calls_WriteValue_method_of_JsonWriter_for_single_value_once_only(FormattedIdConverter sut
            , JsonWriter writer
            , JsonSerializer serializer
            , byte value)
        {

            // Act
            sut.WriteJson(writer, value, serializer);

            // Assert
            Mock.Get(writer).Verify(i => i.WriteValue(It.IsAny<object>()), Times.Once);
        }

        [Test, AutoMoqData]
        public void WriteJson_calls_WriteValue_method_of_JsonWriter_for_single_value_with_the_right_value(FormattedIdConverter sut
            , JsonWriter writer
            , JsonSerializer serializer
            , byte value)
        {

            // Act
            sut.WriteJson(writer, value, serializer);

            // Assert
            Mock.Get(writer).Verify(i => i.WriteValue(value), Times.Once);
        }

        [Test, AutoMoqData]
        public void WriteJson_calls_WriteValue_method_of_JsonWriter_for_non_Id_property_once_only(FormattedIdConverter sut
            , JsonWriter writer
            , JsonSerializer serializer
            , string propertyName
            , byte value)
        {
            Assume.That(propertyName, Does.Not.EndWith("Id"));

            writer.WriteStartObject();
            writer.WritePropertyName(propertyName);

            // Act
            sut.WriteJson(writer, value, serializer);

            // Assert
            Mock.Get(writer).Verify(i => i.WriteValue(It.IsAny<object>()), Times.Once);
        }

        [Test, AutoMoqData]
        public void WriteJson_calls_WriteValue_method_of_JsonWriter_for_non_Id_property_with_the_right_value(FormattedIdConverter sut
            , JsonWriter writer
            , JsonSerializer serializer
            , string propertyName
            , byte value)
        {
            Assume.That(propertyName, Does.Not.EndWith("Id"));

            writer.WriteStartObject();
            writer.WritePropertyName(propertyName);

            // Act
            sut.WriteJson(writer, value, serializer);

            // Assert
            Mock.Get(writer).Verify(i => i.WriteValue(value), Times.Once);
        }

        [Test, AutoMoqData]
        public void WriteJson_calls_WriteValue_of_JsonWriter_for_Id_Property_once(FormattedIdConverter sut
            , JsonWriter writer
            , JsonSerializer serializer
            , string propertyName
            , byte value)
        {
            // Arrange
            writer.WriteStartObject();
            writer.WritePropertyName($"{propertyName}Id");

            // Act
            sut.WriteJson(writer, value, serializer);

            // Assert
            Mock.Get(writer).Verify(i => i.WriteValue(Convert.ToString(value)), Times.Once);
        }

        [Test, AutoMoqData]
        public void WriteJson_calls_WriteValue_of_JsonWriter_for_Id_Property_with_string_value(FormattedIdConverter sut
            , JsonWriter writer
            , JsonSerializer serializer
            , string propertyName
            , byte value)
        {
            // Arrange
            writer.WriteStartObject();
            writer.WritePropertyName($"{propertyName}Id");

            // Act
            sut.WriteJson(writer, value, serializer);

            // Assert
            Mock.Get(writer).Verify(i => i.WriteValue(Convert.ToString(value)), Times.Once);
        }

        [Test, AutoMoqData]
        public void WriteJson_checks_PropertyName_CaseInsensitive_Lower(FormattedIdConverter sut
            , JsonWriter writer
            , JsonSerializer serializer
            , string propertyName
            , byte value)
        {
            // Arrange
            writer.WriteStartObject();
            writer.WritePropertyName($"{propertyName}Id".ToLower());

            // Act
            sut.WriteJson(writer, value, serializer);

            // Assert
            Mock.Get(writer).Verify(i => i.WriteValue(Convert.ToString(value)), Times.Once);
        }

        [Test, AutoMoqData]
        public void WriteJson_checks_PropertyName_CaseInsensitive_Upper(FormattedIdConverter sut
           , JsonWriter writer
           , JsonSerializer serializer
           , string propertyName
           , byte value)
        {
            // Arrange
            writer.WriteStartObject();
            writer.WritePropertyName($"{propertyName}Id".ToUpper());

            // Act
            sut.WriteJson(writer, value, serializer);

            // Assert
            Mock.Get(writer).Verify(i => i.WriteValue(Convert.ToString(value)), Times.Once);
        }

        [Test, AutoMoqData]
        public void ReadJson_throws_NotImplementedException(FormattedIdConverter sut
            , JsonReader reader
            , Type objectType
            , object existingValue
            , JsonSerializer serializer)
        {
            // Assert
            Assert.Throws<NotImplementedException>(() => sut.ReadJson(reader, objectType, existingValue, serializer));
        }
    }
}
