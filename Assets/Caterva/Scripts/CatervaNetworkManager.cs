using System;
using System.Net.Http;
using System.Threading.Tasks;
using CatervaClient;
using CatervaGame.UI;
using CatervaGame.UI.Profile;
using Newtonsoft.Json;
using UnityEngine;

public class CatervaNetworkManager : MonoBehaviour
{
    [SerializeField]
    private string backendAddress = "http://localhost:3000";
    private HttpClient _httpClient;
    [SerializeField]
    private string currentToken;
    public Account currentAccount;
    public Profile currentProfile;
    [SerializeField] private string debugDeviceId;

    public AppControllerClient appControllerClient;
    public AccountControllerClient accountControllerClient;
    public AuthControllerClient authControllerClient;
    public FriendControllerClient friendControllerClient;
    public GroupControllerClient groupControllerClient;
    public LeaderboardControllerClient leaderboardControllerClient;
    public ProfileControllerClient profileControllerClient;
    public WalletControllerClient walletControllerClient;

    public string GetToken() => currentToken;

    public static CatervaNetworkManager Instance;

    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        SetSingleton();
    }

    private void SetSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    async void Start()
    {
        InitControllerClients();
        await AssertConnection();
        Debug.Log("Caterva connection OK");

        await Authenticate();

        currentAccount = await accountControllerClient.GetCurrentAccountAsync();
        Debug.Log(JsonConvert.SerializeObject(currentAccount));
    }

    private void InitControllerClients()
    {
        _httpClient = new HttpClient();
        appControllerClient = new AppControllerClient(backendAddress, _httpClient);
        accountControllerClient = new AccountControllerClient(backendAddress, _httpClient);
        authControllerClient = new AuthControllerClient(backendAddress, _httpClient);
        friendControllerClient = new FriendControllerClient(backendAddress, _httpClient);
        groupControllerClient = new GroupControllerClient(backendAddress, _httpClient);
        leaderboardControllerClient = new LeaderboardControllerClient(backendAddress, _httpClient);
        profileControllerClient = new ProfileControllerClient(backendAddress, _httpClient);
        walletControllerClient = new WalletControllerClient(backendAddress, _httpClient);
        IsInitialized = true;
    }

    private async Task AssertConnection()
    {
        var helloResponse = await appControllerClient.GetHelloAsync();
        if (!helloResponse.Equals("Hello World!"))
        {
            throw new Exception("invalid caterva conn, root response body does not match");
        }
    }

    private async Task Authenticate()
    {
        try
        {
            var authResponse = await authControllerClient.AuthWithDeviceIdAsync(new AuthDto
            {
                Id = string.IsNullOrEmpty(debugDeviceId) ? SystemInfo.deviceUniqueIdentifier : debugDeviceId,
            });

            currentToken = authResponse.AccessToken;

            CreateOrGetProfile();
            // currentAccount = await accountControllerClient.GetCurrentAccountAsync();
            Debug.Log("auth successful");
        }
        catch (ApiException e)
        {
            if (!"{\"status\":400,\"error\":\"Authentication failed\"}".Equals(e.Response))
            {
                Debug.LogError("unknown error occurred during auth, aborting");
                throw;
            }

            Debug.LogWarning("auth failed, trying account creation");


            var createAccountResp = await accountControllerClient.CreateAccountAsync(new CreateAccountDto
            {
                DeviceId = string.IsNullOrEmpty(debugDeviceId) ? SystemInfo.deviceUniqueIdentifier : debugDeviceId,
            });
            var authResponse = await authControllerClient.AuthWithDeviceIdAsync(new AuthDto
            {
                Id = string.IsNullOrEmpty(debugDeviceId) ? SystemInfo.deviceUniqueIdentifier : debugDeviceId,
            });

            currentToken = authResponse.AccessToken;
            currentAccount = createAccountResp;

            CreateOrGetProfile();
        }
    }

    private async void CreateOrGetProfile()
    {
        try
        {
            var profileDto = await profileControllerClient.FindMeAsync();
            if (profileDto?.Profile != null)
            {
                currentProfile = profileDto.Profile;
            }
            else
            {
                CreateProfileViewDisplayManager.Instance.OpenCreateProfilePage();
                CreateProfileViewDisplayManager.Instance.BlockButtons();
            }
        }
        catch (Exception ex)
        {
            CreateProfileViewDisplayManager.Instance.OpenCreateProfilePage();
            CreateProfileViewDisplayManager.Instance.BlockButtons();
            Debug.LogError($"Error while getting profile!\n{ex}");
        }
    }
}