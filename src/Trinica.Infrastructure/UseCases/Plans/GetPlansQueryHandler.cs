//using Corelibs.Basic.Auth;
//using Corelibs.Basic.Blocks;
//using Corelibs.Basic.Repository;
//using Corelibs.MongoDB;
//using Mediator;
//using MongoDB.Bson.Serialization;
//using MongoDB.Driver;
//using Trinica.Entities.Plans;
//using Trinica.Entities.Users;
//using Trinica.UseCases.Plans;
//using System.Security.Claims;

//namespace Trinica.Infrastructure.UseCases.Queries;

//public class GetOwnPlansQueryHandler : IQueryHandler<GetOwnPlansQuery, Result<GetOwnPlansQueryResponse>>
//{
//    private readonly IAccessorAsync<ClaimsPrincipal> _userAccessor;
//    private readonly MongoConnection _mongoConnection;

//    public GetOwnPlansQueryHandler(
//        IAccessorAsync<ClaimsPrincipal> userAccessor,
//        MongoConnection mongoConnection)
//    {
//        _userAccessor = userAccessor;
//        _mongoConnection = mongoConnection;
//    }

//    public async ValueTask<Result<GetOwnPlansQueryResponse>> Handle(GetOwnPlansQuery query, CancellationToken cancellationToken)
//    {
//        var result = Result<GetOwnPlansQueryResponse>.Success();

//        var collection = _mongoConnection.Database.GetCollection<Plan>(Plan.DefaultCollectionName);

//        var userId = await _userAccessor.GetUserID<UserId>();
//        var filter = Builders<Plan>.Filter.Eq(x => x.OwnerId, userId);

//        var projection = Builders<Plan>.Projection
//            .Include(x => x.Id)
//            .Include(x => x.Name);

//        var docs = await collection.Find(filter).Project(projection).ToListAsync();

//        var plans = docs.Select(doc => BsonSerializer.Deserialize<PlanProjection>(doc)).ToArray();
//        var vms = plans.Select(p => new PlanDTO(p.Id.Value, p.Name)).ToArray();

//        return result.With(
//            new GetOwnPlansQueryResponse(vms));
//    }

//    public record PlanProjection(UserId Id, string Name);
//}
