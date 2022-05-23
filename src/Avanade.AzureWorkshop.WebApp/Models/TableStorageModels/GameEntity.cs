using Azure;
using Azure.Data.Tables;
using System;

namespace Avanade.AzureWorkshop.WebApp.Models.TableStorageModels
{
    public class GameEntity : ITableEntity
    {
        public GameEntity()
        {

        }

        public GameEntity(string group, string id)
        {
            PartitionKey = group;
            RowKey = id;
        }

        public string Group { get { return PartitionKey; } }
        public string Team1Name { get; set; }
        public string Team2Name { get; set; }
        public int Team1Goals { get; set; }
        public int Team2Goals { get; set; }
        public DateTime DateOfGame { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}