using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using SecureStruct.Infrastructure.Identity.Authentication.Flow;
using SecureStruct.Infrastructure.Identity.Authentication.Model;

namespace SecureStruct.Infrastructure.Identity.Authentication.Client;
public class PasswordGrantHttpClient : AuthenticatedHttpClient
{
    private KeycloakApiToken? _token;
    private readonly Dictionary<string, string>? _parameters;

    /// <inheritdoc />
    public PasswordGrantHttpClient(PasswordGrantFlow flow)
        : base(flow)
    {
    }

    /// <inheritdoc />
    public PasswordGrantHttpClient(PasswordGrantFlow flow, HttpMessageHandler handler, bool disposeHandler)
        : base(flow, handler, disposeHandler)
    {
        _parameters = new Dictionary<string, string>
            {
                { "client_id", "admin-cli" },
                { "grant_type", "password" },
                { "username", flow.UserName },
                { "password", flow.Password },
            };

        if (!string.IsNullOrWhiteSpace(flow.Scope))
            _parameters.Add("scope", flow.Scope);
    }

    /// <inheritdoc />
    public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await AddAuthorizationHeader(request, cancellationToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task AddAuthorizationHeader(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_token == null || _token.IsExpired)
            _token = await GetToken(cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", _token.AccessToken);
    }

    private async Task<KeycloakApiToken> GetToken(CancellationToken cancellationToken)
    {
        using (var tokenRequest = new HttpRequestMessage(HttpMethod.Post, AuthTokenUrl))
        {
            tokenRequest.Content = new FormUrlEncodedContent(_parameters!);
            using (var response = await base.SendAsync(tokenRequest, cancellationToken))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Username and password authentication failed with code: {response.StatusCode}");

                var tokenJson = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<KeycloakApiToken>(tokenJson, KeycloakJsonSerializerSettings);
                return token!;
            }
        }
    }
}
