using Corelibs.Basic.Maths;

namespace Corelibs.Basic.Threading
{
    public static class TaskExtensions
    {
        public static T GetAwaiterResult<T>(this Task<T> task)
        {
            return task.GetAwaiter().GetResult();
        }

        public static T Result<T>(this Task<T> task, bool configureAwait = false)
        {
            return task.ConfigureAwait(configureAwait).GetAwaiter().GetResult();
        }

        public static void CheckCancellation(this CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();
        }

        public static void CheckCancellationAndSetProgress<T>(this CancellationToken ct, int index, IEnumerable<T> collection, Action<int> setProgress)
        {
            int percent = collection.GetPercent(index);
            setProgress(percent);

            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();
        }

        public static async Task<T[]> Values<T>(this IEnumerable<Task<T>> tasks, bool configureAwait = false)
        {
            return await Task.WhenAll(tasks.ToArray()).ConfigureAwait(configureAwait);
        }
    }

    public static class ThreadEx
    {
        public static Task SleepOnAnotherThread(int milliseconds) => Task.Run(() => Thread.Sleep(milliseconds));

    }
}
