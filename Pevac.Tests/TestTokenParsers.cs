using System;
using System.Text;
using System.Text.Json;

using FluentAssertions;

using Xunit;

namespace Pevac.Tests
{
    public class ParserTests
    {
        private static Utf8JsonReader GetReader(string json, int skip)
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            Utf8JsonReader reader = new(jsonBytes);
            if (skip > 0)
                while (skip != 0)
                {
                    skip--;
                    _ = reader.Read();
                }
            return reader;
        }

        [Theory]
        [InlineData(@"{""foo"": ""bar""}", 2, "bar")]
        [InlineData(@"{""foo"": ""bar""}", 1, null)]
        [InlineData(@"{""foo"": ""bar""}", 0, null)]
        public void String(string json, int skip, string expectedResult)
        {
            var reader = GetReader(json, skip);

            var result = Parser.String(ref reader, default);
            string actualResult = result.IfFailure(default(string));
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(@"true", 0, true)]
        [InlineData(@"false", 0, false)]
        [InlineData(@"null", 0, null)]
        [InlineData(@"{""foo"": true}", 2, true)]
        [InlineData(@"{""foo"": false}", 2, false)]
        [InlineData(@"{""foo"": null}", 2, null)]
        public void OptionalBool_Passes_OnExpectedToken(string json, int skip, bool? expectedResult)
        {
            var reader = GetReader(json, skip);

            Parser<bool?> parser = Parser.OptionalBool;

            Parser.Parse(parser, ref reader, default)
                .Should().Be(expectedResult);
        }



        [Theory, MemberData(nameof(GuidParsers))]
        public void Guid(string json, int skip, Guid? expectedResult)
        {
            var reader = GetReader(json, skip);
            Parser.Parse(Parser.OptionalGuid, ref reader, default).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(@"{""foo"": ""bar""}", 1, "foo")]
        [InlineData(@"{""foo"": ""bar""}", 0, null)]
        public void PropertyName(string json, int skip, string expectedResult)
        {
            var reader = GetReader(json, skip);

            Parser.Parse(Parser.PropertyName, ref reader, default).Should().Be(expectedResult);
        }

        [Theory, MemberData(nameof(NoneParsers))]
        public void Token(Parser<Void> parser, string json, int skip, bool expectedResult)
        {
            var reader = GetReader(json, skip);

            Parser.Parse(parser, ref reader, default);
        }

        public static TheoryData<string, int, Guid?> GuidParsers()
        {
            var theoryData = new TheoryData<string, int, Guid?>();

            var guid = System.Guid.NewGuid();
            theoryData.Add($@"{{""foo"" : ""{guid}"" }}", 2, guid);
            theoryData.Add($@"{{""foo"" : ""invalid-guid"" }}", 2, null);
            theoryData.Add($@"{{""foo"" : ""invalid-guid"" }}", 1, null);
            theoryData.Add($@"{{""foo"" : ""invalid-guid"" }}", 0, null);
            return theoryData;
        }

        public static TheoryData<Parser<Void>, string, int, bool> NoneParsers { get; } = new TheoryData<Parser<Void>, string, int, bool>
        {
            { Parser.StartObjectToken, "{}", 0, true },
            { Parser.StartObjectToken, "{}", 1, false },

            { Parser.EndObjectToken, "{}", 1, true },
            { Parser.EndObjectToken, "{}", 0, false },

            { Parser.StartArrayToken, "[]", 0, true },
            { Parser.StartArrayToken, "[]", 1, false },

            { Parser.EndArrayToken, "[]", 1, true },
            { Parser.EndArrayToken, "[]", 0, false },

            { Parser.PropertyNameToken, @"{""foo"": ""bar""}", 1, true},
            { Parser.PropertyNameToken, @"{""foo"": ""bar""}", 0, false},

            { Parser.StringToken, @"{""foo"": ""bar""}", 2, true},
            { Parser.StringToken, @"{""foo"": ""bar""}", 0, false},
            { Parser.StringToken, @"{""foo"": ""bar""}", 1, false},

            { Parser.NumberToken, @"{""foo"": 123}", 2, true},
            { Parser.NumberToken, @"{""foo"": 123}", 0, false},
            { Parser.NumberToken, @"{""foo"": 123}", 1, false},

            { Parser.NumberToken, @"{""foo"": 123.123}", 2, true},
            { Parser.NumberToken, @"{""foo"": 123.123}", 0, false},
            { Parser.NumberToken, @"{""foo"": 123.123}", 1, false},

            { Parser.TrueToken, @"{""foo"": true}", 2, true},
            { Parser.TrueToken, @"{""foo"": true}", 0, false},
            { Parser.TrueToken, @"{""foo"": true}", 1, false},

            { Parser.FalseToken, @"{""foo"": false}", 2, true},
            { Parser.FalseToken, @"{""foo"": false}", 0, false},
            { Parser.FalseToken, @"{""foo"": false}", 1, false},

            { Parser.NullToken, @"{""foo"": null}", 2, true},
            { Parser.NullToken, @"{""foo"": null}", 0, false},
            { Parser.NullToken, @"{""foo"": null}", 1, false},
        };

