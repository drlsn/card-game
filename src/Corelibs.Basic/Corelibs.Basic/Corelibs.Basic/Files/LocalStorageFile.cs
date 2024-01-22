using Corelibs.Basic.Blocks;
using Corelibs.Basic.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Corelibs.Basic.Files
{
    public class LocalStorageFile<TObject> : IFile<TObject>
        where TObject : class
    {
        private readonly string _pathToDirectory;
        private readonly IJsonConverter _jsonConverter;

        public LocalStorageFile(string pathToDirectory, IJsonConverter jsonConverter)
        {
            _pathToDirectory = pathToDirectory;
            _jsonConverter = jsonConverter;
        }

        public LocalStorageFile(string pathToDirectory)
        {
            _pathToDirectory = pathToDirectory;
            _jsonConverter = new NewtonsoftJsonConverter();
        }

        public Task<Result<TObject>> Get()
        {
            if (!Directory.Exists(_pathToDirectory))
                Directory.CreateDirectory(_pathToDirectory);

            if (!Directory.Exists(_pathToDirectory))
                return Result<TObject>.FailureTask();

            string fileName = typeof(TObject).Name;
            string filePath = FileFunctions.CreateFilePath(_pathToDirectory, fileName);

            try
            {
                var fileText = File.ReadAllText(filePath);
                var obj = _jsonConverter.Deserialize<TObject>(fileText);
                return Result<TObject>.SuccessTask(obj);
            }
            catch (Exception ex)
            {
                return Result<TObject>.FailureTask(ex);
            }
        }

        public Task<Result> Save(TObject @object)
        {
            if (@object == null)
                return Result.FailureTask();

            if (!Directory.Exists(_pathToDirectory))
                Directory.CreateDirectory(_pathToDirectory);

            string fileName = typeof(TObject).Name;
            string filePath = FileFunctions.CreateFilePath(_pathToDirectory, fileName);

            var fileData = _jsonConverter.Serialize(@object);

            try
            {
                File.WriteAllText(filePath, fileData);
                return Result.SuccessTask();
            }
            catch (Exception ex)
            {
                return Result.FailureTask(ex);
            }
        }
    }
}
