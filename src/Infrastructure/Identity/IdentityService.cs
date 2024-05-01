
using FS.Keycloak.RestApiClient.Api;
using FS.Keycloak.RestApiClient.Authentication.Client;
using FS.Keycloak.RestApiClient.ClientFactory;
using FS.Keycloak.RestApiClient.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SecureStruct.Application.Common.Interfaces;
using SecureStruct.Application.Common.Models;

namespace SecureStruct.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly KeycloakParams _keycloakParams;
    private readonly UsersApi _usersApi;
    private readonly RoleMapperApi _roleMapperApi;
    public IdentityService(
        IAuthorizationService authorizationService,
        IOptions<KeycloakParams> optionsAccessor,
        AuthenticationHttpClient authenticationHttpClient
    )
    {
        _keycloakParams = optionsAccessor.Value;
        _authorizationService = authorizationService;

        _usersApi = ApiClientFactory.Create<UsersApi>(authenticationHttpClient);
        _roleMapperApi = ApiClientFactory.Create<RoleMapperApi>(authenticationHttpClient);
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var result = await _usersApi.GetUsersByUserIdAsync(_keycloakParams.Realm, userId);
        return result.Username;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(
        string userName,
        string password
    )
    {
        var user = new UserRepresentation
        {
            Enabled = true,
            Username = userName,
            Credentials =
            [
                new CredentialRepresentation {
                    Type = "password",
                    Value = password,
                    Temporary = false,
                }
            ]
        };
        await _usersApi.PostUsersAsync(_keycloakParams.Realm, user);
        var result = await _usersApi.GetUsersAsync(_keycloakParams.Realm, username: userName);

        if (result.Count == 1)
        {
            return (Result.Success(), result[0].Id);
        }
        else
        {
            throw new Exception("Failed to create user");
        }
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var result = await _roleMapperApi.GetUsersRoleMappingsByUserIdAsync(_keycloakParams.Realm, userId);
        return result.RealmMappings.Any(e => e.Name == role);
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
