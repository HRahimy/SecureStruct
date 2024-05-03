using System.Security.Claims;

namespace SecureStruct.Application.Common.Interfaces;

public interface IUser
{
    string? Id { get; }
    string? UserName { get; }
    string? AccessToken { get; }

    IEnumerable<string>? Roles { get; }
}
