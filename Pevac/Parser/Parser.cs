using System.Text.Json;

namespace Pevac
{
    public delegate Result<T> Parser<T>(ref Utf8JsonReader input, JsonSerializerOptions options);
}
