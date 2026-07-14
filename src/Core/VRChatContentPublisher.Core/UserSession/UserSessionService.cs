using System.Net;
using MessagePipe;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VRChatContentPublisher.Core.Events.UserSession;
using VRChatContentPublisher.Core.Shared;
using VRChatContentPublisher.VRChatApi;
using VRChatContentPublisher.VRChatApi.ApiClient;
using VRChatContentPublisher.VRChatApi.Exceptions;
using VRChatContentPublisher.VRChatApi.Models;
using VRChatContentPublisher.VRChatApi.Models.Rest.Auth;

namespace VRChatContentPublisher.Core.UserSession;

public sealed class UserSessionService : IAsyncDisposable, IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserSessionService> _logger;

    // Cookies, UserId, UserName
    private readonly SaveCookiesDelegate _saveFunc;
    private readonly SemaphoreSlim _saveSemaphore = new(1, 1);

    private readonly HttpClient _sessionHttpClient;
    private readonly VRChatApiClient _apiClient;
    private readonly IPublisher<SessionStateChangedEvent> _sessionStateChangedPublisher;

    public CookieContainer CookieContainer { get; }

    public event EventHandler<UserSessionState>? StateChanged;
    public event EventHandler<CurrentUser?>? CurrentUserUpdated;
    public UserSessionState State { get; set; } = UserSessionState.Pending;
    public bool IsScopeInitialized => _sessionScope is not null;

    private readonly SemaphoreSlim _createOrGetScopeLock = new(1, 1);

    public string UserNameOrEmail { get; private set; }
    public string? UserId { get; private set; }
    public CurrentUser? CurrentUser { get; private set; }

    private AsyncServiceScope? _sessionScope;

    internal UserSessionService(
        string userNameOrEmail,
        string? userId,
        CurrentUser? user,
        SaveCookiesDelegate saveFunc,
        CookieContainer? cookieContainer,
        VRChatApiClientFactory apiClientFactory,
        IPublisher<SessionStateChangedEvent> sessionStateChangedPublisher,
        IServiceScopeFactory scopeFactory,
        UserSessionHttpClientFactory httpClientFactory,
        ILogger<UserSessionService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _saveFunc = saveFunc;
        _sessionStateChangedPublisher = sessionStateChangedPublisher;
        UserId = userId;
        CurrentUser = user;

        UserNameOrEmail = userNameOrEmail;

        CookieContainer = cookieContainer ?? new CookieContainer();

        if (cookieContainer is null || userId is null)
        {
            OnStateChanged(UserSessionState.LoggedOut);
        }

        _sessionHttpClient = httpClientFactory.Create(
            CookieContainer,
            userId ?? userNameOrEmail,
            logger,
            AfterHttpResponseAsync
        );

        _apiClient = apiClientFactory.Create(_sessionHttpClient);
    }

    public HttpClient GetHttpClient() => _sessionHttpClient;
    public VRChatApiClient GetApiClient() => _apiClient;

    public async ValueTask<LoginResult> LoginAsync(string password)
    {
        var result = await _apiClient.LoginAsync(UserNameOrEmail, password);
        OnStateChanged(result.IsSuccess ? UserSessionState.LoggedIn : UserSessionState.LoggedOut);

        return result;
    }

    public async ValueTask LogoutAsync()
    {
        OnStateChanged(UserSessionState.LoggedOut);
        await _apiClient.LogoutAsync();
    }

    public async ValueTask<bool> TryRepairAsync()
    {
        try
        {
            await GetCurrentUserAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to auto repair (check is session really invalid) user session for {UserNameOrEmail}",
                UserNameOrEmail);
            return false;
        }

        return true;
    }

    public async ValueTask<CurrentUser> GetCurrentUserAsync()
    {
        try
        {
            CurrentUser = await _apiClient.GetCurrentUser();
            UserId = CurrentUser.Id;
            UserNameOrEmail = CurrentUser.UserName;
        }
        catch (ApiErrorException ex) when (ex.StatusCode == 401)
        {
            OnStateChanged(UserSessionState.InvalidSession);
            throw;
        }

        await SaveSessionAsync();
        OnStateChanged(UserSessionState.LoggedIn);

        CurrentUserUpdated?.Invoke(this, CurrentUser);
        return CurrentUser;
    }

    public AsyncServiceScope? TryGetSessionScope()
    {
        return _sessionScope;
    }

    public async ValueTask<AsyncServiceScope> CreateOrGetSessionScopeAsync()
    {
        using (await SimpleSemaphoreSlimLockScope.WaitAsync(_createOrGetScopeLock))
        {
            if (_sessionScope is { } scope)
                return scope;

            try
            {
                CurrentUser = await GetCurrentUserAsync();
            }
            catch (Exception ex)
            {
                if (CurrentUser is null)
                {
                    // Fetch failed and no cache available
                    _logger.LogWarning(
                        ex,
                        "Failed to create session scope for {UserNameOrEmail} because fetching current user failed and no cache available",
                        UserNameOrEmail);
                    throw new InvalidOperationException("Failed to create session scope because fetching current user failed and no cache available", ex);
                }

                _logger.LogWarning(
                    ex,
                    "Failed to fetch current user for {UserNameOrEmail} while creating session scope, but cache is available, so continue creating session scope",
                    UserNameOrEmail);
            }

            return await CreateSessionScopeAsyncCore();
        }
    }

    public void DebugForceEnterInvalidState()
    {
        OnStateChanged(UserSessionState.InvalidSession);
    }

    private ValueTask<AsyncServiceScope> CreateSessionScopeAsyncCore()
    {
        var scope = _scopeFactory.CreateAsyncScope();
        var sessionScopeService = scope.ServiceProvider.GetRequiredService<UserSessionScopeService>();
        sessionScopeService.SetUserSessionService(this);

        _sessionScope = scope;

        return ValueTask.FromResult(scope);
    }

    private async Task AfterHttpResponseAsync(HttpResponseMessage response)
    {
        if (response.Headers.Contains("Set-Cookie"))
        {
            await SaveSessionAsync();
        }

        if (State == UserSessionState.LoggedIn &&
            response.RequestMessage?.RequestUri?.Host == "api.vrchat.cloud" &&
            response.StatusCode == HttpStatusCode.Unauthorized)
        {
            OnStateChanged(UserSessionState.InvalidSession);
        }
    }

    private async ValueTask SaveSessionAsync()
    {
        using (await SimpleSemaphoreSlimLockScope.WaitAsync(_saveSemaphore))
        {
            await _saveFunc(CookieContainer, UserId, UserNameOrEmail, CurrentUser);
        }
    }

    #region Dispose

    public ValueTask DisposeAsync()
    {
        _saveSemaphore.Dispose();
        _sessionHttpClient.Dispose();
        _createOrGetScopeLock.Dispose();

        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        _saveSemaphore.Dispose();
        _sessionHttpClient.Dispose();
        _createOrGetScopeLock.Dispose();
    }

    #endregion

    private void OnStateChanged(UserSessionState e)
    {
        if (e == State)
            return;

        var oldState = State;

        State = e;
        StateChanged?.Invoke(this, e);

        _sessionStateChangedPublisher.Publish(new SessionStateChangedEvent(UserId, UserNameOrEmail, e, oldState));
    }
}

public sealed class UserSessionFactory(
    IServiceScopeFactory scopeFactory,
    VRChatApiClientFactory apiClientFactory,
    IPublisher<SessionStateChangedEvent> sessionInvalidatedPublisher,
    UserSessionHttpClientFactory httpClientFactory,
    ILogger<UserSessionService> logger)
{
    public UserSessionService Create(
        string userNameOrEmail,
        string? userId,
        CookieContainer? cookieContainer,
        CurrentUser? user,
        SaveCookiesDelegate saveFunc
    )
    {
        return new UserSessionService(
            userNameOrEmail,
            userId,
            user,
            saveFunc,
            cookieContainer,
            apiClientFactory,
            sessionInvalidatedPublisher,
            scopeFactory,
            httpClientFactory,
            logger);
    }
}

public enum UserSessionState
{
    Pending,
    LoggedOut,
    LoggedIn,
    InvalidSession
}

public delegate Task SaveCookiesDelegate(
    CookieContainer cookies,
    string? userId,
    string? userNameOrEmail,
    CurrentUser? user);