namespace OnionWebApi.Infrastructure.Tokens;

public class KeycloakService : ITokenService
{
    private readonly IOptions<KeycloakConfiguration> _options;
    private readonly TokenSettings _tokenSettings;

    public KeycloakService(IOptions<KeycloakConfiguration> options, IOptions<TokenSettings> tokenSettings)
    {
        _options = options;
        _tokenSettings = tokenSettings.Value;
    }

    public async Task<string> GetAccessToken(CancellationToken cancellationToken = default)
    {
        HttpClient client = new();

        string endpoint = $"{_options.Value.HostName}/realms/{_options.Value.Realm}/protocol/openid-connect/token";

        List<KeyValuePair<string, string>> data = new();
        KeyValuePair<string, string> grantType = new("grant_type", "client_credentials");
        KeyValuePair<string, string> clientId = new("client_id", _options.Value.ClientId);
        KeyValuePair<string, string> clientSecret = new("client_secret", _options.Value.ClientSecret);

        data.Add(grantType);
        data.Add(clientId);
        data.Add(clientSecret);

        var result = await PostUrlEncodedFormAsync<GetAccessTokenResponseDto>(endpoint, data, false, cancellationToken);

        return result.AccessToken;
    }
    public async Task<T> PostUrlEncodedFormAsync<T>(string endpoint, List<KeyValuePair<string, string>> data, bool reqToken = false, CancellationToken cancellationToken = default)
    {

        HttpClient httpClient = new();

        if (reqToken)
        {
            string token = await GetAccessToken();

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var message = await httpClient.PostAsync(endpoint, new FormUrlEncodedContent(data), cancellationToken);

        var response = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {
            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResultForBadRequest = System.Text.Json.JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
                throw new Exception(errorResultForBadRequest!.ErrorDescription);
            }
            else if (message.StatusCode == HttpStatusCode.Unauthorized)
            {
                var errorResultForBadRequest = System.Text.Json.JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
                throw new Exception(errorResultForBadRequest!.ErrorDescription);
            }

            var errorResultForOther = System.Text.Json.JsonSerializer.Deserialize<ErrorResponseDto>(response);
            throw new Exception(errorResultForOther!.ErrorMessage);
        }

        if (message.StatusCode == HttpStatusCode.Created || message.StatusCode == HttpStatusCode.NoContent)
        {
            return (default!);
        }

        var obj = System.Text.Json.JsonSerializer.Deserialize<T>(response);

        return (obj!);
    }
    public async Task<T> GetAsync<T>(string endpoint, bool reqToken = false, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (reqToken)
        {
            string token = await GetAccessToken();

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var message = await httpClient.GetAsync(endpoint, cancellationToken);

        var response = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {
            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResultForBadRequest = System.Text.Json.JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
                throw new Exception(errorResultForBadRequest!.ErrorDescription);
            }

            var errorResultForOther = System.Text.Json.JsonSerializer.Deserialize<ErrorResponseDto>(response);
            throw new Exception(errorResultForOther!.ErrorMessage);
        }

        if (message.StatusCode == HttpStatusCode.Created || message.StatusCode == HttpStatusCode.NoContent)
        {
            return (default!);
        }

        var obj = System.Text.Json.JsonSerializer.Deserialize<T>(response);

        return (obj!);
    }
    public async Task<T> PutAsync<T>(string endpoint, object data, bool reqToken = false, CancellationToken cancellationToken = default)
    {
        string stringData = System.Text.Json.JsonSerializer.Serialize(data);
        var content = new StringContent(stringData, Encoding.UTF8, "application/json");

        HttpClient httpClient = new();

        if (reqToken)
        {
            string token = await GetAccessToken();

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var message = await httpClient.PutAsync(endpoint, content, cancellationToken);

        var response = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {
            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResultForBadRequest = System.Text.Json.JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
                throw new Exception(errorResultForBadRequest!.ErrorDescription);
            }

            var errorResultForOther = System.Text.Json.JsonSerializer.Deserialize<ErrorResponseDto>(response);
            throw new Exception(errorResultForOther!.ErrorMessage);
        }

        if (message.StatusCode == HttpStatusCode.Created || message.StatusCode == HttpStatusCode.NoContent)
        {
            return (default!);
        }

        var obj = System.Text.Json.JsonSerializer.Deserialize<T>(response);

        return (obj!);
    }
    public async Task<T> DeleteAsync<T>(string endpoint, bool reqToken = false, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (reqToken)
        {
            string token = await GetAccessToken();

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var message = await httpClient.DeleteAsync(endpoint, cancellationToken);

        var response = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {
            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResultForBadRequest = System.Text.Json.JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
                throw new Exception(errorResultForBadRequest!.ErrorDescription);
            }

            var errorResultForOther = System.Text.Json.JsonSerializer.Deserialize<ErrorResponseDto>(response);
            throw new Exception(errorResultForOther!.ErrorMessage);
        }

        if (message.StatusCode == HttpStatusCode.Created || message.StatusCode == HttpStatusCode.NoContent)
        {
            return (default!);
        }

        var obj = System.Text.Json.JsonSerializer.Deserialize<T>(response);

        return (obj!);
    }
    public async Task<T> DeleteAsync<T>(string endpoint, object data, bool reqToken = false, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (reqToken)
        {
            string token = await GetAccessToken();

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);

        string str = System.Text.Json.JsonSerializer.Serialize(data);
        request.Content = new StringContent(str, Encoding.UTF8, "application/json");

        var message = await httpClient.SendAsync(request, cancellationToken);

        var response = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {
            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResultForBadRequest = System.Text.Json.JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
                throw new Exception(errorResultForBadRequest!.ErrorDescription);
            }

            var errorResultForOther = System.Text.Json.JsonSerializer.Deserialize<ErrorResponseDto>(response);
            throw new Exception(errorResultForOther!.ErrorMessage);
        }

