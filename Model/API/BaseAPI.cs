using System;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth2;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using dynamics365accelerator.Support.Utils;

namespace dynamics365accelerator.Model.API
{
    public class BaseAPI
    {
        private static readonly Iconfiguration config = EnvConfig.Get();

        public RestClient Client;
        public RestRequest Request;
        public RestResponse Response;
        public string baseUrl = config.GetSection("D365_URL").Value;

        public string GetToken()
        {
            var options = new RestClientOptions(_baseUrl);
            using var client = new RestClient(options)
            {
                Authenticator = new HttpsBasicAuthenticator(config.GetSection("D365_CLIENT_ID").Value,config.GetSection("D365_CLIENT_SECRET_KEY").Value),
            };

            var request = new RestRequest("https://login.mircosoftonline.com/3ba61d01---------------/oauth2/token")
                .AddParameter("grant_type", "client_credentials");
            request.AddParameter("resource", "https://dynamics365accelerator.com"); 

            var respose =  client.Post<TokenResponse>(request); 

            return response.AccessToken;
        }

        public BaseAPI SetBaseURI()
        {
            var options = new RestClientOptions(baseUrl);
            Client = new RestClient(options);
            Client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(GetToken(),"Bearer");
            return this;

        }

        public SalesOrderLine? GetSalesOrderLine(string soNumber)
        {
            var uri = "/data/SalesOrderLines?$filter=SalesOrderNUmber%20eq%20%27" + soNumber + "%27";
            Request = new RestRequest(uri, Method.Get);
            Request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            var queryResult = Client.Execute(Request);
            Console.WriteLine(queryResult.Content);
            return null;
        }

        private SalesOrderLine GetResponse()
        {
            var response = Client.GetJson<SalesOrderLine>("data/SalesOrderLine?");
            Console.WriteLine("RESPONSE CONTENT: " + response);
            Console.WriteLine("RESPONSE CONTENT: " + response.FormattedDeliveryAddress);
            return response;
        }

    }
}