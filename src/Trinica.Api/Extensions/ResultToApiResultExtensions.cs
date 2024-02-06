using Corelibs.Basic.Blocks;
using Trinica.ApiContracts;

namespace Trinica.Api.Extensions;

public static class ResultToApiResultExtensions
{
    public static ApiResult ToApiResult(this Result result) 
    {
        if (result.IsSuccess)
            return ApiResult.Success();

        return new ApiResult(
            result.AllErrors
                .Select(
                    e => new Message(MessageLevel.Error, e.ToString()))
                .ToList());
    }

    public static ApiResult<T> ToApiResult<T>(this Result<T> result)
    {
        var value = result.GetNestedValue<T>();
        if (result.IsSuccess && value is not null)
            return new ApiResult<T>(value);

        return new ApiResult<T>(
            result.AllErrors
                .Select(
                    e => new Message(MessageLevel.Error, e.ToString()))
                .ToList());
    }

    public static Task ToTaskApiResult(this Result result) =>
        Task.FromResult(result.ToApiResult());

    public static Task<ApiResult<T>> ToTaskApiResult<T>(this Result<T> result) =>
        Task.FromResult(result.ToApiResult());

    public static async Task<ApiResult> ToApiResult(this Task<Result> result) =>
        (await result).ToApiResult();

    public static async Task<ApiResult<T>> ToApiResult<T>(this Task<Result<T>> result) =>
        (await result).ToApiResult();

    public static async ValueTask<ApiResult> ToApiResult(this ValueTask<Result> result) =>
        (await result).ToApiResult();

    public static async ValueTask<ApiResult<T>> ToApiResult<T>(this ValueTask<Result<T>> result) =>
        (await result).ToApiResult();
}
