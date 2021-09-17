using System;
using System.Text.Json;

namespace Pevac
{
    /// <summary>
    /// Represents a parser.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="input">The input to the parser.</param>
    /// <param name="options">The options to the parser.</param>
    /// <returns>The result of the parser.</returns>
    public delegate Result<T> Parser<T>(ref Utf8JsonReader input, JsonSerializerOptions? options = default);

    public static partial class Parser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <param name="reader"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Result<T?>? TryParse<T>(this Parser<T?> parser, ref Utf8JsonReader reader, JsonSerializerOptions? options) => parser switch
        {
            null => throw new ArgumentNullException(nameof(parser)),
            not null => parser(ref reader, options),
        };

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <param name="reader"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T? Parse<T>(this Parser<T?> parser, ref Utf8JsonReader reader, JsonSerializerOptions? options) => parser(ref reader, options) switch
        {
            null => throw new ParseException("Parse value returned a null value."),
            Success<T> success => success.Value,
            Failure<T> => throw new ParseException("The result of the parse was a failure"),
            _ => throw new ParseException("You shouldn't really see this"),
        };

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Parser<T?> Failure<T>(string message = "some default message") => (ref Utf8JsonReader _, JsonSerializerOptions? _) => Result.Failure<T>(message);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Parser<T?> Return<T>(T? value) => (ref Utf8JsonReader _, JsonSerializerOptions? _) => Result.Success(value);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="parser"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Parser<U?> Return<T, U>(this Parser<T?> parser, U? value) => parser switch
        {
            null => throw new ArgumentNullException(nameof(parser)),
            _ => parser.Select(t => value)
        };
    }
}
