using System;

namespace Pevac
{
    public static partial class Result
    {
        public interface ISuccess<T, U>
        {
            U Failure(Func<string, U> failure);
        }
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