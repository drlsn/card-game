using Corelibs.Basic.DDD;
using Corelibs.Basic.Repository;
using Corelibs.Basic.UseCases;
using Corelibs.MongoDB;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mediator;
using System.Reflection;
using System.Security.Claims;
using Trinica.Api.Authorization;
using Trinica.Entities.Gameplay;
using Trinica.Entities.Gameplay.Events;
using Trinica.Entities.Users;
using Trinica.Infrastructure.UseCases.Gameplay;
using Trinica.UseCases.Gameplay;

namespace Trinica.Api;

public static class Startup
{
    public static void InitializeApp(this IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddScoped<IAccessorAsync<ClaimsPrincipal>, CurrentUserAccessor>();
        
        var entitiesAssembly = typeof(Entities.Users.User).Assembly;
        var useCasesAssembly = typeof(UseCases.Users.CreateUserCommand).Assembly;

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(useCasesAssembly);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MongoDbCommandTransactionBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MongoDbQueryBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PessimisticGameLockBehaviour<,>));

        services.AddScoped<IQueryExecutor, MediatorQueryExecutor>();
        services.AddScoped<ICommandExecutor, MediatorCommandExecutor>();

        services.AddMediator(opts => opts.ServiceLifetime = ServiceLifetime.Scoped);
        services.AddRepositories(environment, entitiesAssembly);

        var botHub = new BotHub(_userMemoryRepository);
        services.AddSingleton<IBotHub>(sp => botHub);
        services.AddScoped<IBotHub>(sp => botHub);
        services.AddScoped<BotHub>(sp => botHub);
        services.AddHostedService(sp => 
            new BotHubWorker(
                sp.GetRequiredService<IServiceScopeFactory>()));

        services.AddSingleton<IEventsDispatcher<GameId, UserId, GameEvent>>(
            new RoomEventsDispatcher<GameId, UserId, GameEvent>(
                getRoomIdValue: id => id.Value,
                getUserIdValue: id => id.Value,
                toRoomId: value => new(value),
                toUserId: value => new(value),
                getRoomId: ev => ev.GameId,
                getUserId: ev => ev.PlayerId));
    }

    public static void AddRepositories(this IServiceCollection services, IWebHostEnvironment environment, Assembly assembly)
    {
        var mongoConnectionString = Environment.GetEnvironmentVariable("TrinicaDatabaseConn");
        var databaseName = environment.IsDevelopment() ? "Trinica_dev" : "Trinica_prod";

        MongoConventionPackExtensions.AddIgnoreConventionPack();

        services.AddUserRepository();
        services.AddSingleton<IRepository<Game, GameId>, MemoryRepository<Game, GameId>>();
        services.AddMongoRepositories(assembly, mongoConnectionString, databaseName);
    }

    private static MemoryRepository<User, UserId> _userMemoryRepository;
    private static void AddUserRepository(this IServiceCollection services)
    {
        _userMemoryRepository = new MemoryRepository<User, UserId>();

        services.AddScoped<IRepository<User, UserId>>(sp =>
        {
            var mongoRepository = new MongoDbRepository<User, UserId>(
                sp.GetRequiredService<MongoConnection>(), User.DefaultCollectionName);

            var memoryRepositoryDecorator = new MemoryRepositoryDecorator<User, UserId>(
                _userMemoryRepository, mongoRepository);

            return memoryRepositoryDecorator;
        });

        // Just for bots
        services.AddScoped<IMemoryRepository<User, UserId>>(sp => _userMemoryRepository);
    }
}
