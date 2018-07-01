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
        List<HookUser> _records;

        public InMemoryRestHookClientManagementStore()
        {
            Lock = new object();
            _records = new List<HookUser>();
        }
        public Task<RestHookDataResult<HookUser>> FindHookUserAsync(string userId)
        {
            lock (Lock)
            {
                var record = _records.FirstOrDefault(item => item.UserId == userId);
                return Task.FromResult(RestHookDataResult<HookUser>.SuccessResult(record));
            }
        }
 
        public Task<RestHookDataResult<HookClient>> FindHookClientAsync(string userId, string clientId)
        {
            lock (Lock)
            {
                var record = _records.FirstOrDefault(item => item.UserId == userId);
                RestHookDataResult<HookClient> result = null;
                if (record == null)
                {
                    result = RestHookDataResult<HookClient>.FailedResult(new RestHookResultError()
                    {
                        Message = $"User:{userId} record doesn't exist in the database"
                    });
                  
                    return Task.FromResult(result);

                }
                var clientRecord = record.Clients.FirstOrDefault(item => item.ClientId == clientId);
                if (clientRecord == null)
                {
                    result = RestHookDataResult<HookClient>.FailedResult(new RestHookResultError()
                    {
                        Message = $"{clientId} does not exist for user:{userId}"
                    });
                    return Task.FromResult(result);
                }

                return Task.FromResult(RestHookDataResult<HookClient>.SuccessResult(clientRecord));
            }
        }

        public Task<RestHookResult> DeleteHookClientAsync(string userId, string clientId)
        {
            lock (Lock)
            {
                var original = FindHookUserAsync(userId).GetAwaiter().GetResult();
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

        public Task<RestHookDataResult<IEnumerable<HookRecord>>> FindHookRecordsAsync(string userId, string clientId)
        {
            var clientRecordResult = FindHookClientAsync(userId, clientId).GetAwaiter().GetResult();
            if (!clientRecordResult.Success)
            {
                var result = RestHookDataResult<IEnumerable<HookRecord>>.FailedResult(clientRecordResult.Error);
                return Task.FromResult(result);
            }

            var dataResult =
                RestHookDataResult<IEnumerable<HookRecord>>.SuccessResult(clientRecordResult.Data.HookRecords);

            return Task.FromResult(dataResult);
        }

        public Task<RestHookResult> DeleteHookRecordAsync(
            string userId, 
            string clientId, 
            string hookRecordId)
        {
            RestHookDataResult<HookClient> clientRecordResult;
            clientRecordResult = FindHookClientAsync(userId, clientId).GetAwaiter().GetResult();
            if (!clientRecordResult.Success)
            {
                var result = RestHookResult.FailedResult(clientRecordResult.Error);
                return Task.FromResult(result);
            }

            var clientRecord = clientRecordResult.Data;
            clientRecord.HookRecords = clientRecord.HookRecords.FindAll(x => x.Id != hookRecordId);
            return Task.FromResult(RestHookResult.SuccessResult);
        }
        public Task<RestHookDataResult<HookRecord>> AddHookRecordAsync(string userId, HookRecord hookRecord)
        {
            RestHookDataResult<HookRecord> result;
            RestHookDataResult<HookClient> clientRecordResult;
            var clientId = hookRecord.ClientId;
            clientRecordResult = FindHookClientAsync(userId, clientId).GetAwaiter().GetResult();
            if (!clientRecordResult.Success)
            {
                result = RestHookDataResult<HookRecord>.FailedResult(clientRecordResult.Error);
                return Task.FromResult(result);
            }

          
            var clientRecord = clientRecordResult.Data;

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

     

        public Task<RestHookDataResult<HookUser>> CreateHookUserAsync(string userId)
        {
            lock (Lock)
            {
                var record = new HookUser()
                {
                    UserId = userId,
                    Clients = new List<HookClient>()
                };
                _records.Add(record);
                var result = RestHookDataResult<HookUser>.SuccessResult(record);
                return Task.FromResult(result);
            }
        }

        public Task<RestHookResult> DeleteHookUserAsync(string userId)
        {
            lock (Lock)
            {
                _records = _records.FindAll(x => x.UserId != userId);
                return Task.FromResult(RestHookResult.SuccessResult);
            }
        }

        public Task<RestHookDataResult<HookClient>> CreateHookClientAsync(string userId)
        {
            lock (Lock)
            {
                var record = _records.FirstOrDefault(item => item.UserId == userId);
                RestHookDataResult<HookClient> result = null;
                if (record == null)
                {
                    result = RestHookDataResult<HookClient>.FailedResult(new RestHookResultError()
                    {
                        Message = $"User:{userId} record doesn't exist in the database"
                    });
                    return Task.FromResult(result);
                }
                var clientRecord = new HookClient()
                {
                    ClientId = Unique.G
                };
                record.Clients.Add(clientRecord);


                return Task.FromResult(RestHookDataResult<HookClient>.SuccessResult(clientRecord));
            }
        }

        
    }
}