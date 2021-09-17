﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Pevac
{
    public static partial class Parser
    {
        /// <summary>
        /// Parse first, and if successful, then parse the second.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Parser<U?> Then<T, U>(this Parser<T?> first, Func<T?, Parser<U?>> second) => (first, second) switch
        {
            (null, _) => throw new ArgumentNullException(nameof(first)),
            (_, null) => throw new ArgumentNullException(nameof(second)),
            _ => (ref Utf8JsonReader reader, JsonSerializerOptions? options) => first(ref reader, options) switch
            {
                Success<T> success => second(success.Value)(ref reader, options),
                Failure<T> failure => Result.Failure<U>(failure.Message),
                _ => throw new ParseException()
            }
        };

        /// <summary>
        /// Parse first and if succesful, the parse the second.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Parser<U?> Then<T, U>(this Parser<T?> first, Parser<U?> second) => (first, second) switch
        {
            (null, _) => throw new ArgumentNullException(nameof(first)),
            (_, null) => throw new ArgumentNullException(nameof(second)),
            _ => (ref Utf8JsonReader reader, JsonSerializerOptions? options) => first(ref reader, options) switch
            {
                Success<T> success => second(ref reader, options),
                Failure<T> failure => Result.Failure<U>(failure.Message),
                _ => throw new ParseException()
            }
        };

        /// <summary>
        /// Parser first, if it succeds, return first, otherwise try second.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Parser<T?> Or<T>(this Parser<T?> first, Parser<T?> second) => (first, second) switch
        {
            (null, _) => throw new ArgumentNullException(nameof(first)),
            (_, null) => throw new ArgumentNullException(nameof(second)),
            _ => (ref Utf8JsonReader reader, JsonSerializerOptions? options) =>
            {
                var forFirst = reader;
                switch (first(ref forFirst, options))
                {
                    case Success<T?> success:
                        reader = forFirst;
                        return success;
                    default:
                        return second(ref reader, options);
                }
            }
        };

        /// <summary>
        /// Parse a stream of elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<T?>?> Many<T>(this Parser<T?> parser) => parser switch
        {
            null => throw new ArgumentNullException(nameof(parser)),
            not null => (ref Utf8JsonReader reader, JsonSerializerOptions? options) =>
            {
                var list = new List<T?>();
                var nextReader = reader;
                while (parser(ref nextReader, options) is Success<T?> success)
                {
                    list.Add(success.Value);
                    reader = nextReader;
                }
                return Result.Success(list.AsEnumerable());
            }
        };
    }
}
