using Avanade.AzureWorkshop.WebApp.Models;
using Avanade.AzureWorkshop.WebApp.Models.TableStorageModels;
using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avanade.AzureWorkshop.WebApp.Services
{
    public class TeamsRepository
    {
        private TableServiceClient GetServiceClient()
        {
            var serviceClient = new TableServiceClient(GlobalSecrets.StorageAccountConnectionString);
            return serviceClient;
        }

        public void UpdateTeam(dynamic team)
        {
            var tableClient = GetServiceClient();
            var table = tableClient.GetTableClient("teams");

            table.UpdateEntity(team, ETag.All, TableUpdateMode.Replace);
        }

        public void UpdatePlayers(IEnumerable<dynamic> players)
        {
            var tableClient = GetServiceClient();
            var table = tableClient.GetTableClient("players");

            var transactionActions = new List<TableTransactionAction>();

            foreach (var player in players)
            {
                transactionActions.Add(new TableTransactionAction(TableTransactionActionType.UpdateReplace, player));
            }

            table.SubmitTransaction(transactionActions);
        }

        public async Task StoreTeams(IEnumerable<dynamic> teams)
        {
            throw new NotImplementedException();
        }

        public async Task StorePlayers(IEnumerable<dynamic> players)
        {
            throw new NotImplementedException();
        }

        public async Task StoreGame(GameEntity game)
        {
            var tableClient = GetServiceClient();
            var table = tableClient.GetTableClient("games");

            await table.CreateIfNotExistsAsync();
            await table.AddEntityAsync(game);
        }

        public IEnumerable<GameEntity> FetchGamesByGroup(string group)
        {
            var tableClient = GetServiceClient();
            var table = tableClient.GetTableClient("games");
            table.CreateIfNotExists();

            var result = table.Query<GameEntity>(g => g.PartitionKey == group).OrderBy(f => f.DateOfGame);
            return result;
        }

        public dynamic FetchTeam(string teamName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> FetchTeams()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> FetchTeamsByGroup(string group)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> FetchPlayers(string teamId)
        {
            throw new NotImplementedException();
        }

        public dynamic FetchSinglePlayer(string teamId, string playerId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> FetchScorers()
        {
            throw new NotImplementedException();
        }

        public (string teamId, string playerId) GetRandomPlayer()
        {
            throw new NotImplementedException();
        }
    }
}