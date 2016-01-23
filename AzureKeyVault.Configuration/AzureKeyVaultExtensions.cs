using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace AzureKeyVault.Configuration
{
    /// <summary>
    /// Extensions for using the AzureKeyVaultConfigurationProvider when working with an IConfigurationBuilder
    /// </summary>
    public static class AzureKeyVaultExtensions
    {
        /// <summary>
        /// Extension method to populate an AzureKeyVaultConfigurationProvider from the passed in connection string and secret names
        /// </summary>
        /// <param name="builder">IConnfigurationBuilder this method extends</param>
        /// <param name="connectionString">A string in the format of VaultName={url};ClientId={clientId};ClientSecret={clientSecret};</param>
        /// <param name="secretNames">The secrets to load from the Azure Key Vault</param>
        /// <returns>Populated IConfigurationBuilder</returns>
        public static IConfigurationBuilder AddAzureKeyVault(this IConfigurationBuilder builder, string connectionString, IEnumerable<string> secretNames)
        {
            builder.Add(new AzureKeyVaultConfigurationProvider(connectionString, secretNames));
            return builder;
        }

        /// <summary>
        /// Extension method to populate an AzureKeyVaultConfigurationProvider from passing in the connection string parts or environment variable names to retrieve the parts
        /// and the secret names
        /// </summary>
        /// <param name="builder">IConnfigurationBuilder this method extends</param>
        /// <param name="vaultNameOrEnvVarName">The name of the Azure Key Vault or the Environment varialbe name to retrieve it from</param>
        /// <param name="clientIdOrEnvVarName">The name of the ClientId or the Environment variable name to retrieve it from</param>
        /// <param name="clientSecretOrEnvVarName">The name of the ClientSecret or the Environment variable name to retrieve it from</param>
        /// <param name="secretNames">The secrets to load from the Azure Key Vault</param>
        /// <returns>Populated IConfigurationBuilder</returns>
        public static IConfigurationBuilder AddAzureKeyVault(this IConfigurationBuilder builder, string vaultNameOrEnvVarName, string clientIdOrEnvVarName, 
            string clientSecretOrEnvVarName, IEnumerable<string> secretNames)
        {
            var vaultName = GetSetting(vaultNameOrEnvVarName);
            var clientId = GetSetting(clientIdOrEnvVarName);
            var clientSecret = GetSetting(clientSecretOrEnvVarName);
            
            var connectionInfo = new AzureKeyVaultConnectionInfo(vaultName, clientId, clientSecret);
            
            builder.Add(new AzureKeyVaultConfigurationProvider(connectionInfo, secretNames));
            return builder;
        }

        /// <summary>
        /// Utility method that currently (only) checks the Environment variables or uses the passed in string if not found
        /// </summary>
        /// <param name="settingName">Environment variable name or value to use for setting</param>
        /// <returns>String to be used for setting</returns>
        private static string GetSetting(string settingName)
        {
            var settingValue = Environment.GetEnvironmentVariable(settingName);
            if (string.IsNullOrEmpty(settingValue))
            {
                settingValue = settingName;
            }
            return settingValue;
        }
    }
}
