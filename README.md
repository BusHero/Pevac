# Pevac ![Some other name that doesn't matter](https://github.com/BusHero/Pevac/actions/workflows/build.yaml/badge.svg)

Pevac is a simple, lightweight library for constructing functional, forward only parsers that use [Utf8JsonReader](https://docs.microsoft.com/en-us/dotnet/api/system.text.json.utf8jsonreader) struct as the source of tokens. Parsers can then be used to simplify constuction of custom deserializers for the [System.Text.Json](https://docs.microsoft.com/en-us/dotnet/api/system.text.json). 

It is heavely inspired by the [Sprache](https://github.com/sprache/Sprache) parser combinator library! 

## Motivation
The scope of the project is to simplify building cusom deserializers for the [System.Text.Json](https://docs.microsoft.com/en-us/dotnet/api/system.text.json) by providing the posibility of building functional parsers that can consume an instance of [Utf8JsonReader](https://docs.microsoft.com/en-us/dotnet/api/system.text.json.utf8jsonreader) struct.

It is impossible to use the [Utf8JsonReader](https://docs.microsoft.com/en-us/dotnet/api/system.text.json.utf8jsonreader) with any existing parser combinator library because of the limitation imposed on the struct.

* [Utf8JsonReader](https://docs.microsoft.com/en-us/dotnet/api/system.text.json.utf8jsonreader) cannot be specified as a generic parameter.
* All the interaction with the [Utf8JsonReader](https://docs.microsoft.com/en-us/dotnet/api/system.text.json.utf8jsonreader) struct should be through a reference.

## Usage

Unlike most parser-building frameworks, you use [Pevac](#pevac) directly from your program code, and don't need to set up any build-time code generation tasks. [Pevac](#pevac) itself is a single tiny assembly.


[Pevac](#pevac) provides a number of build-in functions that can make bigger parsers from smaller once, often callable via Linq query comprehensions:

```csharp
Parser<string> parser = from _ in Parser.StartObject
                        from propertyName in Parser.PropertyName
                        from value in Parser.String
                        from __ in Parser.EndObject
                        select new Foo(propertyName, value)

var json = "{ \"foo\" : \"bar\" }";
var bytes = Encoding.UTF8.GetBytes(foo);
var reader = new Utf8JsonReader(bytes);
var foo = parser.Parse(reader, default);

Assert.AreEqual(new Foo("foo", "bar"), foo);

```

## Credits

[![](https://avatars1.githubusercontent.com/u/1999078?v=3&s=200)](https://github.com/sprache/Sprache)