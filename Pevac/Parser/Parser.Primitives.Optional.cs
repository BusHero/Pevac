using System;
using System.Text.Json;

namespace Pevac
{
    

    public static partial class Parser
    {
        /// <summary>
        /// Parse an optional <see cref="string"/> value.
        /// </summary>
        public static Result<string?> OptionalString(ref Utf8JsonReader reader, JsonSerializerOptions? options)
        {
            return TryParse(OptionalStringToken, ref reader, options) switch
            {
                Success<Void> => Result.Success(reader.GetString()),
                Failure<Void> failure => failure.Repack<string?>(),
                _ => throw new ParseException(),
            };
        }

        /// <summary>
        /// Parse a nullable <see cref="bool"/> value.
        /// </summary>
        public static Result<bool?> OptionalBool(ref Utf8JsonReader reader, JsonSerializerOptions? options)
        {
            return TryParse(OptionalBoolToken, ref reader, options) switch
            {
                Success<Void> => reader.TokenType switch
                {
                    JsonTokenType.Null => Result.Success(default(bool?)),
                    _ => Result.Success<bool?>(reader.GetBoolean()),
                },
                Failure<Void> failure => failure.Repack<bool?>(),
                _ => throw new ParseException()
            };
        }

        /// <summary>
        /// Parse an optional <see cref="System.DateTime"/> value.
        /// </summary>
        public static Parser<DateTime?> OptionalDateTime { get; } = OptionalStringToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetNullableDateTime(out var dateTime) switch
            {
                true => Result.Success(dateTime),
                false => Result.Failure<DateTime?>()
            });

        /// <summary>
        /// Parse an optional <see cref="System.Uri"/> value.
        /// </summary>
        public static Parser<System.Uri?> OptionalUri { get; } = OptionalStringToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetUri(UriKind.RelativeOrAbsolute, out var uri) switch
            {
                true => Result.Success(uri),
                false => Result.Failure<Uri>()
            });

        /// <summary>
        /// Parse a nullable <see cref="Guid"/> value.
        /// </summary>
        public static Parser<Guid?> OptionalGuid { get; } = OptionalStringToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetNullableGuid(out var guid) switch
            {
                true => Result.Success(guid),
                false => Result.Failure<Guid?>()
            });
    }
}
