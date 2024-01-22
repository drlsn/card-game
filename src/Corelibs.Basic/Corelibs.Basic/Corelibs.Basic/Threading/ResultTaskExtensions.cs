using Corelibs.Basic.Blocks;
using System;
using System.Threading.Tasks;

namespace Corelibs.Basic.Threading
{
    public static class ResultTaskExtensions
    {
        public static T Value<T>(this Task<Result<T>> task, bool configureAwait = false)
        {
            return task.ConfigureAwait(configureAwait).GetAwaiter().GetResult().Get();
        }

        public static async Task<Result<T>> SetValueIfExists<T>(this Task<Result<T>> task, Action<T> setValue)
        {
            var result = await task;
            if (result.IsSuccess)
            {
                var value = result.Get();
                if (value != null)
                    setValue(value);
            }

            return result;
        }

        public static async Task<T> Set<T>(this Task<Result<T>> task, Result result)
        {
            var resultLocal = await task.AddTo(result);
            return resultLocal.Get();
        }

        public static async Task<T[]> Set<T>(this Task<Result<T[]>> task, Result result)
        {
            var resultLocal = await task.AddTo(result);
            return resultLocal.Get();
        }

        public static async Task<Result> With(this Task<Result> task, Result result)
        {
            var resultLocal = await task.AddTo(result);
            return resultLocal.With(result);
        }
    }
}