        if (message.StatusCode == HttpStatusCode.Created || message.StatusCode == HttpStatusCode.NoContent)
        {
            return (default!);
        }

        var obj = System.Text.Json.JsonSerializer.Deserialize<T>(response);

        return (obj!);
    }
    public async Task<T> PostAsync<T>(string endpoint, object data, bool reqToken = false, CancellationToken cancellationToken = default)
    {
        string stringData = System.Text.Json.JsonSerializer.Serialize(data);
        var content = new StringContent(stringData, Encoding.UTF8, "application/json");

        HttpClient httpClient = new();

        if (reqToken)
        {
            string token = await GetAccessToken();

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var message = await httpClient.PostAsync(endpoint, content, cancellationToken);

        var response = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {
            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResultForBadRequest = System.Text.Json.JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
                throw new Exception(errorResultForBadRequest!.ErrorDescription);
            }

            var errorResultForOther = System.Text.Json.JsonSerializer.Deserialize<ErrorResponseDto>(response);
            throw new Exception(errorResultForOther!.ErrorMessage);
        }

        if (message.StatusCode == HttpStatusCode.Created || message.StatusCode == HttpStatusCode.NoContent)
        {
            return (default!);
        }

        var obj = System.Text.Json.JsonSerializer.Deserialize<T>(response);

        return (obj!);
    }
    public async Task<JwtSecurityToken> CreateTokenAsync(AppUser user, string password, IList<string> roles, CancellationToken cancellationToken = default)
    {
        string endpoint = $"{_options.Value.HostName}/realms/{_options.Value.Realm}/protocol/openid-connect/token";

        List<KeyValuePair<string, string>> data = new();
        KeyValuePair<string, string> grantType = new("grant_type", "password");
        KeyValuePair<string, string> clientId = new("client_id", _options.Value.ClientId);
        KeyValuePair<string, string> clientSecret = new("client_secret", _options.Value.ClientSecret);
        KeyValuePair<string, string> username = new("username", user.UserName!);
        KeyValuePair<string, string> passwordKey = new("password", password);

        data.Add(grantType);
        data.Add(clientId);
        data.Add(clientSecret);
        data.Add(username);
        data.Add(passwordKey);

        var response = await PostUrlEncodedFormAsync<GetAccessTokenResponseDto>(endpoint, data, false, cancellationToken = default);

        var token = new JwtSecurityToken(
            issuer: _tokenSettings.Issuer,
            audience: _tokenSettings.Audience,
            expires: DateTime.Now.AddMinutes(_tokenSettings.TokenValidityInMunitues),
            claims: null,
            signingCredentials: null
            );
      


        return token;
    }
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        TokenValidationParameters tokenValidationParamaters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Secret)),
            ValidateLifetime = false
        };

        JwtSecurityTokenHandler tokenHandler = new();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParamaters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg
            .Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Token bulunamadı.");

        return principal;
    }
}
