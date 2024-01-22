using Corelibs.Basic.Blocks;
using System;
using System.Threading.Tasks;

namespace Corelibs.Basic.Threading
{
    public static class ResultTask
    {
        public static Task Run<TReturnValue>(
            Func<Task<Result<TReturnValue>>> function, Action<TReturnValue> onSuccess, Action onError = null)
        {
            return Task.Run(async () =>
            {
                var res = await function();
                if (res.IsSuccess)
                    onSuccess(res.Get<TReturnValue>());
                else
                    onError?.Invoke();
            });
        }

        public static Task Run(
            Func<Task<Result>> function, Action onSuccess, Action onError = null)
        {
            return Task.Run(async () =>
            {
                var res = await function();
                if (res.IsSuccess)
                    onSuccess();
                else
                    onError?.Invoke();
            });
        }
    }
}
