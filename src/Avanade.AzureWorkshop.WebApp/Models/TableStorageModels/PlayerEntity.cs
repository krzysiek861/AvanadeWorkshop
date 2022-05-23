using Azure;
using Azure.Data.Tables;
using System;

namespace Avanade.AzureWorkshop.WebApp.Models.TableStorageModels
{
    public class PlayerEntity : ITableEntity
    {
        public PlayerEntity()
        {

        }

        public PlayerEntity(string teamId, string id)
        {
            PartitionKey = teamId;
            RowKey = id;
        }

        public string TeamId { get { return PartitionKey; } }
        public int? Number { get; set; }
        public string Position { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Club { get; set; }
        public int Goals { get; set; }
        public string Thumbnail { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}