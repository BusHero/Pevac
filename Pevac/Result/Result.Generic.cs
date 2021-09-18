using System;
using System.Text.Json;

namespace Pevac
{
    /// <summary>
    /// Represents a result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public abstract record Result<T> 
    {
        internal Result() { }
    }

    /// <summary>
    /// Represents a succesfull result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public record Success<T>(T Value) : Result<T>;

    /// <summary>
    /// Represents a failed result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public record Failure<T>(string Message) : Result<T>
    {
        /// <summary>
        /// Repack the failure.
        /// </summary>
        public Failure<U> Repack<U>() => new(Message);
    }
}