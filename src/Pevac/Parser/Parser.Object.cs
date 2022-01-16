namespace Pevac;

using System;
using System.Linq;
using System.Text.Json;

public static partial class Parser
{
    /// <summary>
    /// Creates an updater.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <returns></returns>
    public static Parser<Func<U, U>> Updater<T, U>(this Parser<T> parser, Func<T, U, U> func) =>
        from t in parser
        select new Func<U, U>((U u) => func(t, u));

    /// <summary>
    /// Creates an updater.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="parser"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Parser<Func<U, U>> Updater<T, U>(this Parser<T> parser, Action<T, U> action) =>
        from t in parser
        select new Func<U, U>((U u) =>
        {
            action(t, u);
            return u;
        });

    /// <summary>
    /// Creates an updater.
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
    /// Creates an updater.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="parser"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Parser<Func<U, U>> Updater<T, U>(this Parser<T> parser, Action<U> action) =>
        from _ in parser
        select new Func<U, U>((U u) =>
        {
            action(u);
            return u;
        });

    /// <summary>
    /// Creates an identity updater
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="parser"></param>
    /// <returns></returns>
    public static Parser<Func<U, U>> Updater<T, U>(this Parser<T> parser) =>
        from _ in parser
        select new Func<U, U>((U u) => u);

    public static Parser<Func<T, T>> Updater<T>(this Parser<object> parser) => parser.Then(Parser.Return((T t) => t));

    /// <summary>
    /// Parses an object with fields
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parserSelector"></param>
    /// <param name="default"></param>
    /// <returns></returns>
    public static Parser<T> ParseObject<T>(Func<string, Parser<Func<T, T>>> parserSelector, T @default) => parserSelector switch
    {
        null => throw new ArgumentNullException(nameof(parserSelector)),
        not null => from updater in ParseObjectProperties(parserSelector)
                    select updater(@default)
    };

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parserSelector"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Parser<T> ParseObject<T>(Func<string, Parser<Func<T, T>>> parserSelector) where T : new() => parserSelector switch
    {
        null => throw new ArgumentNullException(nameof(parserSelector)),
        not null => from updater in ParseObjectProperties(parserSelector)
                    select updater(new T())
    };


    /// <summary>
    /// Parses an object with fields
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Parser<Func<T, T>> ParseObjectProperties<T>(Func<string, Parser<Func<T, T>>> parserSelector) => parserSelector switch
    {
        null => throw new ArgumentNullException(nameof(parserSelector)),
        not null => PropertyName
            .SelectMany(propertyName => parserSelector(propertyName), (_, updater) => updater)
            .Many()
            .Between(ParseCurrentToken(JsonTokenType.StartObject).Or(StartObjectToken), EndObjectToken)
            .Select(updaters => new Func<T, T>((T t) => updaters.Aggregate(t, (data, updater) => updater(data))))
    };

    /// <summary>
    /// Parses an object with fields.
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <typeparam name="TChild"></typeparam>
    /// <param name="parserSelector"></param>
    /// <param name="cast"></param>
    /// <returns></returns>
    public static Parser<Func<TParent, TParent>> ParseObject<TParent, TChild>(
        Func<string, Parser<Func<TChild, TChild>>> parserSelector,
        Func<TParent, TChild> cast) where TChild : TParent =>
        from updater in ParseObjectProperties(parserSelector)
        select new Func<TParent, TParent>((TParent parent) => updater(cast(parent)));

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <returns></returns>
    public static Parser<Func<T, T>?> FailUpdate<T>(string message = "Some default message") => Failure<Func<T, T>>(message);

}
