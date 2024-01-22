using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Corelibs.Basic.Blocks
{
    public static class ResultConvertExtensions
    {
        public static Result ToResult(this IEnumerable<Result> subResults) =>
            Result.Create(subResults);

        public static Task<Result> ToResultTask(this IEnumerable<Result> subResults)
        {
            return Result.CreateTask(subResults);
        }

        public static Task<Result> ToResultTask(this Result result)
        {
            return Result.CreateTask(result);
        }

        public static Result<T> ToResult<T>(this IEnumerable<Result> subResults)
        {
            return Result<T>.Create(subResults);
        }

        public static Task<Result<T>> ToResultTask<T>(this IEnumerable<Result> subResults)
        {
            return Result<T>.CreateTask(subResults);
        }

        public static Result<T> ToResult<T>(this Result result)
        {
            return Result<T>.Create(result);
        }

        public static Task<Result<T>> ToResultTask<T>(this Result result)
        {
            return Result<T>.CreateTask(result);
        }

        public static Result ToResult(this object obj)
        {
            return Result.Success(obj);
        }

        public static Result<T> ToResult<T>(this T obj)
        {
            return Result<T>.Success(obj);
        }

        public static Task<Result<T>> ToResultTask<T>(this T obj)
        {
            return Task.FromResult(Result<T>.Success(obj));
        }

        public static async Task<Result> ToSuccessResult(this Task task)
        {
            await task;
            return Result.Success();
        }

        public static Type GetTypeValue<TType>(this Result result)
        {
            return result.GetValues<Type>().Where(t => t == typeof(TType)).FirstOrDefault();
        }

        public static async Task<Result<T1>> AddTo<T1, T2>(this Task<Result<T1>> resultTask, Result<T2> myResult)
        {
            var result = await resultTask;

            myResult.Add(result);
            return result;
        }

        public static async Task<Result<T>> AddTo<T>(this Task<Result<T>> resultTask, Result myResult)
        {
            var result = await resultTask;

            myResult.Add(result);
            return result;
        }

        public static async Task<Result<T>[]> AddTo<T>(this Task<Result<T>[]> resultTask, Result myResult)
        {
            var result = await resultTask;

            myResult.Add(result);
            return result;
        }

        public static async Task<T> AddAndGet<T>(this Task<Result<T>> resultTask, Result myResult)
        {
            var result = await resultTask;
            myResult.Add(result);

            var value = result.Get();
            if (!result.IsSuccess)
                return default;

            return value;
        }

        public static async Task<Result> AddTo(this Task<Result> resultTask, Result myResult)
        {
            var result = await resultTask;

            myResult.Add(result);
            return result;
        }

        public static T GetNestedValue<T>(this Result result)
        {
            if (result.Value == null)
            {
                foreach (var subResult in result.SubResults)
                {
                    var subValue = GetNestedValue<T>(subResult);
                    if (subValue != null)
                        return subValue;
                }

                return default;
            }

            var desiredType = typeof(T);
            var valueType = result.Value.GetType();

            if (valueType.Equals(desiredType))
                return (T) result.Value;

            if (valueType.IsSubclassOf(desiredType))
                return (T) result.Value;

            if (desiredType.IsInterface && valueType.GetInterface(desiredType.Name) != null)
                return (T) result.Value;

            return default;
        }
    }
}
