using System.Net.Http;
using System.Net.Http.Headers;

namespace CatervaClient
{
    public static class TokenInjector
    {
        // in? out? ref?
        public static void Inject(HttpClient httpClient)
        {
            var token = CatervaNetworkManager.Instance.GetToken();
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public partial class AppControllerClient
    {
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            TokenInjector.Inject(client);
        }
    }
    
    public partial class AccountControllerClient
    {
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            TokenInjector.Inject(client);
        }
    }
    
    /* Auth controller does not need token
    public partial class AuthControllerClient
    {
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            TokenInjector.Inject(client);
        }
    }
    */

    public partial class FriendControllerClient
    {
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            TokenInjector.Inject(client);
        }
    }
    
    public partial class GroupControllerClient
    {
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            TokenInjector.Inject(client);
        }
    }
    
    public partial class LeaderboardControllerClient
    {
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            TokenInjector.Inject(client);
        }
    }
    
    public partial class ProfileControllerClient
    {
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            TokenInjector.Inject(client);
        }
    }
    
    public partial class WalletControllerClient
    {
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            TokenInjector.Inject(client);
        }
    }
}