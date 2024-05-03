namespace SecureStruct.Application.Common.Interfaces;
public interface IAuthorizerService
{
    Task<bool> AuthorizeAsync(string token, List<string> permissions);
}
