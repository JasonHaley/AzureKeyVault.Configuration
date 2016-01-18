using System;

namespace AzureKeyVault.Configuration
{
    /// <summary>
    /// Object to hold the Azure Key Vault connection information
    /// </summary>
    internal class AzureKeyVaultConnectionInfo
    {
        private static readonly string KeyVaultUriFormat = "https://{0}.vault.azure.net";
        private static readonly char[] PartSeparator = new[] {';'};
        private static readonly char ValueSeparator = '=';
        private static readonly int ValueCount = 3;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString">String with the connection information "VaultName={url};ClientId={clientId};ClientSecret={secret};"</param>
        public AzureKeyVaultConnectionInfo(string connectionString)
        {
            Parse(connectionString);
        }

        internal string VaultName { get; private set; }
        internal string ClientId { get; private set; }
        internal string ClientSecret { get; private set; }

        internal string GetKeyVaultUrl()
        {
            return string.Format(KeyVaultUriFormat, VaultName).ToLower();
        }

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
