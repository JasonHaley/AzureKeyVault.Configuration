# AzureKeyVault.Configuration
ASP vNext ConfigurationProvider and IConfigurationBuilder Extenstion methods for adding Azure Key Vault secrets to an application's configuration.

All code samples require the Azure Key Vault and secrets to already be configured.

#Typical Usage
When you want to pull the client id and client secret from an environment variable but can pass the vault name and secrets names in the startup.

This code requires that the environment variables "ClientIdEnvSetting" and "ClientSecretEnvSetting" exist on the machine with the proper values need to access the secrets.
```
var builder = new ConfigurationBuilder();
builder.AddAzureKeyVault("YourKeyVault", "ClientIdEnvSetting", "ClientSecretEnvSetting",
  new string[]
  {
    "TestKey"
  }
);
Configuration = builder.Build();
```

To access the "TestKey" in the application, you would use code like this:
```
Startup.Configuration["TestKey"];
```
