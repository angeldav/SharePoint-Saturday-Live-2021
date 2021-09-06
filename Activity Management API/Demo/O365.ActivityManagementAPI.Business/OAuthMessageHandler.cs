using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace O365.ActivityManagementAPI.Business
{
    public class OAuthMessageHandler : DelegatingHandler
    {
        private AuthenticationHeaderValue authHeader;

        public OAuthMessageHandler(string tenantId, string clientId, string clientSecret, System.Net.Http.HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            Logging.LogMessage("Obteniendo Bearer Token para la Activity Management API");
            try
            {
                var authenticationContext = new AuthenticationContext("https://login.windows.net/" + tenantId, false);
                ClientCredential clientCred = new ClientCredential(clientId, clientSecret);
                AuthenticationResult authenticationResult = null;
                Task runTask = Task.Run(async () => authenticationResult = await authenticationContext.AcquireTokenAsync("https://manage.office.com", clientCred));
                runTask.Wait();
                string token = authenticationResult.AccessToken;

                authHeader = new AuthenticationHeaderValue("Bearer", token);
                Logging.LogMessage("Bearer Token obtenido correctamente");
            }
            catch (Exception ex)
            {
                Logging.LogMessage("EXCEPCIÓN en OAuthMessageHandler: " + ex.Message);
                throw ex;
            }
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            request.Headers.Authorization = authHeader;
            var _response = await base.SendAsync(request, cancellationToken);

            return _response;
        }
    }
}
