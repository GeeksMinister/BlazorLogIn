using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BlazorLogIn.Authentication;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly IConfiguration _config;
    private readonly HttpClient _client;
    private readonly AuthenticationState _anonymous;

    public AuthStateProvider(HttpClient client, ILocalStorageService ILocalStorage, IConfiguration config)
    {
        _localStorage = ILocalStorage;
        _config = config;
        _client = client;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(_config["authTokenStorageKey"]);

        if (string.IsNullOrWhiteSpace(token))
        {
            return _anonymous;
        }
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

        return 
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(
                JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));
    }

    public void NotifyUserAuthentication(string token)
    {
        var authenticatedUser = new ClaimsPrincipal(
            new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

        //Built-in event
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(_anonymous);
        NotifyAuthenticationStateChanged(authState);
    }
}

