using System;
using System.Linq;
using System.Text.Json;

namespace Pevac
{
    public static partial class Parser
    {
        /// <summary>
        /// Parser for <see cref="JsonTokenType.StartObject"/> token.
        /// </summary>
        public static Parser<Void> StartObjectToken { get; } = ParseToken(JsonTokenType.StartObject);
        
        /// <summary>
        /// Parser for the <see cref="JsonTokenType.EndObject"/> token.
        /// </summary>
        public static Parser<Void> EndObjectToken { get; } = ParseToken(JsonTokenType.EndObject);

        /// <summary>
        /// Parser for the <see cref="JsonTokenType.StartArray"/> token.
        /// </summary>
        public static Parser<Void> StartArrayToken { get; } = ParseToken(JsonTokenType.StartArray);

        /// <summary>
        /// Parser for the <see cref="JsonTokenType.EndArray"/> token.
        /// </summary>
        public static Parser<Void> EndArrayToken { get; } = ParseToken(JsonTokenType.EndArray);

        /// <summary>
        /// Parser for the <see cref="JsonTokenType.PropertyName"/> token.
        /// </summary>
        public static Parser<Void> PropertyNameToken { get; } = ParseToken(JsonTokenType.PropertyName);

        /// <summary>
        /// Parser for the <see cref="JsonTokenType.String"/> token.
        /// </summary>
        public static Parser<Void> StringToken { get; } = ParseToken(JsonTokenType.String);

        /// <summary>
        /// Parser for the <see cref="JsonTokenType.String"/> 
        /// or <see cref="JsonTokenType.None"/> token.
        /// </summary>
        public static Parser<Void> OptionalStringToken { get; } = ParseToken(JsonTokenType.String, JsonTokenType.Null);

        /// <summary>
        /// Parser for the <see cref="JsonTokenType.Number"/> token.
        /// </summary>
        public static Parser<Void> NumberToken { get; } = ParseToken(JsonTokenType.Number);

        /// <summary>
        /// Parser for the <see cref="JsonTokenType.Number"/> 
        /// or <see cref="JsonTokenType.Null"/> token.
        /// </summary>
        public static Parser<Void> OptionalNumber { get; } = ParseToken(JsonTokenType.Number, JsonTokenType.Null);

        /// <summary>
        /// Parser for the <see cref="JsonTokenType.True"/> token.
        /// </summary>
        public static Parser<Void> TrueToken { get; } = ParseToken(JsonTokenType.True);

        /// <summary>
        /// Parser for the <see cref="JsonTokenType.True"/> 
        /// or <see cref="JsonTokenType.Null"/> tokens.
        /// </summary>
        public static Parser<Void> OptionalTrueToken { get; } = ParseToken(JsonTokenType.True, JsonTokenType.Null);

        /// <summary>
        /// Parser for the <see cref="JsonTokenType.False"/> token.
        /// </summary>
        public static Parser<Void> FalseToken { get; } = ParseToken(JsonTokenType.False);

        /// <summary>
        /// Parser for the <see cref="JsonTokenType.False"/> 
        /// or <see cref="JsonTokenType.Null"/> tokens.
        /// </summary>
        public static Parser<Void> OptionalFalseToken { get; } = ParseToken(JsonTokenType.False, JsonTokenType.Null);

        /// <summary>
        /// Parser for the <see cref="JsonTokenType.True"/> 
        /// or <see cref="JsonTokenType.False"/> tokens.
        /// </summary>
        public static Parser<Void> BooleanToken { get; } = ParseToken(JsonTokenType.True, JsonTokenType.False);

        /// <summary>
        /// Parser for the <see cref="JsonTokenType.True"/> 
        /// or <see cref="JsonTokenType.False"/> 
        /// or <see cref="JsonTokenType.Null"/> tokens.
        /// </summary>
        //public static Parser<Void> OptionalBoolToken { get; } = ParseToken(JsonTokenType.True, JsonTokenType.False, JsonTokenType.Null);


        public static Result<Void> OptionalBoolToken(ref Utf8JsonReader reader, JsonSerializerOptions? options)
        {
            return ParseToken(ref reader, options, JsonTokenType.True, JsonTokenType.False, JsonTokenType.Null);
        }

        /// <summary>
        /// Parser for the <see cref="JsonTokenType.Null"/> token.
        /// </summary>
        public static Parser<Void> NullToken { get; } = ParseToken(JsonTokenType.Null);

        public static Result<Void> ParseToken(
            ref Utf8JsonReader reader, 
            JsonSerializerOptions? options, 
            params JsonTokenType[] tokens) 
        {
            return reader.Read() switch
            {
                false => Result.Failure<Void>("Cannot read the next token. You've probably reached the end of stream"),
                true when tokens.Contains(reader.TokenType) => Result.Success(Void.Default),
                _ => Result.Failure<Void>(""),
            };
        }


    }
}