using System;

namespace Pevac
{
    public static partial class Parser
    {
        public static Parser<U> Select<T, U>(this Parser<T> parser, Func<T, U> convert) => (parser, convert) switch
        {
            (null, _) => throw new ArgumentNullException(nameof(parser)),
            (_, null) => throw new ArgumentNullException(nameof(convert)),
            _ => parser.Then(t => {
                var u = convert(t);
                return Pevac.Parser.Return(u);
            })
        };


        public static Parser<V> SelectMany<T, U, V>(this Parser<T> parser, Func<T, Parser<U>> selector, Func<T, U, V> projector) => (parser, selector, projector) switch
        {
            (null, _, _) => throw new ArgumentNullException(nameof(parser)),
            (_, null, _) => throw new ArgumentNullException(nameof(selector)),
            (_, _, null) => throw new ArgumentNullException(nameof(projector)),
            _ => parser.Then(t => selector(t).Select(u => projector(t, u)))
        };
    }
}
