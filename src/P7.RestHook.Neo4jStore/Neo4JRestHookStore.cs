using System;
using System.Threading.Tasks;
using Neo4jClient;
using P7.RestHook.Models;
using P7.RestHook.Store;

namespace P7.RestHook.Neo4jStore
{
    public class Neo4JRestHookStore : IRestHookStore
    {
        private IGraphClient _graphClient;

        public Neo4JRestHookStore(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }
        public Task<RestHookResult> DropAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RestHookResult> UpsertAsync(HookRecord record)
        {
            throw new NotImplementedException();
        }

        public Task<RestHookResult> DeleteAsync(HookRecord record)
        {
            throw new NotImplementedException();
        }

        public Task<RestHookDataResult<IPage<HookRecord>>> PageAsync(int pageSize, byte[] pagingState)
        {
            throw new NotImplementedException();
        }

        public Task<RestHookDataResult<IPage<HookRecord>>> PageAsync(HookRecordQuery hookRecordQuery, int pageSize, byte[] pagingState)
        {
            throw new NotImplementedException();
        }
    }
}
