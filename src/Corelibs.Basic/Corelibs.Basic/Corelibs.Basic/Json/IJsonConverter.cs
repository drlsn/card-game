namespace Corelibs.Basic.Json
{
    public interface IJsonConverter
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
    }
}
