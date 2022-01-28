using System;

using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.OpenApi.Models;

namespace FunctionApp.SecurityFlows
{
    public class AuthCodeAuthFlow : OpenApiOAuthSecurityFlows
    {
        private const string AuthorisationUrl = "https://localhost:5001/authorize";
        private const string TokenUrl = "https://localhost:5001/token";
        private const string RefreshUrl = "https://localhost:5001/token";

        public AuthCodeAuthFlow()
        {
            var tenantId = Environment.GetEnvironmentVariable("OpenApi__Auth__TenantId");

            this.AuthorizationCode = new OpenApiOAuthFlow()
            {
                AuthorizationUrl = new Uri(string.Format(AuthorisationUrl, tenantId)),
                TokenUrl = new Uri(string.Format(TokenUrl, tenantId)),
                RefreshUrl = new Uri(string.Format(RefreshUrl, tenantId)),

                Scopes = { { "https://graph.microsoft.com/.default", "Default scope defined in the app" } }
            };
        }
    }
}
