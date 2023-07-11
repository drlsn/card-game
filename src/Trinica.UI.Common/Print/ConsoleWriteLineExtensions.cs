using System.Reflection;

namespace Trinica.UI.Common.Print;

public static class ConsoleWriteLineExtensions
{
    public static void WritePublicFields(this object obj)
    {
        var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
        fields.ForEach(f =>
        {
            Console.WriteLine($"{f.Name}: {f.GetValue(obj)}");
        });

        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
        properties.ForEach(f =>
        {
            Console.WriteLine($"{f.Name}: {f.GetValue(obj) ?? "null"}");
        });
    }
}


