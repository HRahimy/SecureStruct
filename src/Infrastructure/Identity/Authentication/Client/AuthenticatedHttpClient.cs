using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SecureStruct.Infrastructure.Identity.Authentication.Flow;

namespace SecureStruct.Infrastructure.Identity.Authentication.Client;
public abstract class AuthenticatedHttpClient : HttpClient
{
    /// <summary>
    /// JSON serializer settings for Keycloak API.
    /// </summary>
    protected static readonly JsonSerializerSettings KeycloakJsonSerializerSettings = new JsonSerializerSettings
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        }
    };

    /// <summary>
    /// Base URL to keycloak server, e.g. https://keycloak.example.com:8443/
    /// </summary>
    public string KeycloakUrl { get; }

    /// <summary>
    /// URL to keycloak server's token endpoint
    /// </summary>
    public string AuthTokenUrl { get; }


    /// <summary>
    /// Creates a HttpClient authenticated against a Keycloak server.
    /// </summary>
    /// <param name="flow">Data used for authentication.</param>
    protected AuthenticatedHttpClient(AuthenticationFlow flow)
        : this(flow, new HttpClientHandler(), true) { }


    /// <summary>
    /// Creates a HttpClient authenticated against a Keycloak server.
    /// </summary>
    /// <param name="flow">Data used for authentication.</param>
    /// <param name="handler">The <see cref="HttpMessageHandler" /> responsible for processing the HTTP response messages.</param>
    /// <param name="disposeHandler"><see langword="true" /> if the inner handler should be disposed of by HttpClient.Dispose; <see langword="false" /> if you intend to reuse the inner handler.</param>
    protected AuthenticatedHttpClient(AuthenticationFlow flow, HttpMessageHandler handler, bool disposeHandler)
        : base(handler ?? throw new ArgumentNullException(nameof(handler)), disposeHandler)
    {
        if (flow == null)
            throw new ArgumentNullException(nameof(flow));

        KeycloakUrl = flow.KeycloakUrl;
        AuthTokenUrl = $"{KeycloakUrl}/realms/{flow.Realm}/protocol/openid-connect/token";
    }
}
