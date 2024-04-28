using Microsoft.AspNetCore.Authorization;
using SecureStruct.Application.Common.Interfaces;
using SecureStruct.Application.Common.Models;
using SecureStruct.Infrastructure.Identity.Authentication.Client;

namespace SecureStruct.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly AuthenticatedHttpClient _httpClient;

    public IdentityService(
        IAuthorizationService authorizationService,
        AuthenticatedHttpClient httpClient
    )
    {
        _authorizationService = authorizationService;
        _httpClient = httpClient;
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var result = await _httpClient.SendAsync(new HttpRequestMessage(
                HttpMethod.Get, _httpClient.KeycloakUrl + $"/admin/realms/master/users/{userId}"
                ), CancellationToken.None
            );
        var resultString = await result.Content.ReadAsStringAsync();
        throw new NotImplementedException();
    }

    public Task<(Result Result, string UserId)> CreateUserAsync(
        string userName,
        string password
    )
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsInRoleAsync(string userId, string role)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteUserAsync(string userId)
    {
        throw new NotImplementedException();
    }
}
