using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace AzureKeyVault.Configuration
{
    public static class AzureKeyVaultExtensions
    {
        public static IConfigurationBuilder AddAzureKeyVault(this IConfigurationBuilder builder, string connectionString, IEnumerable<string> secretNames)
        {
            builder.Add(new AzureKeyVaultConfigurationProvider(connectionString, secretNames));
            return builder;
        }
        
    }
}
