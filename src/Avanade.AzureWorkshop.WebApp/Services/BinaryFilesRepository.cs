using Avanade.AzureWorkshop.WebApp.Models;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Avanade.AzureWorkshop.WebApp.Services
{
    public class BinaryFilesRepository
    {
        private BlobServiceClient GetBlobserviceClient()
        {
            var serviceClient = new BlobServiceClient(GlobalSecrets.StorageAccountConnectionString);
            return serviceClient;
        }

        public bool AnyFileExists(string containerName, string fileName)
        {
            var serviceClient = GetBlobserviceClient();
            var container = $"{containerName}-{fileName}".ToLower();
            var blobContainerClient = serviceClient.GetBlobContainerClient(container);
            return blobContainerClient.Exists();
        }

        public string SaveBlob(string containerName, string fileName, byte[] bytes)
        {
            var serviceClient = GetBlobserviceClient();
            var container = $"{containerName}-{fileName}".ToLower();
            var blobContainerClient = serviceClient.GetBlobContainerClient(container);
            blobContainerClient.CreateIfNotExists();
            blobContainerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            var blobClient = blobContainerClient.GetBlobClient(Guid.NewGuid().ToString() + ".png");
            using (var stream = new MemoryStream(bytes, writable: false))
            {
                blobClient.Upload(stream);
                return blobClient.Uri.AbsoluteUri;
            }          
        }

        public List<string> GetBlobUrls(string containerName, string fileName)
        {
            var serviceClient = GetBlobserviceClient();
            var container = $"{containerName}-{fileName}".ToLower();
            var blobContainerClient = serviceClient.GetBlobContainerClient(container);
            return blobContainerClient.GetBlobs()
                .Select(b => blobContainerClient.GetBlobClient(b.Name).Uri.ToString())
                .ToList();
        }
    }
}