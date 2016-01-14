

using Microsoft.Extensions.Configuration;

namespace AzureKeyVault.Configuration
{
    public static class AzureKeyVaultExtensions
    {
        public static IConfigurationBuilder AddAzureKeyVault(this IConfigurationBuilder builder)
        {
            builder.Add(new AzureKeyVaultConfigurationProvider());
            return builder;
        }

        public static IConfigurationBuilder AddAzureKeyVault(this IConfigurationBuilder builder, string prefix)
        {
            builder.Add(new AzureKeyVaultConfigurationProvider(prefix));
            return builder;
        }
    }
}
