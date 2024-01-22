using Corelibs.Basic.Blocks;

namespace Corelibs.Basic.Storage;

public interface IMediaStorage
{
    Task<Result> Get(string name);
    Task<Result> Save(Stream stream, string name);
    Task<Result> Delete(string name);
    string BaseWritePath { get; }
    string BaseReadPath { get; }
}

public interface IMediaStorage<T> : IMediaStorage {}

public static class MediaStorageExtensions
{
    public delegate Stream OpenReadStreamDelegate(long maxAllowedSize = 500 * 1024, CancellationToken cancellationToken = default);

    public static Task<Result> Save(this IMediaStorage storage, Stream stream, ref string name)
    {
        name = Path.ChangeExtension(
               Path.GetRandomFileName(),
               Path.GetExtension(name));

        return storage.Save(stream, name);
    }

    public static async Task<(Result, string)> Save(this IMediaStorage storage,
        OpenReadStreamDelegate getStream, string name)
    {
        var result = Result.Success();

        name = Path.ChangeExtension(
               Path.GetRandomFileName(),
               Path.GetExtension(name));

        try
        {
            using var stream = getStream();

            return (await storage.Save(stream, name), name);
        }
        catch (Exception ex)
        {
            return (result.Fail($"File: {name} Error: {ex.Message}"), "");
        }
    }
}
