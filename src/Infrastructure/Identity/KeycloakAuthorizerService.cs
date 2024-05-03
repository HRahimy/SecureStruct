using System.Net.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using SecureStruct.Application.Common.Interfaces;

namespace SecureStruct.Infrastructure.Identity;
public class KeycloakAuthorizerService : IAuthorizerService
{
    private readonly KeycloakParams _params;
    public KeycloakAuthorizerService(IOptions<KeycloakParams> options)
    {
        _params = options.Value;
    }

    public async Task<bool> AuthorizeAsync(string token, List<string>? permissions = null)
    {
        permissions ??= [];

        using (HttpClient client = new HttpClient())
        {
            token = token.Split(" ")[1];
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var contentDict = new Dictionary<string, string>
            {
                { "grant_type", "urn:ietf:params:oauth:grant-type:uma-ticket" },
                { "audience", _params.ClientId },
                { "response_mode", "decision" }
            };

            string url = _params.KeycloakUrl + $"/realms/{_params.Realm}/protocol/openid-connect/token";
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(contentDict) };

            var requestString = request.ToString();
            var requestContent = await request.Content.ReadAsStringAsync();
            var response = await client.SendAsync(request);


            //var response = await client.PostAsync(
            //    _params.KeycloakUrl + $"/realms/{_params.Realm}/protocol/openid-connect/token",
            //    new FormUrlEncodedContent(contentDict)
            //);

            if (response.IsSuccessStatusCode)
            {
                // Read and display the response content
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response: " + responseContent);
            }
            else
            {
                Console.WriteLine("Error: " + response.StatusCode);
            }
        }

        return false;
    }
}
