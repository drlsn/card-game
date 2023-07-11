//using Corelibs.Basic.Auth;
//using Corelibs.Basic.Blocks;
//using Corelibs.Basic.Repository;
//using Corelibs.MongoDB;
//using Mediator;
//using MongoDB.Bson.Serialization;
//using MongoDB.Driver;
//using Trinica.Entities.Plans;
//using Trinica.Entities.Users;
//using Trinica.Entities.Shared;
//using Trinica.UseCases.Plans;
//using System.Security.Claims;
//using Trinica.Entities.Sessions;
//using MongoDB.Bson;

//namespace Trinica.Infrastructure.UseCases.Queries;

//public class GetPlanDetailsQueryHandlerQueryHandler : IQueryHandler<GetPlanDetailsQuery, Result<GetPlanDetailsQueryResponse>>
//{
//    private readonly IAccessorAsync<ClaimsPrincipal> _userAccessor;
//    private readonly MongoConnection _mongoConnection;

//    public GetPlanDetailsQueryHandlerQueryHandler(
//        IAccessorAsync<ClaimsPrincipal> userAccessor,
//        MongoConnection mongoConnection)
//    {
//        _userAccessor = userAccessor;
//        _mongoConnection = mongoConnection;
//    }

//    public async ValueTask<Result<GetPlanDetailsQueryResponse>> Handle(GetPlanDetailsQuery query, CancellationToken cancellationToken)
//    {
//        var result = Result<GetPlanDetailsQueryResponse>.Success();

//        var plansCollection = _mongoConnection.Database.GetCollection<Plan>(Plan.DefaultCollectionName);
//        var sessionsCollection = _mongoConnection.Database.GetCollection<Session>(Session.DefaultCollectionName);

//        var userId = await _userAccessor.GetUserID<UserId>();

//        var filter = Builders<Plan>.Filter.Eq(x => x.Id, query.PlanId);

//        var projection = Builders<Plan>.Projection
//            .Include(x => x.Id)
//            .Include(x => x.Name)
//            .Include(x => x.OwnerId)
//            .Include(x => x.Activities);

//        var docs = await plansCollection.Find(filter).Project(projection).ToListAsync();

//        var plans = docs.Select(doc => BsonSerializer.Deserialize<PlanProjection>(doc)).ToArray();
//        //var vms = plans.Select(
//        //    p => new PlanDetailsDTO(
//        //        p.Id.ToString(), 
//        //        p.Name,
//        //        Author: p.OwnerId == userId ? "Own" : "Public",
//        //        SessionsInfo: new(
//        //            p.Activities.Count,
//        //            p.Activities.Where(a => a.Type is ActivityType.Session).ToArray(),
//        //            p.Activities.Where(a => a.Type is ActivityType.Session).ToArray(),
//        //            p.Activities.Where(a => a.Type is ActivityType.Rest).ToArray(),
//        //            p.Activities.Select(a =>
//        //            {
//        //                var filter = Builders<Session>.Filter.Eq(x => x.Id, a.SessionId);
//        //                var projection = Builders<Session>.Projection.Include(x => x.Name);
//        //                var docs = await sessionsCollection.Find(filter).Project(projection).ToListAsync();
//        //                projection.ToBsonDocument().
//        //                return new SessionDTO(a.SessionId.ToString(), a.Type);
//        //            }).ToArray()
//        //    ).ToArray();

//        //var response = new GetPlanDetailsQueryResponse(
//        //    new()
//        //    {
                
//        //    });

//        return result.With(null);
//    }

//    public record PlanProjection(UserId Id, string Name, UserId OwnerId, Activity[] Activities);
//    public record SessionProjection(string Name);
//}
