using Corelibs.Basic.Collections;

namespace Corelibs.Basic.Functional
{
    public static class ConditionalAction
    {
        public static bool IfOkThenAdd<T>(this bool value, List<T> list, T item, int index)
        {
            if (!value)
                return value;

            list.InsertClamped(item, index);

            return value;
        }

        public static void IfOkOrNot(Func<bool> condition, Action onTrue, Action onFalse)
        {
            if (condition())
                onTrue();
            else
                onFalse();
        }

        public static void IfOkOrNot(bool value, Action onTrue, Action onFalse) =>
            IfOkOrNot(() => value, onTrue, onFalse);

        public static Action<T1, T2> BranchAction<T1, T2>(params Action<T1, T2>[] actions) =>
            (arg1, arg2) => actions.ForEach(a => a?.Invoke(arg1, arg2));

        public static void AssignIfNotNull<T>(ref T @object, T? value)
        {
            if (value is string strValue && strValue.IsNullOrEmpty())
                return;

            if (value != null)
                @object = value;
        }
    }
}
