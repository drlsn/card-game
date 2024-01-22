namespace Corelibs.Basic.UseCases.DTOs;

public record IdentityTypedDTO(string Id, string Name, string Type) : IdentityDTO(Id, Name);
