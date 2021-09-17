using System;

namespace Pevac.Internals
{
    internal static class Func
    {
        internal static T Id<T>(T item) => item;

        internal static Func<T, U> Returns2<T, U>(Func<U> item) => _ => item();

        internal static Func<T, U> Returns2<T, U>(U item) => _ => item;

        internal static Func<T> Returns<T>(T item) => () => item;

        internal static Func<T, T> Updater<T>(Func<T, T> func) => func;
    }
}