using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Corelibs.Basic.Collections
{
    public static class ConcurrentStackExtensions
    {
        public static bool Pop<T>(this ConcurrentStack<T> stack, int popCount)
        {
            if (popCount <= 0)
                return false;

            if (popCount > stack.Count)
                popCount = stack.Count;

            for (int i = 0; i < popCount; i++)
                if (!stack.TryPop(out var r))
                    return false;

            return true;
        }

        public static bool PopOrDefault<T>(this ConcurrentStack<T> stack, out T result)
        {
            result = default(T);
            if (stack.Count == 0)
                return true;

            return stack.TryPop(out result);
        }

        public static bool PeekOrDefault<T>(this ConcurrentStack<T> stack, out T result)
        {
            result = default(T);
            if (stack.Count == 0)
                return true;

            return stack.TryPeek(out result);
        }
    }
}
