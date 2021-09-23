using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pevac
{
    public static partial class Parser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public static Parser<Func<U, U>> Updater<T, U>(this Parser<T> parser, Func<T, U, U> func) => 
            from t in parser
            select new Func<U, U>(u => func(t, u));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="parser"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Parser<Func<U, U>> Updater<T, U>(this Parser<T> parser, Action<T, U> action) =>
            from t in parser
            select new Func<U, U>(u =>
            {
                action(t, u);
                return u;
            });
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="parser"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Parser<Func<U, U>> Updater<T, U>(this Parser<T> parser, Func<U, U> func) =>
            from _ in parser
            select func;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="parser"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Parser<Func<U, U>> Updater<T, U>(this Parser<T> parser, Action<U> action) =>
            from _ in parser
            select new Func<U, U>(u =>
            {
                action(u);
                return u;
            });

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<Func<U, U>> Updater<T, U>(this Parser<T> parser) =>
            from _ in parser
            select new Func<U, U>(u => u);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parserSelector"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public static Parser<T> ParseObject<T>(Func<string, Parser<Func<T, T>>> parserSelector, T @default)
        {
            return parserSelector switch
            {
                null => throw new ArgumentNullException(nameof(parserSelector)),
                not null => PropertyName
                    .SelectMany(propertyName => parserSelector(propertyName), (_, updater) => updater)
                    .Many()
                    .Between(StartObjectToken, EndObjectToken)
                    .Select(updaters => new Func<T, T>(t => updaters.Aggregate(t, (data, updater) => updater(data))))
                    .Select(updater => updater(@default))
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Parser<Func<T, T>?> ParseObject<T>(Func<string, Parser<Func<T, T>>> parserSelector)
        {
            return parserSelector switch
            {
                null => throw new ArgumentNullException(nameof(parserSelector)),
                not null => from _ in StartObjectToken
                            from updaters in
                                (from propertyName in PropertyName
                                 from updater in parserSelector(propertyName)
                                 select updater).Many()
                            from __ in EndObjectToken
                            select new Func<T, T>(t => updaters.Aggregate(t, (data, updater) => updater(data)))
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="parserSelector"></param>
        /// <param name="cast"></param>
        /// <returns></returns>
        public static Parser<Func<TParent, TParent>?> ParseObject<TParent, TChild>(
            Func<string, Parser<Func<TChild, TChild>>> parserSelector,
            Func<TParent, TChild> cast) where TChild : TParent => 
            from updater in ParseObject(parserSelector)
            select new Func<TParent, TParent>(parent => updater(cast(parent)));

        
    }
}
