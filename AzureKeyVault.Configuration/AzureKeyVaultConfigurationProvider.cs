using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AzureKeyVault.Configuration
{
    public class AzureKeyVaultConfigurationProvider : ConfigurationProvider
    {
        public AzureKeyVaultConfigurationProvider(string connectionString, IEnumerable<string> secretNames)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (secretNames == null)
            {
                throw new ArgumentNullException(nameof(secretNames));
            }

            ConnectionInfo = new AzureKeyVaultConnectionInfo(connectionString);
            SecretsNames = secretNames;
        }


        internal AzureKeyVaultConnectionInfo ConnectionInfo { get; private set; }
        protected IEnumerable<string> SecretsNames { get; private set; }

        public override void Load()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (SecretsNames.Any())
            {
                var keyVaultClient = GetKeyVaultClient();
                foreach (var secretName in SecretsNames)
                {
                    var secret =
                        keyVaultClient.GetSecretAsync(ConnectionInfo.GetKeyVaultUrl(), secretName)
                            .GetAwaiter()
                            .GetResult();
                    data[secretName] = secret.Value;
                }
            }

            Data = data;
        }

        protected internal KeyVaultClient GetKeyVaultClient()
        {
            return new KeyVaultClient(GetAccessToken, GetHttpClient());
        }

        private async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            var clientCredential = new ClientCredential(ConnectionInfo.ClientId, ConnectionInfo.ClientSecret);
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, clientCredential);

            return result.AccessToken;
        }

        private static HttpClient GetHttpClient()
        {
            return new HttpClient(new HttpClientHandler());
        }
    }
}
