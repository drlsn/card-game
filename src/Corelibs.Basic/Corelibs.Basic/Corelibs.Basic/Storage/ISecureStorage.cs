namespace Corelibs.Basic.Storage;

public interface ISecureStorage
{
    bool Remove(string key);
    void RemoveAll();
    Task SetAsync(string key, string value);
    Task<string> GetAsync(string key);
}
