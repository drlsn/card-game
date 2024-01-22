using System.Collections.Generic;

namespace Corelibs.Basic.Collections
{
    public static class QueueExtensions
    {
        public static T DequeueOrDefault<T>(this Queue<T> queue)
        {
            if (queue.IsNullOrEmpty())
                return default;
            
            return queue.Dequeue();
        }

        public static T DeEnqueue<T>(this Queue<T> queue)
        {
            T element = queue.Dequeue();
            queue.Enqueue(element);
            return element;
        }

        public static void EnqueueMany<T>(this Queue<T> queue, IEnumerable<T> elements)
        {
            elements.ForEach(e => queue.Enqueue(e));
        }

        public static Queue<T> ToQueue<T>(this IEnumerable<T> source) =>
            new Queue<T>(source);
    }
}
