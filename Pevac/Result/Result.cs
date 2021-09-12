using System;

namespace Pevac
{
    public static partial class Result
    {
        public static Result<T> Failure<T>(string message = "Some default message") => Result<T>.Failure(message);

        public static Result<T> Success<T>(T value) => Result<T>.Success(value);

        public static ISuccess<T, U> Success<T, U>(this Result<T> result, Func<T?, U> success) => (result, success) switch
        {
            (null, _) => throw new ArgumentNullException(nameof(result)),
            (_, null) => throw new ArgumentNullException(nameof(success)),
            _ => new SuccessResult<T, U>(result, success)
        };

        public static IFailure<T, U> Failure<T, U>(this Result<T> result, Func<string, U> failure) => (result, failure) switch
        {
            (null, _) => throw new ArgumentNullException(nameof(result)),
            (_, null) => throw new ArgumentNullException(nameof(failure)),
            _ => new FailureResult<T, U>(result, failure)
        };

        public static T? IfFailure<T>(this Result<T> result, Func<string, T> alternative) => (result, alternative) switch
        {
            (null, _) => throw new ArgumentNullException(nameof(result)),
            (_, null) => throw new ArgumentNullException(nameof(alternative)),
            _ => result.Match(
                success: Func.Id,
                failure: alternative)
        };

        public static T? IfFailure<T>(this Result<T> result, Func<T> alternative) => result.IfFailure(_ => alternative());

        public static T IfFailure<T>(this Result<T> result, T alternative) => result.IfFailure(_ => alternative);

        public static Result<U> IfSuccess<T, U>(this Result<T> result, Func<T, Result<U>> next) => (result, next) switch
        {
            (null, _) => throw new ArgumentNullException(nameof(result)),
            (_, null) => throw new ArgumentNullException(nameof(next)),
            _ => result.Match(success: item => next(item), failure: message => Failure<U>(message))
        };
    }

    public static class Func
    {
        public static T Id<T>(T item) => item;

        public static Func<T, U> Returns2<T, U>(Func<U> item) => _ => item();

        public static Func<T, U> Returns2<T, U>(U item) => _ => item;

        public static Func<T> Returns<T>(T item) => () => item;

        public static Func<T, T> Updater<T>(Func<T, T> func) => func;
    }

    //public static class ResultHelper
    //{
    //    public static IResult<U> IfSuccess<T, U>(this IResult<T> result, Func<IResult<T>, IResult<U>> next)
    //    {
    //        if (result == null) throw new ArgumentNullException(nameof(result));

    //        if (result.WasSuccessful)
    //            return next(result);

    //        return Result.Failure<U>(result.Remainder, result.Message, result.Expectations);
    //    }

    //    public static IResult<T> IfFailure<T>(this IResult<T> result, Func<IResult<T>, IResult<T>> next)
    //    {
    //        if (result == null) throw new ArgumentNullException(nameof(result));

    //        return result.WasSuccessful
    //            ? result
    //            : next(result);
    //    }
    //}
}