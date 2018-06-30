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
        private object Lock { get; set; }
        List<HookUserClientsRecord> _records;

        public InMemoryRestHookClientManagementStore()
        {
            Lock = new object();
            _records = new List<HookUserClientsRecord>();
        }
        public Task<HookUserClientsRecord> FindHookUserClientsAsync(string userId)
        {
            lock (Lock)
            {
                var record = _records.FirstOrDefault(item => item.UserId == userId);
                return Task.FromResult(record);
            }
        }

        public Task<HookUserClientsRecord> CreateHookUserClientAsync(string userId)
        {
            lock (Lock)
            {
                var record = new HookUserClientsRecord()
                {
                    UserId = userId,
                    Clients = new List<ClientRecord>()
                };
                _records.Add(record);
                return Task.FromResult(record);
            }
        }

        public Task<RestHookResult> DeleteHookUserClientAsync(string userId)
        {
            lock (Lock)
            {
                _records = _records.FindAll(x => x.UserId != userId);
                return Task.FromResult(RestHookResult.SuccessResult);
            }
        }

        public Task<RestHookResult> AddClientAsync(
            HookUserClientsRecord hookUserClientsRecord,
            ClientRecord clientRecord)
        {
            lock (Lock)
            {
                var cl = hookUserClientsRecord.Clients.FirstOrDefault(item => item.ClientId == clientRecord.ClientId);
                if (cl == null)
                {
                    hookUserClientsRecord.Clients.Add(clientRecord);
                }

                return Task.FromResult(RestHookResult.SuccessResult);
            }
        }

        public Task<RestHookResult> UpdateAsync(HookUserClientsRecord hookUserClientsRecord)
        {
            lock (Lock)
            {
                var original = FindHookUserClientsAsync(hookUserClientsRecord.UserId).GetAwaiter().GetResult();
                if (original == null)
                {
                    return Task.FromResult(new RestHookResult()
                    {
                        Success = false,
                        Error = new RestHookResultError()
                        {
                            ErrorCode = 1,
                            Message = "User record doesn't exist in the database"
                        }
                    });
                }

                original.Clients = hookUserClientsRecord.Clients;
                return Task.FromResult(RestHookResult.SuccessResult);
            }
        }

 
        public Task<RestHookResult> DeleteClientAsync(HookUserClientRecord hookUserClientRecord)
        {
            lock (Lock)
            {
                var original = FindHookUserClientsAsync(hookUserClientRecord.UserId).GetAwaiter().GetResult();
                if (original == null)
                {
                    return Task.FromResult(new RestHookResult()
                    {
                        Success = false,
                        Error = new RestHookResultError()
                        {
                            ErrorCode = 1,
                            Message = "User record doesn't exist in the database"
                        }
                    });
                }
                original.Clients = original.Clients.FindAll(x => x.ClientId != hookUserClientRecord.Client.ClientId);
                return Task.FromResult(RestHookResult.SuccessResult);
            }
        }
    }
}