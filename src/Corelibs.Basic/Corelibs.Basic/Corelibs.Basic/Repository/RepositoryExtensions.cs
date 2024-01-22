using Corelibs.Basic.Blocks;
using Corelibs.Basic.Threading;

namespace Corelibs.Basic.Repository
{
    public static class RepositoryExtensions
    {
        public static async Task<T> GetEntity<T, TId>(this IRepository<T, TId> repository, TId id)
        {
            var res = await repository.GetBy(id);
            return res.Get<T>();
        }

        public static async Task<bool> SaveEntity<T, TId>(this IRepository<T, TId> repository, T item)
        {
            var res = await repository.Save(item);
            return res.IsSuccess;
        }

        public static async Task<Result> GetRunAndSaveEntity<T, TId>(this IRepository<T, TId> repository, TId id, Func<T, bool> operation)
        {
            var entity = await repository.GetEntity(id);
            if (!operation(entity))
                return Result.Failure();

            return repository.Save(entity);
        }

        public static async Task<Result<T>> CreateNewAndSaveEntityIfNotExistsOfName<T, TId>(
            this IRepository<T, TId> repository, string name, Func<T, string> getName)
        {
            var result = new Result<T>();

            var existsResult = await repository.ExistsOfName(name, getName);
            if (!existsResult.IsSuccess)
                return result.With(existsResult);

            bool exists = existsResult.Get<bool>();
            if (exists)
                return result;

            string id = Guid.NewGuid().ToString();
            T entity = (T)Activator.CreateInstance(typeof(T), new object[] { id, name });

            var saveResult = await repository.Save(entity);
            if (!saveResult.IsSuccess)
                return result.With(saveResult);

            return result.With(entity);
        }

        public static async Task<Result<T>> GetIfExistsOrCreate<T, TId>(this IRepository<T, TId> repository, TId id, Func<T> createEntity)
        {
            var result = await repository.GetBy(id);
            if (!result.IsSuccess)
                return result;

            T entity = result.Get<T>();
            if (entity != null)
                return result;

            return createEntity().ToResult();
        }

        public static async Task<Result<T>> GetIfExistsOrCreateAndSave<T, TId>(this IRepository<T, TId> repository, TId id, Func<T> createEntity)
        {
            var result = await repository.GetBy(id);
            if (!result.IsSuccess)
                return result;

            T entity = result.Get<T>();
            if (entity != null)
                return result;

            entity = createEntity();
            var saveResult = await repository.Save(entity);
            if (!saveResult.IsSuccess)
                return result.With(saveResult);

            return result.With(entity);
        }

        public static async Task<Result<string>> GetRunAndSaveEntity_ThenCreateNew<T, TId>(
            this IRepository<T, TId> repository, TId id, Func<T, string, bool> operation, Func<string, T> create)
        {
            var entity = await repository.GetEntity(id);

            string newID = Guid.NewGuid().ToString();
            if (!operation(entity, newID))
                return Result<string>.Failure();

            var newEntity = create(newID);
            if (newEntity == null)
                return Result<string>.Failure();

            var saveNewResult = await repository.Save(newEntity);
            if (!saveNewResult.IsSuccess)
                return new Result<string>(saveNewResult);

            var saveOldResult = await repository.Save(entity);
            if (!saveOldResult.IsSuccess)
                return new Result<string>(saveOldResult);

            return Result<string>.Success();
        }

        public static async Task<T> Get<T, TId>(this IRepository<T, TId> repository, TId id) =>
            (await repository.GetBy(id)).Value;

        public static Task<T> Get<T, TId>(this IRepository<T, TId> repository, TId id, Result result) =>
            repository.GetBy(id).Set(result);

        public static Task<T[]> Get<T, TId>(this IRepository<T, TId> repository, IList<TId> ids, Result result) =>
            repository.GetBy(ids).Set(result);

        public static Task<T[]> GetAll<T, TId>(this IRepository<T, TId> repository, Result result) =>
            repository.GetAll().Set(result);

        public static Task Save<T, TId>(this IRepository<T, TId> repository, T entity, Result result) =>
            repository.Save(entity).AddTo(result);

        public static async Task<T> GetIfExistsOrCreate<T, TId>(this IRepository<T, TId> repository, TId id, Func<T> createEntity, Result result)
        {
            var getResult = await repository.GetIfExistsOrCreate(id, createEntity).AddTo(result);
            return getResult.Get();
        }

        public static async Task<T> GetIfExistsOrCreateAndSave<T, TId>(this IRepository<T, TId> repository, TId id, Func<T> createEntity, Result result)
        {
            var getResult = await repository.GetIfExistsOrCreateAndSave(id, createEntity).AddTo(result);
            return getResult.Get();
        }

        public static async Task<Result> Save<T, TId>(this IRepository<T, TId> repository, params T[] entities)
        {
            var results = await Task.WhenAll(entities.Select(e => repository.Save(e)));
            return new Result(results);
        }

        public static async Task<Result> SaveOrBreakIfFail<T, TId>(this IRepository<T, TId> repository, params T[] entities)
        {
            var results = new List<Result>();
            foreach (var entity in entities)
            {
                var result = await repository.Save(entity);
                results.Add(result);

                if (!result.IsSuccess)
                {
                    results.Add(result);
                    break;
                }
            }

            return new Result(results);
        }
    }
}
