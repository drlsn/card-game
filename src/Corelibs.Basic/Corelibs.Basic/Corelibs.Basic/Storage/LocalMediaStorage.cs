using Corelibs.Basic.Blocks;

namespace Corelibs.Basic.Storage;

public class LocalMediaStorage : IMediaStorage
{
    public string BaseWritePath { get; }
    public string BaseReadPath { get; }

    public LocalMediaStorage(
        string baseWritePath, string baseReadPath)
    {
        BaseWritePath = baseWritePath;
        BaseReadPath = baseReadPath;
    }

    public Task<Result> Get(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> Save(Stream stream, string name)
    {
        string filePath = Path.Combine(BaseWritePath, name);

        string directoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        await using FileStream fs = new(filePath, FileMode.Create);
        await stream.CopyToAsync(fs);

        return Result.Success();
    }

    public async Task<Result> Delete(string name)
    {
        string filePath = Path.Combine(BaseWritePath, name);

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Result.Success();
            }
            else
            {
                // If the file does not exist, you can return an appropriate error message.
                return Result.Failure("File not found.");
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions that might occur during the deletion process.
            return Result.Failure($"Failed to delete file: {ex.Message}");
        }
    }
}

public class LocalMediaStorage<T> : LocalMediaStorage, IMediaStorage<T>
{
    public LocalMediaStorage(string baseWritePath, string baseReadPath) : base(baseWritePath, baseReadPath)
    {
    }
}
