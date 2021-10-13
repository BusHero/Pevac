using System;
using System.Text.Json;

namespace Pevac
{


    public static partial class Parser
    {
        private static Parser<DateTime?>? optionalDateTime;
        private static Parser<Uri?>? optionalUri;
        private static Parser<Guid?>? optionalGuid;
        private static Parser<string?>? optionalString;
        private static Parser<bool?>? optionalBool;

        /// <summary>
        /// Parse an optional <see cref="string"/> value.
        /// </summary>
        public static Parser<string?> OptionalString => optionalString ??= OptionalStringToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) =>
            {
                return Result.Success(reader.GetString());
            });

        /// <summary>
        /// Parse a nullable <see cref="bool"/> value.
        /// </summary>
        public static Parser<bool?> OptionalBool => optionalBool ??= OptionalBoolToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TokenType switch
            {
                JsonTokenType.Null => Result.Success(default(bool?)),
                _ => Result.Success<bool?>(reader.GetBoolean()),
            });

        /// <summary>
        /// Parse an optional <see cref="System.DateTime"/> value.
        /// </summary>
        public static Parser<DateTime?> OptionalDateTime => optionalDateTime ??= OptionalStringToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetNullableDateTime(out var dateTime) switch
            {
                true => Result.Success(dateTime),
                false => Result.Failure<DateTime?>()
            });

        /// <summary>
        /// Parse an optional <see cref="System.Uri"/> value.
        /// </summary>
        public static Parser<System.Uri?> OptionalUri => optionalUri ??= OptionalStringToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.GetString() switch
            {
                null => Result.Success(default(System.Uri)),
                var x when System.Uri.TryCreate(x, UriKind.RelativeOrAbsolute, out var uri) => Result.Success(uri),
                _ => Result.Failure<System.Uri>()
            });

        /// <summary>
        /// Parse a nullable <see cref="Guid"/> value.
        /// </summary>
        public static Parser<Guid?> OptionalGuid => optionalGuid ??= OptionalStringToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetNullableGuid(out var guid) switch
            {
                true => Result.Success(guid),
                false => Result.Failure<Guid?>()
            });
    }
}
