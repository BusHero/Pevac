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
        /// <summary>
        /// Creates a parser that succeds if the current token is any of <paramref name="tokens"/>, otherwise fails.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static Parser<Void> ParseToken(params JsonTokenType[] tokens) => (ref Utf8JsonReader reader, JsonSerializerOptions? _) =>
        {
            return reader.Read() switch
            {
                false => Result
                .Failure<Void>("Cannot read the next token. You've probably reached the end of stream"),
                true when tokens.Contains(reader.TokenType) => Result
                .Success(Void.Default),
                _ => Result
                .Failure<Void>(""),
            };
        };

        /// <summary>
        /// Creates a parser that succeds if a object of type <typeparamref name="T"/> 
        /// can be obtained from the current position of the <see cref="Utf8JsonReader"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Parser<T?> ParseType<T>() => (ref Utf8JsonReader reader, JsonSerializerOptions? options) =>
        {
            try
            {
                var result = JsonSerializer.Deserialize<T>(ref reader, options);
                return Result
                .Success(result);
            }
            catch (JsonException)
            {
                return Result
                .Failure<T>($"Cannot convert json to {typeof(T).Name}");
            }
        };
        
        /// <summary>
        /// Creates a parser that succeds if the current position of the reader contains a value that.
        /// </summary>
        /// <param name="expectedValue"></param>
        /// <returns></returns>
        public static Parser<string?> ParseString(params string[] expectedValue) => (ref Utf8JsonReader reader, JsonSerializerOptions? options) =>
        {
            return String(ref reader, options) switch
            {
                Success<string?> success when !expectedValue.Contains(success.Value) =>
                    Result
                .Failure<string>($"\"{expectedValue}\" was expected, but \"{success.Value}\" recieved"),
                var result => result
            };
        };
    }
}
