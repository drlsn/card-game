using System.Collections.Generic;

namespace Corelibs.Basic.Collections
{
    public static class StackExtensions
    {
        public static void Pop<T>(this Stack<T> stack, int popCount)
        {
            if (popCount <= 0)
                return;

            if (popCount > stack.Count)
                popCount = stack.Count;

            for (int i = 0; i < popCount; i++)
                stack.Pop();
        }

        public static T PopOrDefault<T>(this Stack<T> stack)
        {
            if (stack.Count == 0)
                return default(T);

            return stack.Pop();
        }

        public static T PeekOrDefault<T>(this Stack<T> stack)
        {
            if (stack.Count == 0)
                return default(T);

            return stack.Peek();
        }
    }
}
