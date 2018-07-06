using System.Collections.Generic;
using System.Threading.Tasks;
using P7.RestHook.ClientManagement;
using P7.RestHook.ClientManagement.Models;
using P7.RestHook.Models;
using P7.RestHook.Store;

namespace P7.RestHook.Neo4jStore
{
    public class Neo4jRestHookClientManagementStore : IRestHookClientManagementStore
    {
        public Task<RestHookDataResult<HookUser>> CreateHookUserAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<RestHookResult> DeleteHookUserAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<RestHookDataResult<HookUser>> FindHookUserAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<RestHookDataResult<HookClient>> FindHookClientAsync(string userId, string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<RestHookDataResult<HookClient>> CreateHookClientAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<RestHookResult> DeleteHookClientAsync(string userId, string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<RestHookDataResult<IEnumerable<HookRecord>>> FindHookRecordsAsync(string userId, string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<RestHookDataResult<HookRecord>> AddHookRecordAsync(string userId, HookRecord hookRecord)
        {
            throw new System.NotImplementedException();
        }

        public Task<RestHookResult> DeleteHookRecordAsync(string userId, string clientId, string hookRecordId)
        {
            throw new System.NotImplementedException();
        }

        public Task<RestHookDataResult<IEnumerable<EventRecord>>> FindEventRecordsAsync(string userId, string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<RestHookDataResult<EventRecord>> AddEventRecordAsync(string userId, EventRecord record)
        {
            throw new System.NotImplementedException();
        }

        public Task<RestHookResult> DeleteEventRecordAsync(string userId, string clientId, string name)
        {
            throw new System.NotImplementedException();
        }
    }
}