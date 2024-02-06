using Corelibs.Basic.Blocks;
using Microsoft.AspNetCore.Mvc;
using Trinica.ApiContracts;

namespace Trinica.Api.Extensions
{
    public static class ValueTaskToObjectResultExtensions
    {
        public static async Task<IActionResult> GetQueryResponse<T>(this ValueTask<ApiResult<T>> task)
            where T : class
        {
            var result = await task;
            if (result == null)
                return new NoContentResult();

            if (result.Value == null)
                return new NoContentResult();

            return new OkObjectResult(result.Value);
        }

        public static async Task<IActionResult> GetPostCommandResponse(this ValueTask<ApiResult> task)
        {
            var result = await task;
            if (result == null)
                return "Something went wrong".To404Result();

            if (!result.IsSuccess)
                return result.To404();

            return new OkObjectResult(result);
        }

        public static async Task<IActionResult> GetPatchCommandResponse(this ValueTask<ApiResult> task)
        {
            var result = await task;
            if (result == null)
                return "Something went wrong".To404Result();

            if (!result.IsSuccess)
                return result.To404();

            return new OkObjectResult(result);
        }

        public static async Task<IActionResult> GetDeleteCommandResponse(this ValueTask<ApiResult> task)
        {
            var result = await task;
            if (result == null)
                return "Something went wrong".To404Result();

            if (!result.IsSuccess)
                return result.To404();

            return result.To204();
        }

        public static async Task<IActionResult> GetPutCommandResponse(this ValueTask<ApiResult> task)
        {
            var result = await task;
            if (result == null)
                return "Something went wrong".To404Result();

            if (result.IsSuccess)
                return result.To404();

            return result.To200();
        }
    }
}
