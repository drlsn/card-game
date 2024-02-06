using Corelibs.Basic.Blocks;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Trinica.Api.Extensions
{
    public static class MediatorRequestToObjectResultExtensions
    {
        public static Task<IActionResult> SendAndGetResponse<TReturnValue, TQuery>(this IMediator mediator)
            where TQuery : IQuery<Result<TReturnValue>>, new()
            where TReturnValue : class =>
            mediator.Send(new TQuery()).ToApiResult().GetQueryResponse();

        public static Task<IActionResult> SendAndGetResponse<TReturnValue>(this IMediator mediator, IQuery<Result<TReturnValue>> query)
            where TReturnValue : class =>
            mediator.Send(query).ToApiResult().GetQueryResponse();

        public static Task<IActionResult> SendAndGetPostResponse<TCommand>(this IMediator mediator) where TCommand : ICommand<Result>, new() =>
            mediator.Send(new TCommand()).ToApiResult().GetPostCommandResponse();

        public static Task<IActionResult> SendAndGetPostResponse(this IMediator mediator, ICommand<Result> command) =>
            mediator.Send(command).ToApiResult().GetPostCommandResponse();

        public static Task<IActionResult> SendAndGetPatchResponse(this IMediator mediator, ICommand<Result> command) =>
            mediator.Send(command).ToApiResult().GetPatchCommandResponse();

        public static Task<IActionResult> SendAndGetPutResponse(this IMediator mediator, ICommand<Result> command) =>
            mediator.Send(command).ToApiResult().GetPutCommandResponse();

        public static Task<IActionResult> SendAndGetDeleteResponse(this IMediator mediator, ICommand<Result> command) =>
            mediator.Send(command).ToApiResult().GetDeleteCommandResponse();
    }
}
