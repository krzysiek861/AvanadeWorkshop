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
            var storageConnStrSecretName = ConfigurationManager.AppSettings["StorageConnStrSecretName"];
            var serviceBusAccessKeySecretName = ConfigurationManager.AppSettings["ServiceBusAccessKeySecretName"];
            var serviceBusConnStrSecretName = ConfigurationManager.AppSettings["ServiceBusConnStrSecretName"];

            var vaultUri = new Uri(ConfigurationManager.AppSettings["KeyVaultUri"]);
            var secretClient = new SecretClient(vaultUri, new DefaultAzureCredential());

            KeyVaultSecret storageConnStrSecret = secretClient.GetSecret(storageConnStrSecretName);
            GlobalSecrets.StorageAccountConnectionString = storageConnStrSecret.Value;
            
            try
            {
                KeyVaultSecret serviceBusAccessKeySecret = secretClient.GetSecret(serviceBusAccessKeySecretName);
                GlobalSecrets.ServiceBusAccessKey = serviceBusAccessKeySecret.Value;
            }
            catch (Exception)
            {
                GlobalSecrets.ServiceBusAccessKey = null;
            }

            try
            {
                KeyVaultSecret serviceBusConnStrSecret = secretClient.GetSecret(serviceBusConnStrSecretName);
                GlobalSecrets.ServiceBusConnectionString = serviceBusConnStrSecret.Value;
            }
            catch (Exception)
            {
                GlobalSecrets.ServiceBusConnectionString = null;
            }
        }
    }
}