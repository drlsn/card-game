﻿using Microsoft.AspNetCore.Mvc;
using Trinica.Api.Contracts;

namespace Trinica.Api.Extensions
{
    public static class ResultToObjectResultExtensions
    {
        public static ObjectResult To404(this ApiResult result) =>
            new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status404NotFound
            };

        public static ObjectResult To404Result(this string errorMessage) =>
            ApiResult.Failure(errorMessage).To404();

        public static ObjectResult To204(this ApiResult result) =>
            new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status204NoContent
            };

        public static ObjectResult To204Result(this string errorMessage) =>
            ApiResult.Failure(errorMessage).To204();

        public static ObjectResult To200(this ApiResult result) =>
            new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status200OK
            };

        public static ObjectResult To200Result(this string errorMessage) =>
            ApiResult.Failure(errorMessage).To200();
    }
}
