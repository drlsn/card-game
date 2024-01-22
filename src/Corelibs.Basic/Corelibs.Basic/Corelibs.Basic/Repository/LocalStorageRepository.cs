//using Corelibs.Basic.Blocks;
//using Corelibs.Basic.DDD;
//using Corelibs.Basic.Files;
//using Corelibs.Basic.Json;
//using Corelibs.Basic.Threading;

//namespace Corelibs.Basic.Repository
//{
//    public class LocalStorageRepository<TEntity, TEntityId> : IRepository<TEntity, TEntityId>
//        where TEntity : IEntity<TEntityId>
//        where TEntityId : EntityId
//    {
//        private readonly string _pathToDirectory;
//        private readonly IDirectoryOperations _directoryOperations;
//        private readonly IFileOperations _fileOperations;
//        private readonly IJsonConverter _jsonConverter;

//        public LocalStorageRepository(
//            string pathToDirectory)
//        {
//            _pathToDirectory = pathToDirectory;
//            _directoryOperations = new DirectoryOperations();
//            _fileOperations = new FileOperations();
//            _jsonConverter = new NewtonsoftJsonConverter();
//        }

//        public Task<Result<TEntity>> GetBy(string id)
//        {
//            if (string.IsNullOrWhiteSpace(id))
//                return Result<TEntity>.FailureTask();

//            if (!_directoryOperations.Exists(_pathToDirectory))
//                _directoryOperations.Create(_pathToDirectory);

//            if (!Directory.Exists(_pathToDirectory))
//                return Result<TEntity>.FailureTask();

//            string filePath = FileFunctions.CreateFilePath(_pathToDirectory, id);

//            try
//            {
//                var fileText = _fileOperations.ReadAsText(filePath);
//                var obj = _jsonConverter.Deserialize<TEntity>(fileText);
//                return Result<TEntity>.SuccessTask(obj);
//            }
//            catch (Exception ex)
//            {
//                if (ex is FileNotFoundException)
//                    return Result<TEntity>.SuccessTask();

//                return Result<TEntity>.FailureTask(ex);
//            }
//        }

//        public async Task<Result<TEntity>> GetOfName(string name, Func<TEntity, string> getName)
//        {
//            var result = Result<TEntity>.Success();

//            var repository = this as IRepository<TEntity, TEntityId>;
//            var getAllResult = await repository.GetAll();
//            if (!getAllResult.IsSuccess)
//                return result.With(getAllResult);

//            TEntity[] entities = getAllResult.Get();
//            var entityOfName = entities.FirstOrDefault(e =>
//            {
//                var entityName = getName(e);
//                return entityName == name;
//            });

//            return result.With(entityOfName);
//        }

//        public Task<Result<TEntity[]>> GetAll()
//        {
//            if (!_directoryOperations.Exists(_pathToDirectory))
//                _directoryOperations.Create(_pathToDirectory);

//            string[] filePaths = _directoryOperations.GetFiles(_pathToDirectory);
//            if (filePaths == null || filePaths.Length == 0)
//                return Array.Empty<TEntity>().ToResultTask();

//            var fileTexts = filePaths.Select(fp => _fileOperations.ReadAsText(fp));
//            var entities = fileTexts.Select(ft => _jsonConverter.Deserialize<TEntity>(ft)).ToArray();

//            return Result<TEntity[]>.SuccessTask(entities);
//        }

//        public Task<Result<TEntity[]>> GetAll(Action<int> setProgress, CancellationToken ct)
//        {
//            if (!_directoryOperations.Exists(_pathToDirectory))
//                _directoryOperations.Create(_pathToDirectory);

//            string[] filePaths = _directoryOperations.GetFiles(_pathToDirectory);
//            if (filePaths == null || filePaths.Length == 0)
//                return Array.Empty<TEntity>().ToResultTask();

//            ct.CheckCancellation();

//            var entities = new List<TEntity>(filePaths.Length);
//            for (int i = 0; i < filePaths.Length; i++)
//            {
//                ct.CheckCancellationAndSetProgress(i, filePaths, setProgress);

//                var filePath = filePaths[i];
//                var fileText = _fileOperations.ReadAsText(filePath);
//                var entity = _jsonConverter.Deserialize<TEntity>(fileText);

//                entities.Add(entity);
//            }

//            return Result<TEntity[]>.SuccessTask(entities.ToArray());
//        }

//        public Task<Result> Save(TEntity item)
//        {
//            if (string.IsNullOrWhiteSpace(item.Id))
//                return Result.FailureTask();

//            if (item == null)
//                return Result.FailureTask();

//            if (!_directoryOperations.Exists(_pathToDirectory))
//                _directoryOperations.Create(_pathToDirectory);

//            string filePath = FileFunctions.CreateFilePath(_pathToDirectory, item.ID);

//            item.Version++;
//            var fileData = _jsonConverter.Serialize(item);

//            try
//            {
//                _fileOperations.WriteAsText(filePath, fileData);
//                return Result.SuccessTask();
//            }
//            catch (Exception ex)
//            {
//                return Result.FailureTask(ex);
//            }
//        }

//        public Task<Result> Clear()
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<Result<bool>> ExistsOfName(string name, Func<TEntity, string> getName)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<Result> Delete(string id)
//        {
//            if (string.IsNullOrWhiteSpace(id))
//                return Result.FailureTask();

//            if (!_directoryOperations.Exists(_pathToDirectory))
//                _directoryOperations.Create(_pathToDirectory);

//            string filePath = FileFunctions.CreateFilePath(_pathToDirectory, id);
//            try
//            {
//                _fileOperations.Delete(filePath);
//                return Result.SuccessTask();
//            }
//            catch (Exception ex)
//            {
//                return Result.FailureTask(ex);
//            }
//        }

//        public Task<Result<TEntity[]>> GetBy(IList<string> ids)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
