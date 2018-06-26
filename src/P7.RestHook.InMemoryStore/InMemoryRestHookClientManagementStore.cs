using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P7.RestHook.ClientManagement;
using P7.RestHook.ClientManagement.Models;
using P7.RestHook.Store;

namespace P7.RestHook.InMemoryStore
{
    public class InMemoryRestHookClientManagementStore : IRestHookClientManagementStore
    {
        List<HookUserClientsRecord> _records;

        public InMemoryRestHookClientManagementStore()
        {
            _records = new List<HookUserClientsRecord>();
        }
        public Task<HookUserClientsRecord> FindHookUserClientAsync(string userId)
        {
            var record = _records.FirstOrDefault(item => item.UserId == userId);
            return record == null ? CreateHookUserClientAsync(userId) : Task.FromResult(record);
        }

        public Task<HookUserClientsRecord> CreateHookUserClientAsync(string userId)
        {
            var record = new HookUserClientsRecord()
            {
                UserId = userId,
                Clients = new List<string>()
            };
            _records.Add(record);
            return Task.FromResult(record);
        }

        public Task<RestHookResult> AddClientAsync(HookUserClientsRecord hookUserClientsRecord, 
            string clientId)
        {
            var cl = hookUserClientsRecord.Clients.FirstOrDefault(item => item == clientId);
            if (cl == null)
            {
                hookUserClientsRecord.Clients.Add(clientId);
            }

            return Task.FromResult(RestHookResult.SuccessResult);
        }

        public Task<RestHookResult> UpdateAsync(HookUserClientsRecord hookUserClientsRecord)
        {
            var original = FindHookUserClientAsync(hookUserClientsRecord.UserId).GetAwaiter().GetResult();
            if (original == null)
            {
                _records.Add(hookUserClientsRecord);
            }

            original.Clients = hookUserClientsRecord.Clients;
            return Task.FromResult(RestHookResult.SuccessResult);
        }
    }
}