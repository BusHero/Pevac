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
}
