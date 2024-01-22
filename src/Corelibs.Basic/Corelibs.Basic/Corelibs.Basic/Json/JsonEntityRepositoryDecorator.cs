using Corelibs.Basic.Blocks;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Functional;
using Corelibs.Basic.Repository;

namespace Corelibs.Basic.Json
{
    public class JsonEntityRepositoryDecorator<TEntity, TEntityId, TDataEntity> : IRepository<TEntity, TEntityId>, IReadRepository<TEntityId>
        where TEntity : class, IEntity<TEntityId>
        where TDataEntity : JsonEntity<TEntityId>, new()
        where TEntityId : EntityId
    {
        private readonly IJsonConverter _jsonConverter;
        private readonly IRepository<TDataEntity, TEntityId> _jsonTableRepository;

        public JsonEntityRepositoryDecorator(
            IRepository<TDataEntity, TEntityId> jsonTableRepository)
        {
            _jsonConverter = new NewtonsoftJsonConverter();
            _jsonTableRepository = jsonTableRepository;
        }

        public Task<Result> Clear()
        {
            throw new NotImplementedException(); // to delete?
        }

        public Task<Result> Create(IEnumerable<TEntity> items)
        {
            throw new NotImplementedException();
        }

        public Task<Result> Create(TEntity item)
        {
            throw new NotImplementedException();
        }

        public Task<Result> Delete(TEntityId id)
        {
            return _jsonTableRepository.Delete(id);
        }

        public Task<Result<bool>> ExistsOfName(string name, Func<TEntity, string> getName)
        {
            throw new NotImplementedException(); // to delete?
        }

        public async Task<Result<TEntity[]>> GetAll()
        {
            var result = Result<TEntity[]>.Success();

            var jsonEntitiesResult = await _jsonTableRepository.GetAll();
            if (!jsonEntitiesResult.ValidateSuccessAndValues())
                return result.Fail().With(jsonEntitiesResult);

            var jsonEntities = jsonEntitiesResult.Get();
            var entities = jsonEntities.Select(e => _jsonConverter.Deserialize<TEntity>(e.Content)).ToArray();
            return Result<TEntity[]>.Success(entities);
        }

        public Task<Result<TEntity[]>> GetAll(Action<int> setProgress, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<TEntity>> GetBy(TEntityId id)
        {
            var jsonEntityResult = await _jsonTableRepository.GetBy(id);
            if (!jsonEntityResult.IsSuccess)
                return Result<TEntity>.Failure().With(jsonEntityResult);

            var jsonEntity = jsonEntityResult.Get();
            if (jsonEntity.IsNull())
                return Result<TEntity>.Success();

            var entity = _jsonConverter.Deserialize<TEntity>(jsonEntity.Content);

            return Result<TEntity>.Success(entity);
        }

        public async Task GetBy(TEntityId id, Result result)
        {
            var resultLocal = await GetBy(id);
            result.Add(resultLocal);
        }

        public async Task<Result<TEntity[]>> GetBy(IList<TEntityId> ids)
        {
            var result = Result<TEntity[]>.Success();

            var jsonEntitiesResult = await _jsonTableRepository.GetBy(ids);
            if (!jsonEntitiesResult.ValidateSuccessAndValues())
                return result.Fail().With(jsonEntitiesResult);

            var jsonEntities = jsonEntitiesResult.Get();
            var entities = jsonEntities.Select(e => _jsonConverter.Deserialize<TEntity>(e.Content)).ToArray();
            return Result<TEntity[]>.Success(entities);
        }

        public Task<Result<TEntity>> GetOfName(string name, Func<TEntity, string> getName)
        {
            throw new NotImplementedException(); // to delete?
        }

        public async Task<Result> Save(TEntity item)
        {
            var result = Result.Success();

            var json = _jsonConverter.Serialize(item);

            var entity = await _jsonTableRepository.Get(item.Id, result);
            if (entity != null)
            {
                entity.Content = json;
                entity.Version = item.Version++;
            }
            else
            {
                entity = new TDataEntity()
                {
                    Id = item.Id,
                    Content = json
                };
            }

            return _jsonTableRepository.Save(entity);
        }
    }
}
