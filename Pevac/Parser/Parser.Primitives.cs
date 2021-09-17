using System;
using System.Linq;
using System.Text.Json;

namespace Pevac
{
    public partial class Parser
    {
        /// <summary>
        /// Parse a <see cref="string"/> value.
        /// </summary>
        public static Parser<string?> String { get; } = StringToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) =>
            {
                return Result.Success(reader.GetString());
            });

        /// <summary>
        /// Parse a <see cref="bool"/> value.
        /// </summary>
        public static Parser<bool> Bool { get; } = BooleanToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) =>
            { 
                return Result.Success(reader.GetBoolean()); 
            });

        /// <summary>
        /// Parse a <see cref="System.Guid"/> value.
        /// </summary>
        public static Parser<Guid> Guid { get; } = StringToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetGuid(out var guid) switch
            {
                true => Result.Success(guid),
                false => Result.Failure<Guid>()
            });

        /// <summary>
        /// Parse a <see cref="byte"/> value.
        /// </summary>
        public static Parser<byte> Byte { get; } = NumberToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetByte(out var value) switch
            {
                true => Result.Success(value),
                false => Result.Failure<byte>("Cannot convert the current token to a byte value")
            });

        /// <summary>
        /// Parse a bynary value codded as an array <see cref="byte"/>.
        /// </summary>
        public static Parser<byte[]?> BytesFromBase64 { get; } = NumberToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetBytesFromBase64(out var value) switch
            {
                true => Result.Success(value),
                false => Result.Failure<byte[]?>("Cannot convert the current token into a byte array")
            });

        /// <summary>
        /// Parse a <see cref="System.DateTime"/> value.
        /// </summary>
        public static Parser<DateTime> DateTime { get; } = OptionalStringToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetDateTime(out var value) switch
            {
                true => Result.Success(value),
                false => Result.Failure<DateTime>()
            });

        /// <summary>
        /// Parse a <see cref="System.DateTimeOffset"/> value.
        /// </summary>
        public static Parser<DateTimeOffset> DateTimeOffset { get; } = StringToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetDateTimeOffset(out var value) switch
            {
                true => Result.Success(value),
                false => Result.Failure<DateTimeOffset>()
            });

        /// <summary>
        /// Parse a <see cref="decimal"/> value.
        /// </summary>
        public static Parser<decimal> Decimal { get; } = NumberToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetDecimal(out var value) switch
            {
                true => Result.Success(value),
                false => Result.Failure<decimal>()
            });

        /// <summary>
        /// Parse a <see cref="double"/> value.
        /// </summary>
        public static Parser<double> Double { get; } = NumberToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetDouble(out var value) switch
            {
                true => Result.Success(value),
                false => Result.Failure<double>()
            });

        /// <summary>
        /// Parse a <see cref="short"/> value.
        /// </summary>
        public static Parser<short> Int16 { get; } = NumberToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetInt16(out var value) switch
            {
                true => Result.Success(value),
                false => Result.Failure<short>()
            });

        /// <summary>
        /// Parse a <see cref="int"/> value.
        /// </summary>
        public static Parser<int> Int32 { get; } = NumberToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetInt32(out var value) switch
            {
                true => Result.Success(value),
                false => Result.Failure<int>()
            });

        /// <summary>
        /// Parse a <see cref="long"/> value.
        /// </summary>
        public static Parser<long> Int64 { get; } = NumberToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetInt64(out var value) switch
            {
                true => Result.Success(value),
                false => Result.Failure<long>()
            });

        /// <summary>
        /// Parse a <see cref="sbyte"/> value.
        /// </summary>
        public static Parser<sbyte> SByte { get; } = NumberToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetSByte(out var value) switch
            {
                true => Result.Success(value),
                false => Result.Failure<sbyte>()
            });

        /// <summary>
        /// Parse a <see cref="float"/> value.
        /// </summary>
        public static Parser<float> Single { get; } = NumberToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetSingle(out var value) switch
            {
                true => Result.Success(value),
                false => Result.Failure<float>()
            });

        /// <summary>
        /// Parse a <see cref="ushort"/> value.
        /// </summary>
        public static Parser<ushort> UInt16 { get; } = NumberToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetUInt16(out var value) switch
            {
                true => Result.Success(value),
                false => Result.Failure<ushort>()
            });

        /// <summary>
        /// Parse a <see cref="uint"/> value.
        /// </summary>
        public static Parser<uint> UInt32 { get; } = NumberToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetUInt32(out var value) switch
            {
                true => Result.Success(value),
                false => Result.Failure<uint>()
            });

        /// <summary>
        /// Parse a <see cref="ulong"/> value.
        /// </summary>
        public static Parser<ulong> UInt64 { get; } = NumberToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) => reader.TryGetUInt64(out var value) switch
            {
                true => Result.Success(value),
                false => Result.Failure<ulong>()
            });


        /// <summary>
        /// Parse a property name value.
        /// </summary>
        public static Parser<string?> PropertyName { get; } = PropertyNameToken
            .Then((ref Utf8JsonReader reader, JsonSerializerOptions? _) =>
            {
                return Result.Success(reader.GetString());
            });


        /// <summary>
        /// Parse a <see cref="System.Uri"/> value.
        /// </summary>
        public static Parser<System.Uri?> Uri { get; } = from uri in String
                                                         select new Uri(uri);

        /// <summary>
        /// 
        /// </summary>
        public static Parser<Void> EmptyObject { get; } = StartObjectToken.Then(EndObjectToken);

    }
}
