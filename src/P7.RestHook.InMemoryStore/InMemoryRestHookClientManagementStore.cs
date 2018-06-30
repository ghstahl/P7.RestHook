using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P7.RestHook.ClientManagement;
using P7.RestHook.ClientManagement.Models;
using P7.RestHook.Models;
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
        public Task<RestHookDataResult<HookUserClientsRecord>> FindHookUserClientsAsync(string userId)
        {
            lock (Lock)
            {
                var record = _records.FirstOrDefault(item => item.UserId == userId);
                return Task.FromResult(RestHookDataResult<HookUserClientsRecord>.SuccessResult(record));
            }
        }

        public Task<RestHookDataResult<HookUserClientRecord>> FindHookUserClientRecordAsync(string userId, string clientId)
        {
            lock (Lock)
            {
                var original = FindHookUserClientsAsync(userId).GetAwaiter().GetResult();
                if (original == null)
                {
                    var errorResult = RestHookDataResult<HookUserClientRecord>.FailedResult(new RestHookResultError());
                    return Task.FromResult(errorResult);
                }

                var record = original.Data;
                var client = record.Clients.FirstOrDefault(x => x.ClientId == clientId);
                return Task.FromResult(
                    RestHookDataResult<HookUserClientRecord>.SuccessResult(new HookUserClientRecord()
                    {
                        UserId = userId,
                        Client = client
                    }));
            }
        }

        public Task<RestHookResult> DeleteHookUserClientRecordAsync(string userId, string clientId)
        {
            lock (Lock)
            {
                var original = FindHookUserClientsAsync(userId).GetAwaiter().GetResult();
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

                var record = original.Data;
                record.Clients = record.Clients.FindAll(x => x.ClientId != clientId);
                return Task.FromResult(RestHookResult.SuccessResult);
            }
        }

        public Task<RestHookDataResult<HookRecord>> FindHookRecordsAsync(string userId, string clientId, string hookRecordId)
        {
            throw new System.NotImplementedException();
        }

        public Task<RestHookDataResult<HookUserClientsRecord>> CreateHookUserClientAsync(string userId)
        {
            lock (Lock)
            {
                var record = new HookUserClientsRecord()
                {
                    UserId = userId,
                    Clients = new List<ClientRecord>()
                };
                _records.Add(record);
                var result = RestHookDataResult<HookUserClientsRecord>.SuccessResult(record);
                return Task.FromResult(result);
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
                if (original == null || original.Data == null)
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

                var record = original.Data;
                record.Clients = hookUserClientsRecord.Clients;
                return Task.FromResult(RestHookResult.SuccessResult);
            }
        }
    }
}