using Avanade.AzureWorkshop.WebApp.Models;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Avanade.AzureWorkshop.WebApp.Services
{
    public class BinaryFilesRepository
    {
        public bool AnyFileExists(string containerName, string fileName)
        {
            throw new NotImplementedException();
        }

        public string SaveBlob(string containerName, string fileName, byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public List<string> GetBlobUrls(string containerName, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}