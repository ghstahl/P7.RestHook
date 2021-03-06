﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;
using Neo4jClient.Cypher;
using P7.RestHook.ClientManagement;
using P7.RestHook.ClientManagement.Models;
using P7.RestHook.Models;
using P7.RestHook.Store;

namespace P7.RestHook.Neo4jStore
{
    public class Neo4jRestHookClientManagementStore : 
        BaseStore,
        IRestHookClientManagementStore, 
        IRestHookClientManagementStoreTest
    {
        public Neo4jRestHookClientManagementStore(
            IGraphClient graphClient) : base(graphClient)
        {
        }
        public async Task<RestHookResult> DropAsync()
        {

            var query = new CypherFluentQuery(GraphClient)
                .Match("((n)-[r]->())")
                .DetachDelete("r");
            await query.ExecuteWithoutResultsAsync();
            query = new CypherFluentQuery(GraphClient)
                .Match("((n))")
                .DetachDelete("n");
            await query.ExecuteWithoutResultsAsync();
            return RestHookResult.SuccessResult;
        }
        public async Task<RestHookDataResult<HookUser>> UpsertHookUserAsync(string userId)
        {
            var found = await FindHookUserAsync(userId);
            HookUser user = found.Data;
           
            if (found.Data == null)
            {
                user = new HookUser()
                {
                    UserId = userId
                };
                var query = new CypherFluentQuery(GraphClient)
                    .Merge("(n:HookUser { UserId: {userId} })")
                    .OnCreate()
                    .Set("n = {record}")
                    .WithParams(new
                    {
                        userId = user.UserId,
                        record = user
                    });
                await query.ExecuteWithoutResultsAsync();
            }

            return RestHookDataResult<HookUser>.SuccessResult(user);
        }

        public async Task<RestHookResult> DeleteHookUserAsync(string userId)
        {
            Throw.ArgumentException.IfNullOrWhiteSpace(userId, nameof(userId));
            ThrowIfDisposed();
            /*
            var findQuery = new CypherFluentQuery(GraphClient)
                .Match($"(user:HookUser)")
                .Where((HookUser user) => user.UserId== userId)
                .AndWhere("(user)-[:OWNS]->()")
                .Return(user => user.As<HookUser>());
            var findResult = (await findQuery.ResultsAsync).SingleOrDefault();
            if (findResult != null)
            {
                // not allowed
                return RestHookResult.FailedResult(new RestHookResultError()
                {
                    Message = "Not allowed because relationships exist"
                });
            }
            */
            var query = new CypherFluentQuery(GraphClient)
                .Match($"(user:HookUser)")
                .Where((HookUser user) => user.UserId == userId)
                .AndWhere("size((user)-[:OWNS]->(:HookClient))=0")
                .DetachDelete("user");

            await query.ExecuteWithoutResultsAsync();
            return RestHookResult.SuccessResult;

        }
        public async Task<RestHookDataResult<HookUrl>> FindHookUrlAsync(
            string userId, string clientId, string url)
        {
            Throw.ArgumentException.IfNull(userId, nameof(userId));
            Throw.ArgumentException.IfNull(clientId, nameof(clientId));
            Throw.ArgumentException.IfNull(url, nameof(url));
            ThrowIfDisposed();

            var query = new CypherFluentQuery(GraphClient)
                .Match(
                    "((consumerUser:HookUser)-[:OWNS]->(consumerHookClient:HookClient))",
                    "((consumerHookClient:HookClient)-[:OWNS]->(hookUrl:HookUrl))")
                .Where((HookUser consumerUser) => consumerUser.UserId == userId)
                .AndWhere((HookClient consumerHookClient) => consumerHookClient.ClientId == clientId)
                .AndWhere((HookUrl hookUrl) => hookUrl.Url == url)
                .Return(hookUrl => hookUrl.As<HookUrl>());
            var foundHookUrl = (await query.ResultsAsync).SingleOrDefault();

            var result = (await query.ResultsAsync).SingleOrDefault();
            if (result == null)
            {
                return RestHookDataResult<HookUrl>.FailedResult(new RestHookResultError()
                {
                    ErrorCode = 404,
                    Message = $"Url:{url} not found"
                });
            }
            return RestHookDataResult<HookUrl>.SuccessResult(result);

        }

        public async Task<RestHookDataResult<HookUser>> FindHookUserAsync(string userId)
        {
            Throw.ArgumentException.IfNull(userId, nameof(userId));
            ThrowIfDisposed();

            var query = new CypherFluentQuery(GraphClient)
                .Match($"(r:HookUser)")
                .Where((HookUser r) => r.UserId == userId)
                .Return(r => r.As<HookUser>());

            var result = (await query.ResultsAsync).SingleOrDefault();
            if (result == null)
            {
                return RestHookDataResult<HookUser>.FailedResult(new RestHookResultError()
                {
                    ErrorCode = 404,
                    Message = $"user{userId} not found"
                });
            }
            return RestHookDataResult<HookUser>.SuccessResult(result);

        }

        public async Task<RestHookDataResult<List<HookClient>>> FindHookUserClientsAsync(string userId)
        {
            Throw.ArgumentException.IfNull(userId, nameof(userId));
            ThrowIfDisposed();
            var hookUserResult = await FindHookUserAsync(userId);
            if (!hookUserResult.Success)
            {
                return RestHookDataResult<List<HookClient>>.FailedResult(hookUserResult.Error);
            }

            var query = new CypherFluentQuery(GraphClient)
                .Match("(r:HookUser)-[:OWNS]->(c:HookClient)")
                .Where((HookUser r) => r.UserId == userId)
                .Return((c)=>c.As<HookClient>());
            var result = (await query.ResultsAsync);
 
            return RestHookDataResult<List<HookClient>>.SuccessResult(result.ToList());

        }

        public async Task<RestHookDataResult<HookClient>> FindHookClientAsync(string userId, string clientId)
        {
            Throw.ArgumentException.IfNull(userId, nameof(userId));
            Throw.ArgumentException.IfNull(clientId, nameof(clientId));
            ThrowIfDisposed();

            var query = new CypherFluentQuery(GraphClient)
                .Match("((user:HookUser)-[:OWNS]->(client:HookClient))")
                .Where((HookUser user) => user.UserId == userId)
                .AndWhere((HookClient client) => client.ClientId == clientId)
                .Return(client => client.As<HookClient>());

            var foundHookClient = (await query.ResultsAsync).SingleOrDefault();
            if (foundHookClient == null)
            {
                return RestHookDataResult<HookClient>.FailedResult(new RestHookResultError()
                {
                    ErrorCode = 404
                });
            }
            return RestHookDataResult<HookClient>.SuccessResult(foundHookClient);

        }

        public async Task<RestHookDataResult<HookClient>> CreateHookClientAsync(string userId)
        {
            Throw.ArgumentException.IfNull(userId, nameof(userId));
            ThrowIfDisposed();
            var hookUserResult = await FindHookUserAsync(userId);
            if (!hookUserResult.Success)
            {
                return RestHookDataResult<HookClient>.FailedResult(hookUserResult.Error);
            }

            var hookClient = new HookClient()
            {
                ClientId = Unique.G
            };

            var query = new CypherFluentQuery(GraphClient)
                .Create("(c:HookClient {hookClient})")
                .WithParam("hookClient", hookClient);
            await query.ExecuteWithoutResultsAsync();

            query = new CypherFluentQuery(GraphClient)
                .Match("(user:HookUser)", "(client:HookClient)")
                .Where((HookUser user) => user.UserId == userId)
                .AndWhere((HookClient client) => client.ClientId == hookClient.ClientId)
                .CreateUnique("(user)-[:OWNS]->(client)");
            await query.ExecuteWithoutResultsAsync();

            return RestHookDataResult<HookClient>.SuccessResult(hookClient);
        }

        public async Task<RestHookResult> DeleteHookClientAsync(string userId, string clientId)
        {
            Throw.ArgumentException.IfNullOrWhiteSpace(userId, nameof(userId));
            Throw.ArgumentException.IfNullOrWhiteSpace(clientId, nameof(clientId));
            ThrowIfDisposed();

            var query = new CypherFluentQuery(GraphClient)
                .Match("((user:HookUser)-[:OWNS]->(client:HookClient))")
                .Where((HookUser user) => user.UserId == userId)
                .AndWhere((HookClient client) => client.ClientId== clientId)
                .AndWhere("size((client)-[:OWNS]->(:HookEvent))=0")
                .DetachDelete("client");

            await query.ExecuteWithoutResultsAsync();
            return RestHookResult.SuccessResult;
        }

        public async Task<RestHookResult> DeepCleanProducerHookClientAsync(string userId, string clientId)
        {
            Throw.ArgumentException.IfNullOrWhiteSpace(userId, nameof(userId));
            Throw.ArgumentException.IfNullOrWhiteSpace(clientId, nameof(clientId));
            ThrowIfDisposed();

            var query = new CypherFluentQuery(GraphClient)
                .Match(
                    "((user:HookUser)-[:OWNS]->(client:HookClient))",
                    "((client:HookClient)-[rHookClient]->(hookEvent:HookEvent))",
                    "((hookEvent:HookEvent)-[rHookEvent]->())")
                .Where((HookUser user) => user.UserId == userId)
                .AndWhere((HookClient client) => client.ClientId == clientId)
                .DetachDelete("rHookClient,rHookEvent,hookEvent");

            await query.ExecuteWithoutResultsAsync();
            return RestHookResult.SuccessResult;
        }

        public async Task<RestHookResult> DeepCleanConsumerHookClientAsync(string userId, string clientId)
        {
            Throw.ArgumentException.IfNullOrWhiteSpace(userId, nameof(userId));
            Throw.ArgumentException.IfNullOrWhiteSpace(clientId, nameof(clientId));
            ThrowIfDisposed();

            var query = new CypherFluentQuery(GraphClient)
                .Match(
                    "((user:HookUser)-[:OWNS]->(client:HookClient))",
                    "((client:HookClient)-[rHookClient]->(hookUrl:HookUrl))",
                    "((hookUrl:HookUrl)-[rHookUrl]-())")
                .Where((HookUser user) => user.UserId == userId)
                .AndWhere((HookClient client) => client.ClientId == clientId)
                .DetachDelete("rHookUrl,rHookClient,hookUrl");

            await query.ExecuteWithoutResultsAsync();
            return RestHookResult.SuccessResult;
        }

        public async Task<RestHookDataResult<IEnumerable<HookRecord>>> FindHookRecordsAsync(string userId, string clientId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<RestHookDataResult<HookRecord>> AddHookRecordAsync(string userId, HookRecord hookRecord)
        {
            throw new System.NotImplementedException();
        }

        public async Task<RestHookResult> DeleteHookRecordAsync(string userId, string clientId, string hookRecordId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<RestHookDataResult<IEnumerable<HookEvent>>> 
            FindHookEventsAsync(string userId, string clientId)
        {

            Throw.ArgumentException.IfNull(userId, nameof(userId));
            Throw.ArgumentException.IfNull(clientId, nameof(clientId));
            ThrowIfDisposed();

            var query = new CypherFluentQuery(GraphClient)
                .Match("(user:HookUser)-[:OWNS]->(hookClient:HookClient)")
                .Match("(hookClient:HookClient)-[:PRODUCES]->(hookEvent:HookEvent)")
                .Where((HookUser user) => user.UserId == userId)
                .AndWhere((HookClient hookClient) => hookClient.ClientId== clientId)
                .Return((hookEvent) => hookEvent.As<HookEvent>());
            var result = (await query.ResultsAsync);

            return RestHookDataResult<IEnumerable<HookEvent>>.SuccessResult(result);
        }

        public async Task<RestHookDataResult<IEnumerable<HookUrl>>> FindProducerHookEventCallbackUrlsAsync(
            string userId, string clientId,string eventName)
        {
            Throw.ArgumentException.IfNull(userId, nameof(userId));
            Throw.ArgumentException.IfNull(clientId, nameof(clientId));
            Throw.ArgumentException.IfNull(eventName, nameof(eventName));
            ThrowIfDisposed();

            //PUBLISHES_TO
            var query = new CypherFluentQuery(GraphClient)
                .Match(
                    "((user:HookUser)-[:OWNS]->(hookClient:HookClient))",
                    "((hookClient:HookClient)-[:PRODUCES]->(hookEvent:HookEvent))",
                    "((hookEvent:HookEvent)-[:PUBLISHES_TO]->(hookUrl:HookUrl))")
                .Where((HookUser user) => user.UserId == userId)
                .AndWhere((HookClient hookClient) => hookClient.ClientId == clientId)
                .AndWhere((HookEvent hookEvent) => hookEvent.Name == eventName)
                .Return(hookUrl => hookUrl.As<HookUrl>());
            var foundHookUrls = (await query.ResultsAsync);

            return RestHookDataResult<IEnumerable<HookUrl>>.SuccessResult(foundHookUrls);

        }

        public async Task<RestHookResult> AddProducesHookEventAsync(
            string userId, string clientId, string eventName)
        {
            var foundEventRecord = await FindHookEventAsync(userId, clientId, eventName);
            if (foundEventRecord.Success)
            {
                return RestHookResult.SuccessResult;
            }

            var hookEventNew = new HookEvent()
            {
                Id = Unique.G,
                Name = eventName
            };

            var query = new CypherFluentQuery(GraphClient)
                .Create("(:HookEvent {record})")
                .WithParam("record", hookEventNew);
            await query.ExecuteWithoutResultsAsync();

            query = new CypherFluentQuery(GraphClient)
                .Match(
                    "((user:HookUser)-[:OWNS]->(hookClient:HookClient))", 
                    "(hookEvent:HookEvent)")
                .Where((HookUser user) => user.UserId == userId)
                .AndWhere((HookClient hookClient) => hookClient.ClientId == clientId)
                .AndWhere((HookEvent hookEvent) => hookEvent.Id == hookEventNew.Id)
                .AndWhere("(user)-[:OWNS]->(hookClient)")
                .CreateUnique("(hookClient)-[:PRODUCES]->(hookEvent)");
            await query.ExecuteWithoutResultsAsync();

            return RestHookResult.SuccessResult;

        }

        public async Task<RestHookResult> AddConsumerHookEventAsync(
            string producerUserId, string producerClientId,
            string eventName,
            string consumerUserId, string consumerClientId, string callbackUrl)
        {
            var foundUrlResult = await FindHookUrlAsync(consumerUserId, consumerClientId, callbackUrl);
            HookUrl foundHookUrl = foundUrlResult.Success ? foundUrlResult.Data : null;
            var urlId = foundHookUrl == null?Unique.G: foundHookUrl.Id;
            if (foundHookUrl == null)
            {
                foundHookUrl = new HookUrl()
                {
                    Id = urlId,
                    Url = callbackUrl,
                    Validated = false
                };
                var queryCreateHookUrl = new CypherFluentQuery(GraphClient)
                    .Merge("(n:HookUrl { url: {url} })")
                    .OnCreate()
                    .Set("n = {record}")
                    .WithParams(new
                    {
                        url = foundHookUrl.Url,
                        record = foundHookUrl
                    });
                await queryCreateHookUrl.ExecuteWithoutResultsAsync();
            }

            

            var query2 = new CypherFluentQuery(GraphClient)
                .Match(
                    "((consumerUser:HookUser)-[:OWNS]->(consumerHookClient:HookClient))",
                    "(hookUrl:HookUrl)")
                .Where((HookUser consumerUser) => consumerUser.UserId == consumerUserId)
                .AndWhere((HookClient consumerHookClient) => consumerHookClient.ClientId == consumerClientId)
                .AndWhere((HookUrl hookUrl) => hookUrl.Id == urlId)
                .CreateUnique($"(consumerHookClient)-[:OWNS]->(hookUrl)");
            await query2.ExecuteWithoutResultsAsync();

            var query3 = new CypherFluentQuery(GraphClient)
                .Match(
                    "((producerUser:HookUser)-[:OWNS]->(producerHookClient:HookClient))",
                    "((producerHookClient:HookClient)-[:PRODUCES]->(hookEvent:HookEvent))",
                    "(hookUrl:HookUrl)")
                .Where((HookUser producerUser) => producerUser.UserId == producerUserId)
                .AndWhere((HookClient producerHookClient) => producerHookClient.ClientId == producerClientId)
                .AndWhere((HookEvent hookEvent) => hookEvent.Name == eventName)
                .AndWhere((HookUrl hookUrl) => hookUrl.Url == callbackUrl)
                .CreateUnique("(hookEvent)-[:PUBLISHES_TO]->(hookUrl)");

            await query3.ExecuteWithoutResultsAsync();
            return RestHookResult.SuccessResult;
        }

        public async Task<RestHookDataResult<HookEvent>> FindHookEventAsync(
            string userId, string clientId, string eventName)
        {
            Throw.ArgumentException.IfNull(userId, nameof(userId));
            Throw.ArgumentException.IfNull(clientId, nameof(clientId));
            Throw.ArgumentException.IfNull(eventName, nameof(eventName));
            ThrowIfDisposed();

            var query = new CypherFluentQuery(GraphClient)
                .Match(
                    "((user:HookUser)-[:OWNS]->(hookClient:HookClient))",
                    "((hookClient:HookClient)-[:PRODUCES]->(hookEvent:HookEvent))")
                .Where((HookUser user) => user.UserId == userId)
                .AndWhere((HookClient hookClient) => hookClient.ClientId == clientId)
                .AndWhere((HookEvent hookEvent) => hookEvent.Name == eventName)
                .Return(hookEvent => hookEvent.As<HookEvent>());

            var foundHookEvent = (await query.ResultsAsync).SingleOrDefault();
            if (foundHookEvent == null)
            {
                return RestHookDataResult<HookEvent>.FailedResult(new RestHookResultError()
                {
                    ErrorCode = 404
                });
            }
            return RestHookDataResult<HookEvent>.SuccessResult(foundHookEvent);
               
        }


        public async Task<RestHookResult> DeleteHookEventAsync(
            string userId, string clientId, string eventName)
        {
            Throw.ArgumentException.IfNullOrWhiteSpace(userId, nameof(userId));
            Throw.ArgumentException.IfNullOrWhiteSpace(clientId, nameof(clientId));
            Throw.ArgumentException.IfNullOrWhiteSpace(eventName, nameof(eventName));
            ThrowIfDisposed();

            var query = new CypherFluentQuery(GraphClient)
                .Match(
                    "((user:HookUser)-[:OWNS]->(client))",
                    "((client:HookClient)-[:PRODUCES]->(hookEvent:HookEvent))")
                .Where((HookUser user) => user.UserId == userId)
                .AndWhere((HookClient client) => client.ClientId == clientId)
                .AndWhere((HookEvent hookEvent) => hookEvent.Name == eventName)
                .DetachDelete("hookEvent");

            await query.ExecuteWithoutResultsAsync();
            return RestHookResult.SuccessResult;
        }

        public async Task<RestHookResult> DeleteConsumerHookEventAsync(string userId, 
            string clientId, string eventName)
        {
            Throw.ArgumentException.IfNullOrWhiteSpace(userId, nameof(userId));
            Throw.ArgumentException.IfNullOrWhiteSpace(clientId, nameof(clientId));
            Throw.ArgumentException.IfNullOrWhiteSpace(eventName, nameof(eventName));
            ThrowIfDisposed();

            var query = new CypherFluentQuery(GraphClient)
                .Match(
                    "((user:HookUser)-[:OWNS]->(client:HookClient))",
                    "((client:HookClient)-[:OWNS]->(hookUrl:HookUrl))", 
                    "((hookEvent:HookEvent)-[r:PUBLISHES_TO]->(hookUrl:HookUrl))")
                .Where((HookUser user) => user.UserId == userId)
                .AndWhere((HookClient client) => client.ClientId == clientId)
                .AndWhere((HookEvent hookEvent) => hookEvent.Name == eventName)
                .DetachDelete("r");

            await query.ExecuteWithoutResultsAsync();
            return RestHookResult.SuccessResult;
        }
    }
}