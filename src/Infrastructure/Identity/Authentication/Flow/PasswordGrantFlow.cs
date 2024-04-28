namespace SecureStruct.Infrastructure.Identity.Authentication.Flow;
public class PasswordGrantFlow : AuthenticationFlow
{
    /// <summary>
    /// Username to authenticate with.
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// Password for the user to authenticate.
    /// </summary>
    public required string Password { get; set; }

}
