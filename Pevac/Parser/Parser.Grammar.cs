using System;
using System.Linq;
using System.Text.Json;

namespace Pevac
{
    public static partial class Parser
    {
        public static Parser<None> StartObjectToken { get; } = ParseToken(JsonTokenType.StartObject);
        public static Parser<None> EndObjectToken { get; } = ParseToken(JsonTokenType.EndObject);
        public static Parser<None> StartArrayToken { get; } = ParseToken(JsonTokenType.StartArray);
        public static Parser<None> EndArrayToken { get; } = ParseToken(JsonTokenType.EndArray);
        public static Parser<None> PropertyNameToken { get; } = ParseToken(JsonTokenType.PropertyName);
        public static Parser<None> StringToken { get; } = ParseToken(JsonTokenType.String);
        public static Parser<None> NumberToken { get; } = ParseToken(JsonTokenType.Number);
        public static Parser<None> TrueToken { get; } = ParseToken(JsonTokenType.True);
        public static Parser<None> FalseToken { get; } = ParseToken(JsonTokenType.False);
        public static Parser<None> TrueOrFalseToken { get; } = ParseToken(JsonTokenType.True, JsonTokenType.False);
        public static Parser<None> NullToken { get; } = ParseToken(JsonTokenType.Null);

        public static Parser<DateTime?> OptionalDateTime { get; } = TryParse(StringToken.Or(NullToken), (ref Utf8JsonReader reader, out DateTime? value, JsonSerializerOptions _) =>
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

        public static Parser<string> OptionalString { get; } = Parse(StringToken.Or(NullToken), (ref Utf8JsonReader reader, JsonSerializerOptions _) => reader.GetString());
        public static Parser<string> String { get; } = Parse(StringToken, (ref Utf8JsonReader reader, JsonSerializerOptions _) => reader.GetString());
        public static Parser<bool> Bool { get; } = Parse(TrueOrFalseToken, (ref Utf8JsonReader reader, JsonSerializerOptions _) => reader.GetBoolean());
        public static Parser<Guid> Guid { get; } = TryParse(StringToken, (ref Utf8JsonReader reader, out Guid guid, JsonSerializerOptions _) => reader.TryGetGuid(out guid));
        public static Parser<byte> Byte { get; } = TryParse(NumberToken, (ref Utf8JsonReader reader, out byte value, JsonSerializerOptions _) => reader.TryGetByte(out value));
        public static Parser<byte[]> BytesFromBase64 { get; } = TryParse(NumberToken, (ref Utf8JsonReader reader, out byte[] value, JsonSerializerOptions _) => reader.TryGetBytesFromBase64(out value));
        public static Parser<DateTime> DateTime { get; } = TryParse(StringToken.Or(NullToken), (ref Utf8JsonReader reader, out DateTime value, JsonSerializerOptions _) => reader.TryGetDateTime(out value));
        public static Parser<DateTimeOffset> DateTimeOffset { get; } = TryParse(StringToken, (ref Utf8JsonReader reader, out DateTimeOffset value, JsonSerializerOptions _) => reader.TryGetDateTimeOffset(out value));
        public static Parser<decimal> Decimal { get; } = TryParse(NumberToken, (ref Utf8JsonReader reader, out decimal value, JsonSerializerOptions _) => reader.TryGetDecimal(out value));
        public static Parser<double> Double { get; } = TryParse(NumberToken, (ref Utf8JsonReader reader, out double value, JsonSerializerOptions _) => reader.TryGetDouble(out value));
        public static Parser<short> Int16 { get; } = TryParse(NumberToken, (ref Utf8JsonReader reader, out short value, JsonSerializerOptions _) => reader.TryGetInt16(out value));
        public static Parser<int> Int32 { get; } = TryParse(NumberToken, (ref Utf8JsonReader reader, out int value, JsonSerializerOptions _) => reader.TryGetInt32(out value));
        public static Parser<long> Int64 { get; } = TryParse(NumberToken, (ref Utf8JsonReader reader, out long value, JsonSerializerOptions _) => reader.TryGetInt64(out value));
        public static Parser<sbyte> SByte { get; } = TryParse(NumberToken, (ref Utf8JsonReader reader, out sbyte value, JsonSerializerOptions _) => reader.TryGetSByte(out value));
        public static Parser<float> Single { get; } = TryParse(NumberToken, (ref Utf8JsonReader reader, out float value, JsonSerializerOptions _) => reader.TryGetSingle(out value));
        public static Parser<ushort> UInt16 { get; } = TryParse(NumberToken, (ref Utf8JsonReader reader, out ushort value, JsonSerializerOptions _) => reader.TryGetUInt16(out value));
        public static Parser<uint> UInt32 { get; } = TryParse(NumberToken, (ref Utf8JsonReader reader, out uint value, JsonSerializerOptions _) => reader.TryGetUInt32(out value));
        public static Parser<ulong> UInt64 { get; } = TryParse(NumberToken, (ref Utf8JsonReader reader, out ulong value, JsonSerializerOptions _) => reader.TryGetUInt64(out value));

        public static Parser<string> PropertyName { get; } = Parse(PropertyNameToken, (ref Utf8JsonReader reader, JsonSerializerOptions _) => reader.GetString());

        public static Parser<None> ParseToken(params JsonTokenType[] tokens) => (ref Utf8JsonReader reader, JsonSerializerOptions _) => reader.Read() switch
        {
            false => Result.Failure<None>("Cannot read the next token. You've probably reached the end of stream"),
            true when tokens.Contains(reader.TokenType) => Result.Success(None.Instance),
            _ => Result.Failure<None>(""),
        };

        
    }
}