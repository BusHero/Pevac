using System;
using System.Text.Json;

namespace Pevac
{
    public record Result<T>
    {
        private readonly T? _value;
        private readonly string? _message;

        private string Message
        {
            get => WasSuccesful switch
            {
                false => _message ?? throw new ArgumentException(),
                _ => throw new InvalidOperationException()
            };

            init
            {
                _message = value ?? throw new ArgumentNullException();
            }
        }

        public bool IsSuccess(out T? actualValue)
        {
            actualValue = WasSuccesful ? Value : default;
            return WasSuccesful;
        }

        private T? Value
        {
            get => WasSuccesful switch
            {
                true => _value,
                _ => throw new InvalidOperationException(),
            };

            init
            {
                _value = value;
                WasSuccesful = true;
            }
        }

        private bool WasSuccesful { get; init; }

        public bool IsFailure(out string? message)
        {
            message = WasSuccesful ? default : Message;
            return !WasSuccesful;
        }

        public static Result<T> Success(T value) => new() { Value = value };

        public static Result<T> Failure(string message) => message switch
        {
                null => throw new ArgumentNullException(nameof(message)),
            not null => new() { Message = message }
        };

        public U Match<U>(Func<T?, U> success, Func<string, U> failure) => (success, failure) switch
        {
            (null, _) => throw new ArgumentNullException(nameof(success)),
            (_, null) => throw new ArgumentNullException(nameof(failure)),
            var _ when WasSuccesful => success(Value),
            var _ when !WasSuccesful => failure(Message),
            _ => throw new WhatAFuckException()
        };

        public T ThrowIfFailure() => ThrowIfFailure<JsonException>();

        public T ThrowIfFailure<E>() where E : Exception, new() => WasSuccesful switch
        {
            true => Value,
            false => throw new E()
        };
    }
}