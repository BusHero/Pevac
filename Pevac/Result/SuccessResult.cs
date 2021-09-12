using System;

namespace Pevac
{
    public static partial class Result
    {
        private class SuccessResult<T, U> : ISuccess<T, U>
        {
            private readonly Result<T> _result;
            private readonly Func<T?, U> _success;

            public U Failure(Func<string, U> failure) => failure switch
            {
                    null => throw new ArgumentNullException(nameof(failure)),
                not null => _result.Match(_success, failure),
            };

            public SuccessResult(Result<T> result, Func<T?, U> success)
            {
                _result = result ?? throw new ArgumentNullException(nameof(result));
                _success = success ?? throw new ArgumentNullException(nameof(success));
            }
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