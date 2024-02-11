using Corelibs.Basic.Collections;
using System.Text.Json.Serialization;

namespace Trinica.Api.Contracts;

public class ApiResult
{
    public static ApiResult Success() => new ApiResult();
    public static ApiResult Failure(string message) => new ApiResult(message);

    protected readonly List<Message> _messages = new();

    public bool IsSuccess => !_messages.ContainsErrors();

    public Message[] Messages => _messages.ToArray();
    [JsonIgnore]
    public string Message => _messages.Select(m => m.Content).AggregateOrEmpty((x, y) => $"{x}. {y}", string.Empty);

    public ApiResult(List<Message> messages)
    {
        _messages = messages;
    }
    
    protected ApiResult(string message = null)
    {
        if (message is not null)
            Fail(message);
    }

    public ApiResult Fail(string errorMessage)
    {
        _messages.Add(new Message(MessageLevel.Error, errorMessage));
        return this;
    }
}

public class ApiResult<T> : ApiResult
{
    public static new ApiResult<T> Success() => new ApiResult<T>();
    public static ApiResult<T> Success(T value) => new ApiResult<T>(value);
    public static new ApiResult<T> Failure(string message) => new ApiResult<T>(message);

    public ApiResult(T value = default) => Value = value;
    public ApiResult(string message) : base(message) {}
    public ApiResult(List<Message> messages) : base(messages) {}

    public T Value { get; private set; }

    public new ApiResult<T> Fail(string message)
    {
        _messages.Add(new Message(MessageLevel.Error, message));
        return this;
    }

    public new ApiResult<T> Fail(IEnumerable<Message> messages)
    {
        _messages.AddRange(messages);
        return this;
    }

    public new ApiResult<T> With(T value)
    {
        Value = value;
        return this;
    }

    public static implicit operator ApiResult<T>(string errorMessage) => new(errorMessage);
}

public record Message(MessageLevel Level, string Content);
public record MessageLevel(string name)
{
    public static readonly MessageLevel Info = new("info");
    public static readonly MessageLevel Warning = new("warning");
    public static readonly MessageLevel Error = new("error");
}

public static class MessageExtensions
{
    public static bool ContainsErrors(this IEnumerable<Message> messages) =>
        messages.Any(m => m.Level == MessageLevel.Error);
}