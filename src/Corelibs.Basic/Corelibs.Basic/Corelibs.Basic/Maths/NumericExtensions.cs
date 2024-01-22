namespace Corelibs.Basic.Maths
{
    public static class NumericExtensions
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0)
                return min;
            else
            if (val.CompareTo(max) > 0)
                return max;

            return val;
        }
        public static T Clamp<T>(this T val, T max) where T : IComparable<T> => val.Clamp(default, max);

        public static void IncreaseBy<T>(this ref T value, T byValue, T lowerLimit, T upperLimit, bool loop = true) where T : struct
        {
            dynamic dynamicValue = value;

            dynamicValue += byValue;
            if (dynamicValue < lowerLimit)
                dynamicValue = lowerLimit;

            if (dynamicValue > upperLimit)
                dynamicValue = loop ? lowerLimit : upperLimit;
                
            value = dynamicValue;

        }

        public static bool Equals<T>(this T first, T second, T tolerance) where T : struct
        {
            dynamic dynamicFirst = first;
            dynamic dynamicSecond = second;
            
            dynamic res = dynamicFirst - dynamicSecond;
            dynamic resAbs = System.Math.Abs(res);

            return resAbs <= tolerance;
        }

        public static int GetPercent<T>(this IEnumerable<T> source, int index)
        {
            var count = source.Count();
            var percent = (index / (float) count) * 100;
            return (int) percent;
        }
    }
}
