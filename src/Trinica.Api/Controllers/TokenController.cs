using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Trinica.Api.Controllers;

[Route("api/v1/token")]
[ApiController]
public class TokenController : BaseController
{
    private readonly HttpClient _client;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public TokenController(
        IHttpClientFactory clientFactory,
        IConfiguration configuration)
    {
        _client = clientFactory.CreateClient();

        _clientId = configuration["Authentication:ClientId"];
        _clientSecret = Environment.GetEnvironmentVariable("Trinica-Google-Auth-Secret");
    }

    [HttpPost]
    public async Task<IActionResult> GetAccessToken([FromBody] TokenPostRequest request)
    {
        var response = await _client.PostAsJsonAsync(
            "https://www.googleapis.com/oauth2/v4/token",
            new GoogleApisTokenRequest()
            {
                code = request.Code,
                client_id = _clientId,
                client_secret = _clientSecret,
                grant_type = "authorization_code"
            });

        response.EnsureSuccessStatusCode();

        var jsonResult = await response.Content.ReadAsStringAsync();
        var tokenInfo = JsonSerializer.Deserialize<GoogleApisTokenResponse>(jsonResult);

        var tokenResponse = new TokenPostResponse()
        { 
            AccessToken = tokenInfo.access_token
        };

        return Ok(tokenResponse);
    }

    public class TokenPostRequest
    {
        [Required]
        public required string Code { get; init; }
    }

    class TokenPostResponse
    {
        public required string? AccessToken { get; init; }
    }

    class GoogleApisTokenRequest
    {
        public required string code { get; init; }
        public required string client_id { get; init; }
        public required string client_secret { get; init; }
        public required string grant_type { get; init; }
    }

    class GoogleApisTokenResponse
    {
        public string access_token { get; init; }
        public string scope { get; init; }
        public int expires_in { get; init; }
        public string token_type { get; init; }
    }
}
