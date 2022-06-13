using Avanade.AzureWorkshop.WebApp.Models;
using Avanade.AzureWorkshop.WebApp.Models.TableStorageModels;
using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        public void UpdateTeam(TeamEntity team)
        {
            var tableClient = GetServiceClient();
            var table = tableClient.GetTableClient("teams");

            table.UpdateEntity(team, ETag.All, TableUpdateMode.Replace);
        }

        public void UpdatePlayers(IEnumerable<PlayerEntity> players)
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

        public async Task StoreTeams(IEnumerable<TeamEntity> teams)
        {
            var tableClient = GetServiceClient();
            var table = tableClient.GetTableClient("teams");
            await table.CreateIfNotExistsAsync();

            var transactionActions = new List<TableTransactionAction>();

            foreach (var team in teams)
            {
                transactionActions.Add(new TableTransactionAction(TableTransactionActionType.UpdateReplace, team));
            }

            await table.SubmitTransactionAsync(transactionActions);
        }

        public async Task StorePlayers(IEnumerable<PlayerEntity> players)
        {
            var tableClient = GetServiceClient();
            var table = tableClient.GetTableClient("players");

            await table.CreateIfNotExistsAsync();            

            foreach (var group in players.GroupBy(p => p.PartitionKey))
            {
                var transactionActions = new List<TableTransactionAction>();
                foreach (var player in group)
                {
                    transactionActions.Add(new TableTransactionAction(TableTransactionActionType.Add, player));
                }
                await table.SubmitTransactionAsync(transactionActions);
            }            
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

        public TeamEntity FetchTeam(string teamName)
        {
            var tableClient = GetServiceClient();
            var table = tableClient.GetTableClient("teams");

            var result = table.Query<TeamEntity>(t => t.Name == teamName).FirstOrDefault();
            return result;
        }

        public IEnumerable<TeamEntity> FetchTeams()
        {
            var tableClient = GetServiceClient();
            var table = tableClient.GetTableClient("teams");
            return table.Query<TeamEntity>().OrderBy(f => f.Group);
        }

        public IEnumerable<TeamEntity> FetchTeamsByGroup(string group)
        {
            var tableClient = GetServiceClient();
            var table = tableClient.GetTableClient("teams");

            var result = table.Query<TeamEntity>(t => t.Group == group).OrderBy(f => f.Name);
            return result;
        }

        public IEnumerable<PlayerEntity> FetchPlayers(string teamId)
        {
            var tableClient = GetServiceClient();
            var table = tableClient.GetTableClient("players");

            var result = table.Query<PlayerEntity>(p => p.PartitionKey == teamId).OrderBy(f => f.Number);
            return result;
        }

        public PlayerEntity FetchSinglePlayer(string teamId, string playerId)
        {
            var tableClient = GetServiceClient();
            var table = tableClient.GetTableClient("players");

            var player = table.GetEntity<PlayerEntity>(teamId, playerId);
            return player.Value;
        }

        public IEnumerable<PlayerEntity> FetchScorers()
        {
            var tableClient = GetServiceClient();
            var table = tableClient.GetTableClient("players");

            var result = table.Query<PlayerEntity>(p => p.Goals > 0).OrderByDescending(f => f.Goals);
            return result;
        }

        public (string teamId, string playerId) GetRandomPlayer()
        {
            var random = new Random();
            var tableClient = GetServiceClient();
            var teamsTable = tableClient.GetTableClient("teams");
            var playersTable = tableClient.GetTableClient("players");

            var teamNames = teamsTable.Query<TeamEntity>().Select(t => t.RowKey);
            var teamIndex = random.Next(teamNames.Count());
            var randomTeam = teamNames.ElementAt(teamIndex);

            var playerIds = playersTable.Query<PlayerEntity>(p => p.PartitionKey == randomTeam).Select(p => p.RowKey);
            var playerIndex = random.Next(playerIds.Count());
            var randomPlayer = playerIds.ElementAt(playerIndex);

            return (randomTeam, randomPlayer);
        }
    }
}