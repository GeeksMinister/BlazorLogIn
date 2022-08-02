using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BlazorLogIn.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _client;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;
    private readonly IConfiguration _config;

    public AuthenticationService(HttpClient client,
                                 AuthenticationStateProvider authStateProvider,
                                 ILocalStorageService localStorage,
                                 IConfiguration config)
    {
        _client = client;
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
        _config = config;
    }

    public async Task<AuthenticatedUserModel> Login(AuthenticationUserModel userForAuthentication)
    {
        var data = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("id", userForAuthentication.Id),
            new KeyValuePair<string, string>("password", userForAuthentication.Password)
        });
        string location = _config["ApiLocation"];
        var authResult = await _client.PostAsync(location, data);
        var authContent = await authResult.Content.ReadAsStringAsync();

        if (authResult.IsSuccessStatusCode == false)
        {
            return null!;
        }

        var result = JsonSerializer.Deserialize<AuthenticatedUserModel>(authContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        await _localStorage.SetItemAsync(_config["authTokenStorageKey"], result.Access_Token);

        ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Access_Token);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Access_Token);
        return result;
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync(_config["authTokenStorageKey"]);
        ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
        _client.DefaultRequestHeaders.Authorization = null;
    }

}