        [Fact]
        public void Then_Succeds_WithStartAndEndObject()
        {
            string json = "{}";
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            Utf8JsonReader reader = new(bytes);

            var parser = Parser.StartObjectToken.Then(Parser.EndObjectToken);

            Parser.Parse(parser, ref reader, default);

            Assert.Equal(JsonTokenType.EndObject, reader.TokenType);
        }

        [Fact]
        public void Then_Succeds_ComplexObject()
        {
            string json = @"{""foo"": ""bar""}";
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            Utf8JsonReader reader = new(bytes);

            var parser = Parser.StartObjectToken
                .Then(_ => Parser.PropertyName)
                .Then(propertyName => Parser.String)
                .Then(@string => Parser.EndObjectToken);

            bool result = parser(ref reader, default)
                .Match(_ => true, _ => false);

            Assert.Equal(JsonTokenType.EndObject, reader.TokenType);
            Assert.True(result);
        }

        [Fact]
        public void Select_Succeds()
        {
            string json = @"{""foo"": ""bar""}";
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            Utf8JsonReader reader = new(bytes);

            var parser = Parser.StartObjectToken
                .Then(_ => Parser.PropertyName)
                .Then(propertyName => Parser.String)
                .Select(asd => new Data(asd));

            var actualResult = parser(ref reader, default).IfFailure(default(Data));
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Then_Fails_WithStartAndEndArray()
        {
            string json = "[]";
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            Utf8JsonReader reader = new(bytes);

            var parser = Parser.StartObjectToken.Then(_ => Parser.EndObjectToken);
            bool result = parser(ref reader, default)
                .Match(_ => true, _ => false);
            Assert.Equal(JsonTokenType.StartArray, reader.TokenType);
            Assert.False(result);
        }

        [Fact]
        public void LinqTests()
        {
            string json = @"{""foo"": ""bar""}";
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            Utf8JsonReader reader = new(bytes);

            var parser = from _ in Parser.StartObjectToken
                         from __ in Parser.PropertyNameToken
                         from value in Parser.String
                         from ___ in Parser.EndObjectToken
                         select new Data(value);

            var result = Parser.Parse(parser, ref reader, default);
            result.Should().BeOfType<Success<Data>>().Which.Value.Should().Be("bar");
        }

        [Fact]
        public void PropertyName_Succeds_RightPropertyName()
        {
            string json = @"{""foo"": ""bar""}";
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            Utf8JsonReader reader = new(bytes);

            var parser = from _ in Parser.StartObjectToken
                         from __ in Parser.ParsePropertyName("foo")
                         from value in Parser.String
                         from ___ in Parser.EndObjectToken
                         select new Data(value);
            
            Parser.Parse(parser, ref reader, default)
                .Should().BeOfType<Success<Data>>()
                .Which.Value.Should().Be("bar");
        }

        [Fact]
        public void ParseObject_Succeds_OnValidJson()
        {
            string json = @"{""foo"": ""bar""}";
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            Utf8JsonReader reader = new(bytes);

            Parser<Data> parser = Parser.ParseType<Data>();

            Parser.Parse(parser, ref reader, default).Should().BeOfType<Success<Data>>()
                .Which.Value.Should().Match<Data>(foo => foo.Foo == "bar");
        }

        [Fact]
        public void ParseString_Succeds_ValidJson()
        {
            string json = @"{""foo"": ""bar""}";
            var reader = GetReader(json, 2);

            var expectedData = "bar";

            Parser<string> parser = Parser.ParseString(expectedData);

            Parser.Parse(parser, ref reader, default).Should().BeOfType<Success<string>>()
                .Which.Value.Should().Be("bar");
        }

        [Fact]
        public void ParseString_Fails_InvalidJson()
        {
            var reader = GetReader(@"{""foo"": ""foo""}", 2);


            Parser<string> parser = Parser.ParseString("bar");

            Parser.Parse(parser, ref reader, default).Should().BeOfType<Failure<string>>();
        }

        [Theory]
        [InlineData(@"{""foo"": ""bar""}", 1, "foo", "bar", "foo", "bar")]
        [InlineData(@"{""foo"": ""foo""}", 1, "foo", "bar", default, default)]
        public void ParseStringProperty_Succeds_ValidJson(string json, int skip, string propertyName, string stringValue, string expectedPropertyName, string expectedStringValue)
        {
            var reader = GetReader(json, skip);
            Parser<(string, string)> parser = Parser.ParseStringProperty(propertyName, stringValue);

            var result = parser(ref reader, default);
            var (actualPropertyName, actualStringValue) = result.IfFailure(default((string, string)));

            Assert.Equal(expectedPropertyName, actualPropertyName);
            Assert.Equal(expectedStringValue, actualStringValue);
        }
    }

    public static class Utils
    {
        public static Utf8JsonReader ReadAndReturn(this Utf8JsonReader reader)
        {
            reader.Read();
            return reader;
        }
    }

    public record Data(string Foo);
}
