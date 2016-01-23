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
    /// <summary>
    /// A configuration provider that will load secrets from an Azure Key Vault into a ConfigurationProvider
    /// </summary>
    public class AzureKeyVaultConfigurationProvider : ConfigurationProvider
    {
        /// <summary>
        /// Constructor used when all the connection informaiton is provided seperately
        /// </summary>
        /// <param name="connectionInfo">A valid AzureKeyVaultConnectionInfo object</param>
        /// <param name="secretNames">The names of the secrets to retrieve from the AzureKeyVault</param>
        public AzureKeyVaultConfigurationProvider(AzureKeyVaultConnectionInfo connectionInfo, IEnumerable<string> secretNames)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException(nameof(connectionInfo));
            }

            if (secretNames == null)
            {
                throw new ArgumentNullException(nameof(secretNames));
            }

            ConnectionInfo = connectionInfo;
            SecretNames = secretNames;
        }

        /// <summary>
        /// Constructor used when a connection string with all the parts is provided
        /// </summary>
        /// <param name="connectionString">A string in the format of VaultName={url};ClientId={clientId};ClientSecret={clientSecret};</param>
        /// <param name="secretNames">The names of the secrets to retrieve from the AzureKeyVault</param>
        public AzureKeyVaultConfigurationProvider(string connectionString, IEnumerable<string> secretNames)
            : this(new AzureKeyVaultConnectionInfo(connectionString), secretNames)
        { }

        /// <summary>
        /// Holds the Azure Key Vault connection information
        /// </summary>
        internal AzureKeyVaultConnectionInfo ConnectionInfo { get; private set; }

        /// <summary>
        /// Names of the secrets to be retrieved from the key vault
        /// </summary>
        protected IEnumerable<string> SecretNames { get; private set; }

        /// <summary>
        /// Retreives the SecretNames from the Key Vault
        /// </summary>
        public override void Load()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (SecretNames.Any())
            {
                var keyVaultClient = GetKeyVaultClient();
                foreach (var secretName in SecretNames)
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

        /// <summary>
        /// Factory method for creating a KeyVaultClient
        /// </summary>
        /// <returns>A valid KeyVaultClient object</returns>
        protected internal KeyVaultClient GetKeyVaultClient()
        {
            return new KeyVaultClient(GetAccessToken, GetHttpClient());
        }

        /// <summary>
        /// Used by the KeyVaultClient
        /// </summary>
        /// <param name="authority"></param>
        /// <param name="resource"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        private async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            var clientCredential = new ClientCredential(ConnectionInfo.ClientId, ConnectionInfo.ClientSecret);
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, clientCredential);

            return result.AccessToken;
        }

        /// <summary>
        /// Factory method for creating an HttpClient
        /// </summary>
        /// <returns>A valid HttpClient</returns>
        protected internal HttpClient GetHttpClient()
        {
            return new HttpClient(new HttpClientHandler());
        }
    }
}
