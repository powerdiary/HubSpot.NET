using FluentAssertions;
using FluentAssertions.Execution;
using HubSpot.NET.Core.Requests;
using System.Dynamic;
using System.Reflection;

namespace HubSpot.NET.IntegrationTests.Core.Requests
{
    public class RequestDataConverterTests
    {
        private readonly RequestDataConverter _converver;

        public RequestDataConverterTests()
        {
            _converver = new RequestDataConverter();
        }

        [Fact]
        public void ConvertSingleEntity_WhenStringPropertyExists_ShouldMapCorrectly()
        {
            var sourceDto = new SourceTestDto { 
                PublicString = "TestString" 
            };
            dynamic expando = CreateExpandoWithProperties(sourceDto);

            var destDto = new DestTestDto();
            var result = InvokeConvertSingleEntity(expando, destDto) as DestTestDto;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.PublicString.Should().Be("TestString");
            }
        }

        [Fact]
        public void ConvertSingleEntity_WhenIntPropertyExists_ShouldMapCorrectly()
        {
            var sourceDto = new SourceTestDto();
            sourceDto.SetPrivateInt(100);

            dynamic expando = CreateExpandoWithProperties(sourceDto);

            var destDto = new DestTestDto();
            var result = InvokeConvertSingleEntity(expando, destDto) as DestTestDto;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var privateIntProp = typeof(DestTestDto).GetProperty("PrivateInt", BindingFlags.NonPublic | BindingFlags.Instance);
                privateIntProp.Should().NotBeNull();
                var privateIntValue = Convert.ToInt32(privateIntProp!.GetValue(result));
                privateIntValue.Should().Be(100);
            }
        }

        [Fact]
        public void ConvertSingleEntity_WhenDoublePropertyExists_ShouldMapCorrectly()
        {
            var sourceDto = new SourceTestDto();
            sourceDto.PublicDouble = 99.99;

            dynamic expando = CreateExpandoWithProperties(sourceDto);

            var destDto = new DestTestDto();
            var result = InvokeConvertSingleEntity(expando, destDto) as DestTestDto;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.PublicDouble.Should().Be(99.99);
            }
        }

        [Fact]
        public void ConvertSingleEntity_WhenBoolPropertyExists_ShouldMapCorrectly()
        {
            var sourceDto = new SourceTestDto();
            sourceDto.SetPrivateBool(true);

            dynamic expando = CreateExpandoWithProperties(sourceDto);

            var destDto = new DestTestDto();
            var result = InvokeConvertSingleEntity(expando, destDto) as DestTestDto;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var privateBoolProp = typeof(DestTestDto).GetProperty("PrivateBool", BindingFlags.NonPublic | BindingFlags.Instance);
                privateBoolProp.Should().NotBeNull();
                var privateBoolValue = Convert.ToBoolean(privateBoolProp!.GetValue(result));
                privateBoolValue.Should().BeTrue();
            }
        }

        [Fact]
        public void ConvertSingleEntity_WhenDateTimePropertyExists_ShouldMapCorrectly()
        {
            var sourceDto = new SourceTestDto();
            sourceDto.PublicDate = new DateTime(2024, 3, 26);

            dynamic expando = CreateExpandoWithProperties(sourceDto);

            var destDto = new DestTestDto();
            var result = InvokeConvertSingleEntity(expando, destDto) as DestTestDto;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.PublicDate.Should().Be(new DateTime(2024, 3, 26));
            }
        }

        [Fact]
        public void ConvertSingleEntity_WhenDecimalPropertyExists_ShouldMapCorrectly()
        {
            var sourceDto = new SourceTestDto();
            sourceDto.SetPrivateDecimal(123.45m);

            dynamic expando = CreateExpandoWithProperties(sourceDto);

            var destDto = new DestTestDto();
            var result = InvokeConvertSingleEntity(expando, destDto) as DestTestDto;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var privateDecimalProp = typeof(DestTestDto).GetProperty("PrivateDecimal", BindingFlags.NonPublic | BindingFlags.Instance);
                var privateDecimalValue = Convert.ToDecimal(privateDecimalProp!.GetValue(result));
                privateDecimalValue.Should().Be(123.45m);
            }
        }

        [Fact]
        public void ConvertSingleEntity_WhenDecimalPropertyDoesntExist_ShouldSetDefaultValue()
        {
            dynamic expando = new ExpandoObject();
            var propertiesExpando = new ExpandoObject() as IDictionary<string, object?>;
            propertiesExpando.Add("TestProperty", "TestValue");
            expando.properties = propertiesExpando;

            var destDto = new DestTestDto();
            var result = InvokeConvertSingleEntity(expando, destDto) as DestTestDto;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                var privateDecimalProp = typeof(DestTestDto).GetProperty("PrivateDecimal", BindingFlags.NonPublic | BindingFlags.Instance);
                privateDecimalProp.Should().NotBeNull();

                var privateDecimalValue = Convert.ToDecimal(privateDecimalProp!.GetValue(result!));
                privateDecimalValue.Should().Be(0);
            }
        }

        [Fact]
        public void ConvertSingleEntity_WhenDecimalPropertyIsNull_ShouldSetDefaultValue()
        {
            dynamic expando = new ExpandoObject();
            var propertiesExpando = new ExpandoObject() as IDictionary<string, object?>;
            propertiesExpando.Add("PrivateDecimal", null);
            expando.properties = propertiesExpando;

            var destDto = new DestTestDto();
            var result = InvokeConvertSingleEntity(expando, destDto) as DestTestDto;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                var privateDecimalProp = typeof(DestTestDto).GetProperty("PrivateDecimal", BindingFlags.NonPublic | BindingFlags.Instance);
                privateDecimalProp.Should().NotBeNull();

                var privateDecimalValue = Convert.ToDecimal(privateDecimalProp!.GetValue(result!));
                privateDecimalValue.Should().Be(0);
            }
        }

        [Fact]
        public void ConvertSingleEntity_WhenDecimalPropertyIsEmptyString_ShouldSetDefaultValue()
        {
            dynamic expando = new ExpandoObject();
            var propertiesExpando = new ExpandoObject() as IDictionary<string, object?>;
            propertiesExpando.Add("PrivateDecimal", "");
            expando.properties = propertiesExpando;

            var destDto = new DestTestDto();
            var result = InvokeConvertSingleEntity(expando, destDto) as DestTestDto;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                var privateDecimalProp = typeof(DestTestDto).GetProperty("PrivateDecimal", BindingFlags.NonPublic | BindingFlags.Instance);
                privateDecimalProp.Should().NotBeNull();

                var privateDecimalValue = Convert.ToDecimal(privateDecimalProp!.GetValue(result!));
                privateDecimalValue.Should().Be(0);
            }
        }

        [Fact]
        public void ConvertSingleEntity_WhenPublicDecimalPropertyDoesntExist_ShouldSetDefaultValue()
        {
            dynamic expando = new ExpandoObject();
            var propertiesExpando = new ExpandoObject() as IDictionary<string, object?>;
            propertiesExpando.Add("TestProperty", "TestValue");
            expando.properties = propertiesExpando;

            var destDto = new DestTestDto();
            var result = InvokeConvertSingleEntity(expando, destDto) as DestTestDto;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                var publicDecimalProp = typeof(DestTestDto).GetProperty("PublicDecimal", BindingFlags.Public | BindingFlags.Instance);
                publicDecimalProp.Should().NotBeNull();

                var publicDecimalValue = Convert.ToDecimal(publicDecimalProp!.GetValue(result!));
                publicDecimalValue.Should().Be(0);
            }
        }

        [Fact]
        public void ConvertSingleEntity_WhenPublicDecimalPropertyIsNull_ShouldSetDefaultValue()
        {
            dynamic expando = new ExpandoObject();
            var propertiesExpando = new ExpandoObject() as IDictionary<string, object?>;
            propertiesExpando.Add("PublicDecimal", null);
            expando.properties = propertiesExpando;

            var destDto = new DestTestDto();
            var result = InvokeConvertSingleEntity(expando, destDto) as DestTestDto;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                var publicDecimalProp = typeof(DestTestDto).GetProperty("PublicDecimal", BindingFlags.Public | BindingFlags.Instance);
                publicDecimalProp.Should().NotBeNull();

                var publicDecimalValue = Convert.ToDecimal(publicDecimalProp!.GetValue(result!));
                publicDecimalValue.Should().Be(0);
            }
        }

        [Fact]
        public void ConvertSingleEntity_WhenPublicDecimalPropertyIsEmptyString_ShouldSetDefaultValue()
        {
            dynamic expando = new ExpandoObject();
            var propertiesExpando = new ExpandoObject() as IDictionary<string, object?>;
            propertiesExpando.Add("PublicDecimal", "");
            expando.properties = propertiesExpando;

            var destDto = new DestTestDto();
            var result = InvokeConvertSingleEntity(expando, destDto) as DestTestDto;

            using (new AssertionScope())
            {
                result.Should().NotBeNull();

                var publicDecimalProp = typeof(DestTestDto).GetProperty("PublicDecimal", BindingFlags.Public | BindingFlags.Instance);
                publicDecimalProp.Should().NotBeNull();

                var publicDecimalValue = Convert.ToDecimal(publicDecimalProp!.GetValue(result!));
                publicDecimalValue.Should().Be(0);
            }
        }

        private static ExpandoObject CreateExpandoWithProperties(object dto)
        {
            dynamic expando = new ExpandoObject();
            var propertiesExpando = new ExpandoObject() as IDictionary<string, object?>;

            foreach (var prop in dto.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                propertiesExpando.Add(prop.Name, prop.GetValue(dto)); 
            }

            expando.properties = propertiesExpando;
            return expando;
        }

        private object? InvokeConvertSingleEntity(ExpandoObject expando, object dto)
        {
            var method = typeof(RequestDataConverter).GetMethod("ConvertSingleEntity", BindingFlags.NonPublic | BindingFlags.Instance);
            return method?.Invoke(_converver, new object[] { expando, dto });
        }

        private class SourceTestDto
        {
            public string PublicString { get; set; } = string.Empty;
            private int PrivateInt { get; set; }
            public double PublicDouble { get; set; }
            private bool PrivateBool { get; set; }
            public DateTime PublicDate { get; set; }
            private decimal PrivateDecimal { get; set; }

            public void SetPrivateInt(int value) => PrivateInt = value;
            public int GetPrivateInt() => PrivateInt;
            public void SetPrivateBool(bool value) => PrivateBool = value;
            public bool GetPrivateBool() => PrivateBool;
            public void SetPrivateDecimal(decimal value) => PrivateDecimal = value;
            public decimal GetPrivateDecimal() => PrivateDecimal;
        }

        private class DestTestDto
        {
            public string PublicString { get; set; } = string.Empty;
            private int PrivateInt { get; set; }
            public double PublicDouble { get; set; }
            private bool PrivateBool { get; set; }
            public DateTime PublicDate { get; set; }
            private decimal PrivateDecimal { get; set; }
            public decimal PublicDecimal { get; set; }

            public void SetPrivateInt(int value) => PrivateInt = value;
            public int GetPrivateInt() => PrivateInt;
            public void SetPrivateBool(bool value) => PrivateBool = value;
            public bool GetPrivateBool() => PrivateBool;
            public void SetPrivateDecimal(decimal value) => PrivateDecimal = value;
            public decimal GetPrivateDecimal() => PrivateDecimal;
        }
    }
}
