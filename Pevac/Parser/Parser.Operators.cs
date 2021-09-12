using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Pevac
{
    public static partial class Parser
    {
        public static T Parse<T>(this Parser<T> parser, ref Utf8JsonReader reader, JsonSerializerOptions options) => parser(ref reader, options)
            .ThrowIfFailure();

        public static Parser<U> Then<T, U>(this Parser<T> first, Func<T, Parser<U>> second) => (first, second) switch
        {
            (null, _) => throw new ArgumentNullException(nameof(first)),
            (_, null) => throw new ArgumentNullException(nameof(second)),
            _ => (ref Utf8JsonReader reader, JsonSerializerOptions options) => first(ref reader, options) switch
            {
                var result when result.IsSuccess(out var value) => second(value)(ref reader, options),
                var result when result.IsFailure(out string message) => Result.Failure<U>(message),
                _ => throw new WhatAFuckException()
            }
        };

        public static Parser<U> Then<T, U>(this Parser<T> first, Parser<U> second) => (first, second) switch
        {
            (null, _) => throw new ArgumentNullException(nameof(first)),
            (_, null) => throw new ArgumentNullException(nameof(second)),
            _ => (ref Utf8JsonReader reader, JsonSerializerOptions options) => first(ref reader, options) switch
            {
                var result when result.IsSuccess(out var value) => second(ref reader, options),
                var result when result.IsFailure(out string message) => Result.Failure<U>(message),
                _ => throw new WhatAFuckException()
            }
        };



        public static Parser<T> Where<T>(this Parser<T> parser, Func<T, bool> predicate) => (parser, predicate) switch
        {
            (null, _) => throw new ArgumentNullException(nameof(parser)),
            (_, null) => throw new ArgumentNullException(nameof(predicate)),
            _ => (ref Utf8JsonReader reader, JsonSerializerOptions options) => parser(ref reader, options) switch
            {
                var result when result.IsSuccess(out var item) && !predicate(item) => Result.Failure<T>($"Unnexpected value {item}"),
                var result => result
            }
        };

        public static Parser<T> Or<T>(this Parser<T> first, Parser<T> second) => (first, second) switch
        {
            (null, _) => throw new ArgumentNullException(nameof(first)),
            (_, null) => throw new ArgumentNullException(nameof(second)),
            _ => (ref Utf8JsonReader reader, JsonSerializerOptions options) =>
            {
                var forFirst = reader;
                switch (first(ref forFirst, options))
                {
                    case var result when result.IsSuccess(out var item):
                        reader = forFirst;
                        return result;
                    default:
                        return second(ref reader, options);
                }
            }
        };

        public static Parser<T> Failure<T>(string message = "some default message") => (ref Utf8JsonReader _, JsonSerializerOptions _) => Result.Failure<T>(message);

        public static Parser<T> Return<T>(T value) => (ref Utf8JsonReader _, JsonSerializerOptions _) => Result.Success(value);

        public static Parser<U> Return<T, U>(this Parser<T> parser, U value) => parser switch
        {
            null => throw new ArgumentNullException(nameof(parser)),
            _ => parser.Select(t => value)
        };

        public static Parser<T> IfThen<T, U>(Func<T, bool> predicate, Func<T, Parser<U>> next)
        {
            return default;
        }

        public static Parser<T> ParseType<T>() => (ref Utf8JsonReader reader, JsonSerializerOptions options) =>
        {
            try
            {
                var result = JsonSerializer.Deserialize<T>(ref reader, options);
                return Result.Success(result);
            }
            catch (JsonException)
            {
                return Result.Failure<T>($"Cannot convert json to {typeof(T).Name}");
            }
        };

        public static Parser<T> ParseTypeProperty<T>(string propertyName) => from _ in ParsePropertyName(propertyName)
                                                                             from type in ParseType<T>()
                                                                             select type;

        public static Parser<IEnumerable<T>> Many<T>(this Parser<T> parser) => parser switch
        {
            null => throw new ArgumentNullException(nameof(parser)),
            not null => (ref Utf8JsonReader reader, JsonSerializerOptions options) =>
            {
                var list = new List<T>();
                var nextReader = reader;
                while (parser(ref nextReader, options).IsSuccess(out T value))
                {
                    list.Add(value);
                    reader = nextReader;
                }
                return Result.Success(list.AsEnumerable());
            }
        };
    }
}
