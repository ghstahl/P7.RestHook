using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;
using Neo4jClient.Cypher;
using P7.RestHook.Models;
using P7.RestHook.Store;

namespace P7.RestHook.Neo4jStore
{
    public class Neo4JRestHookStore : 
        BaseStore, 
        IRestHookStore,
        IRestHookStoreTest
    {
        public Neo4JRestHookStore(IGraphClient graphClient) :
            base(graphClient)
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

        public async Task<RestHookDataResult<HookRecord>> FindByIdAsync(string id)
        {
            Throw.ArgumentException.IfNull(id, nameof(id));
            ThrowIfDisposed();

            var query = new CypherFluentQuery(GraphClient)
                .Match($"(r:HookRecord)")
                .Where((HookRecord r) => r.Id == id)
                .Return(r => r.As<HookRecord>());

            var result = (await query.ResultsAsync).SingleOrDefault();
            return RestHookDataResult<HookRecord>.SuccessResult(result);
        }

        public async Task<RestHookResult> UpsertAsync(HookRecord record)
        {
            var found = await FindByIdAsync(record.Id);
            if (found.Data == null)
            {
                var query = new CypherFluentQuery(GraphClient)
                    .Merge("(n:HookRecord { Id: {id} })")
                    .OnCreate()
                    .Set("n = {record}")
                    .WithParams(new
                    {
                        id = record.Id,
                        record
                    });
                await query.ExecuteWithoutResultsAsync();
            }
            else
            {
                var query = new CypherFluentQuery(GraphClient)
                    .Match("(r:HookRecord )")
                    .Where((HookRecord r) => r.Id == record.Id)
                    .Set("r = {record}")
                    .WithParams(new
                    {

                        record
                    });
                await query.ExecuteWithoutResultsAsync();
            }


            return RestHookResult.SuccessResult;
        }

        public async Task<RestHookResult> DeleteAsync(HookRecord record)
        {
            Throw.ArgumentException.IfNull(record, nameof(record));
            ThrowIfDisposed();

            if (record == null || string.IsNullOrWhiteSpace(record.ClientId))
            {
                return 
                    RestHookResult.FailedResult(
                        new RestHookResultError()
                    {
                       
                        Message = $"{nameof(record.ClientId)} is null"
                    });
            }

            var query = new CypherFluentQuery(GraphClient)
                .Match($"(r:HookRecord)")
                .Where((HookRecord r) => r.ClientId == record.ClientId)
                .DetachDelete("r");

            await query.ExecuteWithoutResultsAsync();
            return RestHookResult.SuccessResult;
        }

        public async Task<RestHookDataResult<IPage<HookRecord>>>
            PageAsync(int pageSize, byte[] pagingState)
        {
            /*
                Match(n:HookRecord) RETURN n.ClientId
                ORDER BY n.id
                SKIP 2
                LIMIT 2
             */
            if (pageSize == 0)
            {
                var result = RestHookDataResult<IPage<HookRecord>>.FailedResult(new RestHookResultError()
                {
                    Message = $"{nameof(pageSize)} argument is bad"
                });
                return result;
            }

            byte[] currentPagingState = pagingState;
            var ps = pagingState.DeserializePageState();

            var query = new CypherFluentQuery(GraphClient)
                .Match($"(r:HookRecord)")
                .Return((r) => new
                {
                    r = r.As<HookRecord>(),
                    id = r.Id()
                })
                .OrderBy("id")
                .Skip(ps.CurrentIndex)
                .Limit(pageSize);

            var queryResult = (await query.ResultsAsync);

            var qq = from item in queryResult
                let c = item.r
                select c;
            var finalList = qq.ToList();
            if (finalList.Count() < pageSize)
            {
                // we are at the end
                pagingState = null;
            }
            else
            {
                ps.CurrentIndex += pageSize;
                pagingState = ps.Serialize();
            }

            var page = new Page<HookRecord>(
                currentPagingState, pagingState, finalList);
            return RestHookDataResult<IPage<HookRecord>>
                .SuccessResult(page);

        }

        public async Task<RestHookDataResult<IPage<HookRecord>>>
            PageAsync(
                HookRecordQuery hookRecordQuery,
                int pageSize,
                byte[] pagingState)
        {
            if (pageSize == 0)
            {
                var result = RestHookDataResult<IPage<HookRecord>>.FailedResult(new RestHookResultError()
                {
                    Message = $"{nameof(pageSize)} argument is bad"
                });
                return result;
            }

            if (hookRecordQuery == null
                ||
                (string.IsNullOrWhiteSpace(hookRecordQuery.ClientId) &&
                 string.IsNullOrWhiteSpace(hookRecordQuery.EventName)))
            {
                return RestHookDataResult<IPage<HookRecord>>.FailedResult(new RestHookResultError()
                {
                    Message = $"{nameof(hookRecordQuery.ClientId)} and {nameof(hookRecordQuery.EventName)} is null"
                });
            }

            byte[] currentPagingState = pagingState;
            var ps = pagingState.DeserializePageState();
            List<HookRecord> slice = new List<HookRecord>();

            if (!string.IsNullOrWhiteSpace(hookRecordQuery.ClientId) &&
                string.IsNullOrWhiteSpace(hookRecordQuery.EventName))
            {
                var query = new CypherFluentQuery(GraphClient)
                    .Match($"(r:HookRecord)")
                    .Where((HookRecord r) => r.ClientId == hookRecordQuery.ClientId)
                    .Return((r) => new
                    {
                        r = r.As<HookRecord>(),
                        id = r.Id()
                    })
                    .OrderBy("id")
                    .Skip(ps.CurrentIndex)
                    .Limit(pageSize);

                var queryResult = (await query.ResultsAsync);
                var qq = from item in queryResult
                    let c = item.r
                    select c;
                slice = qq.ToList();
            }
            else if (string.IsNullOrWhiteSpace(hookRecordQuery.ClientId) &&
                     !string.IsNullOrWhiteSpace(hookRecordQuery.EventName))
            {
                var query = new CypherFluentQuery(GraphClient)
                    .Match($"(r:HookRecord)")
                    .Where((HookRecord r) => r.EventName == hookRecordQuery.EventName)
                    .Return((r) => new
                    {
                        r = r.As<HookRecord>(),
                        id = r.Id()
                    })
                    .OrderBy("id")
                    .Skip(ps.CurrentIndex)
                    .Limit(pageSize);

                var queryResult = (await query.ResultsAsync);
                var qq = from item in queryResult
                    let c = item.r
                    select c;
                slice = qq.ToList();
            }
            else if (!string.IsNullOrWhiteSpace(hookRecordQuery.ClientId) &&
                     !string.IsNullOrWhiteSpace(hookRecordQuery.EventName))
            {
                var query = new CypherFluentQuery(GraphClient)
                    .Match($"(r:HookRecord)")
                    .Where((HookRecord r) =>
                        r.ClientId == hookRecordQuery.ClientId &&
                        r.EventName == hookRecordQuery.EventName)
                    .Return((r) => new
                    {
                        r = r.As<HookRecord>(),
                        id = r.Id()
                    })
                    .OrderBy("id")
                    .Skip(ps.CurrentIndex)
                    .Limit(pageSize);

                var queryResult = (await query.ResultsAsync);
                var qq = from item in queryResult
                    let c = item.r
                    select c;
                slice = qq.ToList();
            }

            if (slice.Count < pageSize)
            {
                // we are at the end
                pagingState = null;
            }
            else
            {
                ps.CurrentIndex += pageSize;
                pagingState = ps.Serialize();
            }

            var page = new Page<HookRecord>(currentPagingState, pagingState, slice);
            return RestHookDataResult<IPage<HookRecord>>.SuccessResult(page);

          
        }
    }
}
