namespace SecureStruct.Infrastructure.Identity.Authentication.Flow;
public class ClientCredentialsFlow : AuthenticationFlow
{
    /// <summary>
    /// The client id of the client to authenticate.
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// The client secret of the client to authenticate.
    /// </summary>
    public required string ClientSecret { get; set; }

}
