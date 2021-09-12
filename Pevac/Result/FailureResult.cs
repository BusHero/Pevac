using System;

namespace Pevac
{
    public static partial class Result
    {
        public class FailureResult<T, U> : IFailure<T, U>
        {
            private readonly Result<T> _result;
            private readonly Func<string, U> _failure;

            public FailureResult(Result<T> result, Func<string, U> failure)
            {
                _result = result ?? throw new ArgumentNullException(nameof(result));
                _failure = failure ?? throw new ArgumentNullException(nameof(failure));
            }

            public U Success(Func<T?, U> success) => success switch
            {
                null => throw new ArgumentNullException(nameof(success)),
                not null => _result.Match(success: success, failure: _failure),
            };
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