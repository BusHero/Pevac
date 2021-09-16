using System;
using System.Text.Json;

namespace Pevac
{
    public static partial class Parser
    {
        /// <summary>
        /// Parser for <see cref="System.Text.Json.JsonTokenType.StartObject"/> token.
        /// </summary>
        public static Parser<Void> StartObjectToken { get; } = ParseToken(JsonTokenType.StartObject);
        
        /// <summary>
        /// Parser for the <see cref="System.Text.Json.JsonTokenType.EndObject"/> token.
        /// </summary>
        public static Parser<Void> EndObjectToken { get; } = ParseToken(JsonTokenType.EndObject);

        /// <summary>
        /// Parser for the <see cref="System.Text.Json.JsonTokenType.StartArray"/> token.
        /// </summary>
        public static Parser<Void> StartArrayToken { get; } = ParseToken(JsonTokenType.StartArray);

        /// <summary>
        /// Parser for the <see cref="System.Text.Json.JsonTokenType.EndArray"/> token.
        /// </summary>
        public static Parser<Void> EndArrayToken { get; } = ParseToken(JsonTokenType.EndArray);

        /// <summary>
        /// Parser for the <see cref="System.Text.Json.JsonTokenType.PropertyName"/> token.
        /// </summary>
        public static Parser<Void> PropertyNameToken { get; } = ParseToken(JsonTokenType.PropertyName);

        /// <summary>
        /// Parser for the <see cref="System.Text.Json.JsonTokenType.String"/> token.
        /// </summary>
        public static Parser<Void> StringToken { get; } = ParseToken(JsonTokenType.String);

        /// <summary>
        /// Parser for the <see cref="System.Text.Json.JsonTokenType.Number"/> token.
        /// </summary>
        public static Parser<Void> NumberToken { get; } = ParseToken(JsonTokenType.Number);

        /// <summary>
        /// Parser for the <see cref="System.Text.Json.JsonTokenType.True"/> token.
        /// </summary>
        public static Parser<Void> TrueToken { get; } = ParseToken(JsonTokenType.True);

        /// <summary>
        /// Parser for the <see cref="System.Text.Json.JsonTokenType.False"/> token.
        /// </summary>
        public static Parser<Void> FalseToken { get; } = ParseToken(JsonTokenType.False);

        /// <summary>
        /// Parser for the <see cref="System.Text.Json.JsonTokenType.True"/> 
        /// or <see cref="System.Text.Json.JsonTokenType.False"/> tokens.
        /// </summary>
        public static Parser<Void> TrueOrFalseToken { get; } = ParseToken(JsonTokenType.True, JsonTokenType.False);

        /// <summary>
        /// Parser for the <see cref="System.Text.Json.JsonTokenType.Null"/> token.
        /// </summary>
        public static Parser<Void> NullToken { get; } = ParseToken(JsonTokenType.Null);


        /// <summary>
        /// Parser for the <see cref="System.Text.Json.JsonTokenType.EndObject"/> token.
        /// </summary>
        public static Parser<DateTime?> OptionalDateTime { get; } = TryParse(StringToken.Or(NullToken), (ref Utf8JsonReader reader, out DateTime? value, JsonSerializerOptions? _) =>
        {
            bool result = false;
            (value, result) = reader.GetString() switch
            {
                null => (default(DateTime?), true),
                var str when System.DateTime.TryParse(str, out DateTime dt) => (dt, true),
                _ => (default(DateTime?), false)
            };
            return result;
        });

        

        
    }
}