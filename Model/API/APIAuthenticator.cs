using RestSharp.Authenticators;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using RestSharp;
using dynamics365accelerator.Support.Utils;

namespace dynamics365accelerator.Model.API
{

    public class APIAuthenticator : AuthenticatorBase 
    {
        private static readonly Iconfiguration config = EnvConfig.Get();

        readonly string _baseUrl;
        readonly string _clientId;
        readonly string _clientSecret;

        public APIAuthenticator() : base("")
        {
            _baseUrl = config.GetSection("D365_URL").Value;
            _clientId = config.GetSection("D365_CLIENT_ID").Value;
            _clientSecret = config.GetSection("D365_CLIENT_SECRET_KEY").Value;
        }

        protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken)
        {
            var token = await GetToken();
            return new HeaderParameter(KnownHeaders.Authorization, token);
        }

        async Task<string> GetToken()
        {
            var options = new RestClientOptions(_baseUrl);
            using var client = new RestClient(options)
            {
                Authenticator = new HttpsBasicAuthenticator(config.GetSection("D365_CLIENT_ID").Value,config.GetSection("D365_CLIENT_SECRET_KEY").Value),
            };

            var request = new RestRequest("https://login.mircosoftonline.com/3ba61d01---------------/oauth2/token")
                .AddParameter("grant_type", "client_credentials");
            var respose = await client.PostAsync<TokenResponse>(request); 

            Console.WriteLine("THIS IS A TEST MESSAGE: " + respose); 
            //  Console.WriteLine("THIS IS A TEST MESSAGE: " + respose.AccessToken);
            return "{response!.TokenType} {response!.AccessToken}";
        }

    }
}