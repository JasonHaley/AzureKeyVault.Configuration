using System.CodeDom;
using Microsoft.Extensions.Configuration;

namespace AzureKeyVault.Configuration
{
    public class AzureKeyVaultConfigurationProvider : ConfigurationProvider
    {
        public AzureKeyVaultConfigurationProvider() : this(string.Empty)
        { }

        public AzureKeyVaultConfigurationProvider(string prefix)
        {
            // TODO:
        }
    }
}
