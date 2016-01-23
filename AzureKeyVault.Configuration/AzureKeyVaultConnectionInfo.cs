using System;

namespace AzureKeyVault.Configuration
{
    /// <summary>
    /// Object to hold the Azure Key Vault connection information
    /// </summary>
    public class AzureKeyVaultConnectionInfo
    {
        private static readonly string KeyVaultUriFormat = "https://{0}.vault.azure.net";
        private static readonly char[] PartSeparator = new[] {';'};
        private static readonly char ValueSeparator = '=';
        private static readonly int ValueCount = 3;

        /// <summary>
        /// Constructor to use when all the connection parts are seperate
        /// </summary>
        /// <param name="vaultName">Name of the Azure Key Vault</param>
        /// <param name="clientId">ClientId used for accessing the Azure Key Vault</param>
        /// <param name="clientSecret">ClientSecret used for accessing the Azure Key Vault</param>
        public AzureKeyVaultConnectionInfo(string vaultName, string clientId, string clientSecret)
        {
            if (vaultName == null)
            {
                throw new ArgumentNullException(nameof(vaultName));
            }

            if (clientId == null)
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            if (clientSecret == null)
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }

            VaultName = vaultName;
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        /// <summary>
        /// Costructor used when the connetion parts are in a single string
        /// </summary>
        /// <param name="connectionString">String with the connection information "VaultName={url};ClientId={clientId};ClientSecret={secret};"</param>
        public AzureKeyVaultConnectionInfo(string connectionString)
        {
            Parse(connectionString);
        }

        /// <summary>
        /// Name of the Azure Key Vault
        /// </summary>
        public string VaultName { get; private set; }

        /// <summary>
        /// ClientId to use when getting secrets from the Azure Key Vault
        /// </summary>
        public string ClientId { get; private set; }

        /// <summary>
        /// ClientSecret to use when getting secrets from the Azure Key Vault
        /// </summary>
        public string ClientSecret { get; private set; }

        /// <summary>
        /// Gets the VaultUrl.  Used by the AuzreKeyVaultConfigurationProvider when retrieving the secrets
        /// </summary>
        /// <returns></returns>
        public string GetKeyVaultUrl()
        {
            return string.Format(KeyVaultUriFormat, VaultName).ToLower();
        }

        /// <summary>
        /// Parses a given string into the indivual parts needed to connect to a Azure Key Vault
        /// </summary>
        /// <param name="connectionString"></param>
        private void Parse(string connectionString)
        {
            string[] parts = connectionString.Split(PartSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != ValueCount)
            {
                throw new FormatException("Azure Key Vault connection needs to have only the VaultName, ClientId and ClientSecret");
            }

            foreach (var part in parts)
            {
                int firstSeperator = part.IndexOf(ValueSeparator);

                var name = part.Substring(0, firstSeperator);
                var value = part.Substring(firstSeperator + 1);

                if (String.Compare(name, "VaultName", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    VaultName = value;
                }

                if (String.Compare(name, "ClientId", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ClientId = value;
                }

                if (String.Compare(name, "ClientSecret", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ClientSecret = value;
                }
            }
        }
    }
}
