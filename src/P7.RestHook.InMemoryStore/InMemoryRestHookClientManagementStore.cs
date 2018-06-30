using System;
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
 
        public Task<RestHookDataResult<ClientRecord>> FindClientRecordAsync(string userId, string clientId)
        {
            lock (Lock)
            {
                var original = FindHookUserClientsAsync(userId).GetAwaiter().GetResult();
                if (original == null)
                {
                    var errorResult = RestHookDataResult<ClientRecord>.FailedResult(new RestHookResultError());
                    return Task.FromResult(errorResult);
                }

                var record = original.Data;
                var client = record.Clients.FirstOrDefault(x => x.ClientId == clientId);
                if (client == null)
                {
                    var errorResult = RestHookDataResult<ClientRecord>.FailedResult(new RestHookResultError()
                    {
                        Message = $"Client:{clientId} does not exist for user:{userId}"
                    });
                    return Task.FromResult(errorResult);
                }
               
                return Task.FromResult(RestHookDataResult<ClientRecord>.SuccessResult(client));
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
                            Message = $"User:{userId} record doesn't exist in the database"
                        }
                    });
                }

                var record = original.Data;
                record.Clients = record.Clients.FindAll(x => x.ClientId != clientId);
                return Task.FromResult(RestHookResult.SuccessResult);
            }
        }

        public Task<RestHookDataResult<HookRecord>> FindHookRecordAsync(string userId, string clientId, string hookRecordId)
        {
            throw new System.NotImplementedException();
        }

        public Task<RestHookDataResult<HookRecord>> AddHookRecordAsync(string userId, HookRecord hookRecord)
        {
            var record = _records.FirstOrDefault(item => item.UserId == userId);
            RestHookDataResult<HookRecord> result = null;
            if (record == null)
            {
                result = RestHookDataResult<HookRecord>.FailedResult(new RestHookResultError()
                {
                    Message = $"User:{userId} record doesn't exist in the database"
                });
                return Task.FromResult(result);

            }
            var clientRecord = record.Clients.FirstOrDefault(item => item.ClientId == hookRecord.ClientId);
            if (clientRecord == null)
            {
                result = RestHookDataResult<HookRecord>.FailedResult(new RestHookResultError()
                {
                    ErrorCode = 1,
                    Message = $"{hookRecord.ClientId} does not exist for user:{userId}"
                });
                return Task.FromResult(result);
            }

            var foundHookRecord = clientRecord.HookRecords.FirstOrDefault(hr =>
                (hr.EventName == hookRecord.EventName && string.Compare(hr.CallbackUrl, hookRecord.CallbackUrl,
                     StringComparison.OrdinalIgnoreCase) == 0));
            if (foundHookRecord != null)
            {
                result = RestHookDataResult<HookRecord>.FailedResult(new RestHookResultError()
                {
                    ErrorCode = 1,
                    Message = $"{hookRecord.ClientId} already has this record"
                });
                return Task.FromResult(result);
            }
            var finalHookRecord = new HookRecord()
            {
                Id = Unique.G,
                CallbackUrl = hookRecord.CallbackUrl,
                ClientId = clientRecord.ClientId,
                EventName = hookRecord.EventName
            };

            clientRecord.HookRecords.Add(finalHookRecord);
            result = RestHookDataResult<HookRecord>.SuccessResult(finalHookRecord);
            return Task.FromResult(result);
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

        public Task<RestHookDataResult<ClientRecord>> CreateClientAsync(string userId)
        {
            lock (Lock)
            {
                var record = _records.FirstOrDefault(item => item.UserId == userId);
                RestHookDataResult<ClientRecord> result = null;
                if (record == null)
                {
                    result = RestHookDataResult<ClientRecord>.FailedResult(new RestHookResultError()
                    {
                        Message = $"User:{userId} record doesn't exist in the database"
                    });
                    return Task.FromResult(result);
                }
                var clientRecord = new ClientRecord()
                {
                    ClientId = Unique.G
                };
                record.Clients.Add(clientRecord);


                return Task.FromResult(RestHookDataResult<ClientRecord>.SuccessResult(clientRecord));
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
                            Message = $"User:{hookUserClientsRecord.UserId} record doesn't exist in the database"
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