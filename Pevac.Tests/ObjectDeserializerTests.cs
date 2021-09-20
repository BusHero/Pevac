using Xunit;
using FluentAssertions;
using System.Text;
using System.Text.Json;
using System;

namespace Pevac.Tests
{
    public class ObjectDeserializerTests
    {
        private record Data(string Foo, string Bar);

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
                "foo" => from value in Parser.String
                         select new Func<Data, Data>(data => data with { Foo = value }),
                "bar" => from value in Parser.String
                         select new Func<Data, Data>(data => data with { Bar = value }),
                _ => Parser.Failure<Func<Data, Data>>()
            }).Select(updater => updater(new Data("", "")));

            Parser.Parse(parser, ref reader, default).Should().Be(new Data("foo", "bar"));
        }
    }
}
