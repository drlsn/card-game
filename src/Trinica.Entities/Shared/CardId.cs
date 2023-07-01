using Corelibs.Basic.DDD;

namespace Trinica.Entities.Shared;

public record CardId(string Value) : EntityId(Value) { }
