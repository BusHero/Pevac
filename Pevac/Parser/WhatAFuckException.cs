using System;

namespace Pevac
{
    /// <summary>
    /// An exception that will be thrown in unexpected situations(aka. What a fuck?).
    /// </summary>
    public sealed class WhatAFuckException : Exception
    {
        internal WhatAFuckException(string? message = default) : base(message) { }
    }
}