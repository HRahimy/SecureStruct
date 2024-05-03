using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SecureStruct.Application.Common.Interfaces;

namespace SecureStruct.Infrastructure.Identity;
public class KeycloakAuthorizerService : IAuthorizerService
{
    private readonly KeycloakParams _params;
    public KeycloakAuthorizerService(IOptions<KeycloakParams> options)
    {
        _params = options.Value;
    }

    public async Task<bool> AuthorizeAsync(string authToken, List<string>? permissions = null)
    {
        permissions ??= [];

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", authToken);

            var contentDict = new List<KeyValuePair<string, string>>
            {
                new( "grant_type", "urn:ietf:params:oauth:grant-type:uma-ticket" ),
                new( "audience", _params.ClientId ),
                new( "response_mode", "decision" ),
            };

            foreach (var permission in permissions)
            {
                contentDict.Add(new KeyValuePair<string, string>("permission", permission));
            }

            string url = _params.KeycloakUrl + $"/realms/{_params.Realm}/protocol/openid-connect/token";

            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(contentDict) };
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                // Read and display the response content
                string responseContent = await response.Content.ReadAsStringAsync();
                var parsedContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent) ?? throw new Exception("Failed to parse Authorization response.");
                if (parsedContent["result"] == null) throw new Exception("Could not find `result` property in Authorization response");
                if (parsedContent["result"] is not bool) throw new Exception("Could not find `result` property in Authorization response");

                return (bool)parsedContent["result"];
            }
            else
            {
                return false;
            }
        }
    }
}
