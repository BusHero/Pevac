using Xunit;
using FluentAssertions;
using System.Text;
using System.Text.Json;
using System;
using System.Collections.Generic;

namespace Pevac.Tests
{
    public class ObjectDeserializerTests
    {
        private record Data(string Foo, string Bar)
        {
            public string Foo { get; set; } = Foo;
            public string Bar { get; set; } = Bar;
        }

        private record SubType(string Foo, string Bar, string Baz): Data(Foo, Bar) 
        {
            public string Baz { get; set; } = Baz;

            public static SubType Copy(Data data) => new(data.Foo, data.Bar, "");
        }

        [Fact]
        public void DeserializeObject()
        {
            var json = @"
            {
                ""foo"": ""foo"",
                ""bar"": ""bar""
            }";

            var bytes = Encoding.UTF8.GetBytes(json);
            var reader = new Utf8JsonReader(bytes);
            Parser<Data> parser = Parser.ParseObject(propertyName => propertyName switch
            {
                "foo" => Parser.String.Updater((string foo, Data data) => data with { Foo = foo }),
                "bar" => Parser.String.Updater((string bar, Data data) => data.Bar = bar),
                _ => Parser.Failure<Func<Data, Data>>()
            }, new Data("", ""));

            Parser.Parse(parser, ref reader, default).Should().Be(new Data("foo", "bar"));
        }

        [Fact]
        public void DeserializeStartedObject()
        {
            var json = @"
            {
                ""foo"": ""foo"",
                ""bar"": ""bar""
            }";

            var bytes = Encoding.UTF8.GetBytes(json);
            var reader = new Utf8JsonReader(bytes);
            reader.Read();
            Parser<Data> parser = Parser.ParseObject(propertyName => propertyName switch
            {
                "foo" => Parser.String.Updater((string foo, Data data) => data with { Foo = foo }),
                "bar" => Parser.String.Updater((string bar, Data data) => data.Bar = bar),
                _ => Parser.Failure<Func<Data, Data>>()
            }, new Data("", ""));

            Parser.Parse(parser, ref reader, default).Should().Be(new Data("foo", "bar"));
        }
    }
}
