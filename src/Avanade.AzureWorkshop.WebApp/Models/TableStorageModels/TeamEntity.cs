using Azure;
using Azure.Data.Tables;
using System;

namespace Avanade.AzureWorkshop.WebApp.Models.TableStorageModels
{
    public class TeamEntity : ITableEntity
    {
        public TeamEntity()
        {

        }

        public TeamEntity(string partition, string id)
        {
            PartitionKey = partition;
            RowKey = id;
        }

        public string Group { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Flag { get; set; }
        public int Games { get; set; }
        public int Points { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}