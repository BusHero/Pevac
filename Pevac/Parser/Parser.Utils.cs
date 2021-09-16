using System;
using System.Linq;
using System.Text.Json;

namespace Pevac
{
    /// <summary>
    /// Contains a bunch of utilities.
    /// </summary>
    public static partial class Parser
    {
        public static Parser<System.Uri> Uri { get; } = from uri in String
                                                        select new Uri(uri);


        public static Parser<System.Uri> OptionalUri { get; } = (from stringUri in OptionalString
                                                                 select System.Uri.TryCreate(stringUri, UriKind.RelativeOrAbsolute, out var uri) switch
                                                                 {
                                                                     _ => uri
                                                                 });

        public static Parser<Guid?> OptionalGuid { get; } = from stringGuid in OptionalString
                                                            select System.Guid.TryParse(stringGuid, out var guid) switch
                                                            {
                                                                true => guid,
                                                                false => default(System.Guid?)
                                                            };


        public static Parser<Void> EmptyObject { get; } = StartObjectToken.Then(EndObjectToken);

        public static Parser<string> ParseString(params string[] expectedValue) => (ref Utf8JsonReader reader, JsonSerializerOptions? options) => String(ref reader, options) switch
        {
            Success<string?> success when !expectedValue.Contains(success.Value) => 
                Result.Failure<string>($"\"{expectedValue}\" was expected, but \"{success.Value}\" recieved"),
            var result => result
        };

        public static Parser<string?> ParsePropertyName(string expectedPropertyName) => (ref Utf8JsonReader reader, JsonSerializerOptions? options) => PropertyName(ref reader, options) switch
        {
            Success<string> success when success.Value != expectedPropertyName => 
                Result.Failure<string>($"\"{expectedPropertyName}\" was expected but \"{success.Value}\" was instead"),
            var result => result
        };

        public static Parser<(string, string)> ParseStringProperty(string expectedPropertyName) =>
            from propertyName in ParsePropertyName(expectedPropertyName)
            from stringValue in String
            select (propertyName, stringValue);


        public static Parser<(string, string)> ParseStringProperty(string expectedPropertyName, string expectedStringValue) => 
            from propertyName in ParsePropertyName(expectedPropertyName)
            from stringValue in ParseString(expectedStringValue)
            select (propertyName, stringValue);

        public static Parser<T> ParseObject<T>(Parser<T> parser) => from _ in StartObjectToken
                                                                    from item in parser
                                                                    from __ in EndObjectToken
                                                                    select item;

        public static Parser<Func<T, T>> Object<T>(this Parser<Func<string, Parser<Func<T, T>>>> parser) =>
            from parserSelector in parser
            from updater in ParseObject(parserSelector)
            select updater;

        public static Parser<T> ParseObject<T>(Func<string, Parser<Func<T, T>>> parserSelector, T initial) =>
            from updater in ParseObject(parserSelector)
            select updater(initial);

        public static Parser<Func<U, U>> ParseObject<T, U>(Func<string, Parser<Func<T, T>>> parserSelector, Func<U, T> cast) where T : U =>
            from updater in ParseObject(parserSelector)
            select new Func<U, U>(u => updater(cast(u)));
        
        public static Parser<Func<T, T>> ParseObject<T>(Func<string, Parser<Func<T, T>>> parserSelector) =>
            from _ in StartObjectToken
            from updater in ParseStartedObject(parserSelector)
            select updater;

        public static Parser<T> ParseStartedObject<T>(Func<string, Parser<Func<T, T>>> parserSelector, T initial) =>
            from updater in ParseStartedObject(parserSelector)
            select updater(initial);

        public static Parser<Func<T, T>> ParseStartedObject<T>(Func<string, Parser<Func<T, T>>> parserSelector) =>
            from updaters in (
                from propertyName in PropertyName
                from updater in parserSelector(propertyName)
                select updater)
                .Many()
            from __ in EndObjectToken
            select new Func<T, T>(t => updaters.Aggregate(t, (data, updater) => updater(data)));

        public static Parser<Func<TParent, TParent>> ParseEmptySubType<TParent>(
            Func<TParent, TParent> cast) =>
            EmptyObject.Select(_ => cast);

        public static Parser<Func<TParent, TParent>> ParseSubType<TChild, TParent>(
            Func<string, Parser<Func<TChild, TChild>>> parserSelector,
            Func<TParent, TChild> cast) where TChild : TParent, new() =>
            Parser.ParseObject<TChild, TParent>(parserSelector, cast);

        public static Parser<Func<T, T>> FailedUpdater<T>() => Parser.Failure<Func<T, T>>();

        private static Parser<T> Parse<T>(Parser<Void> parser, Get<T> get) => (ref Utf8JsonReader reader, JsonSerializerOptions? options) => parser(ref reader, options) switch
        {
            Failure<Void> failure => Result.Failure<T>(failure.Message),
            Success<Void> => Result.Success(get(ref reader, options)),
            _ => throw new WhatAFuckException()
        };

        private static Parser<T> TryParse<T>(Parser<Void> parser, TryGet<T> tryGet) => (ref Utf8JsonReader reader, JsonSerializerOptions? options) => parser(ref reader, options) switch
        {
            Failure<Void> failure => Result.Failure<T>(failure.Message),
            Success<Void> success when tryGet(ref reader, out T item) => Result.Success<T>(item),
            _ => Result.Failure<T>($"Cannot get a token from \"{reader.GetString()}\"")
        };

        private delegate bool TryGet<T>(ref Utf8JsonReader reader, out T value, JsonSerializerOptions? options = default);
        private delegate T Get<T>(ref Utf8JsonReader reader, JsonSerializerOptions? options = default);
    }
}
