using System.Security.Claims;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using SecureStruct.Application.Common.Interfaces;

namespace SecureStruct.Web.Services;

class RealmAccessClaim
{
    public List<string>? roles { get; set; }
}

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("preferred_username");

    public string? AccessToken =>
        _httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Authorization];

    public IEnumerable<string>? Roles
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("realm_access");
            if (claim == null) return null;

            return JsonConvert.DeserializeObject<RealmAccessClaim>(claim)?.roles;
        }
    }
}
