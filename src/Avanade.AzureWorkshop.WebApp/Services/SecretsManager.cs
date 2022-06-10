using Avanade.AzureWorkshop.WebApp.Models;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Configuration;

namespace Avanade.AzureWorkshop.WebApp.Services
{
    public class SecretsManager
    {
        public void Initialize()
        {
            var storageConnStr_SecretName = ConfigurationManager.AppSettings["StorageConnStrSecretName"];
            var serviceBusAccessKey_SecretName = ConfigurationManager.AppSettings["ServiceBusAccessKeySecretName"];
            var serviceBusConnStr_SecretName = ConfigurationManager.AppSettings["ServiceBusConnStrSecretName"];
            var serviceBusAccessKeyName_SecretName = ConfigurationManager.AppSettings["serviceBusSharedAccessKeyName"];
            var sendgridApiKey_SecretName = ConfigurationManager.AppSettings["SendgridApiKeySecretName"];
            var bingSearchPrimaryKey_SecretName = ConfigurationManager.AppSettings["BingSearchPrimaryKeySecretName"];

            var vaultUri = new Uri(ConfigurationManager.AppSettings["KeyVaultUri"]);
            var secretClient = new SecretClient(vaultUri, new DefaultAzureCredential());

            GlobalSecrets.StorageAccountConnectionString =
                GetSecretValueBySecretName(secretClient, storageConnStr_SecretName);
            GlobalSecrets.ServiceBusAccessKey =
                GetSecretValueBySecretName(secretClient, serviceBusAccessKey_SecretName);
            GlobalSecrets.ServiceBusConnectionString =
                GetSecretValueBySecretName(secretClient, serviceBusConnStr_SecretName);
            GlobalSecrets.SendgridApiKey =
                GetSecretValueBySecretName(secretClient, sendgridApiKey_SecretName);
            GlobalSecrets.BingSearchPrimaryKey =
                GetSecretValueBySecretName(secretClient, bingSearchPrimaryKey_SecretName);
            GlobalSecrets.ServiceBusSharedAccessKeyName =
                GetSecretValueBySecretName(secretClient, serviceBusAccessKeyName_SecretName);
        }

        private string GetSecretValueBySecretName(SecretClient client, string secretName)
        {
            if (string.IsNullOrEmpty(secretName)) return "";

            try
            {
                KeyVaultSecret secret = client.GetSecret(secretName);
                return secret.Value;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}