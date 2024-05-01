namespace SecureStruct.Infrastructure.Identity;
public class KeycloakParams
{
    public required string KeycloakUrl { get; set; }
    public required string ClientId { get; set; }
    public required string Realm { get; set; }
}
