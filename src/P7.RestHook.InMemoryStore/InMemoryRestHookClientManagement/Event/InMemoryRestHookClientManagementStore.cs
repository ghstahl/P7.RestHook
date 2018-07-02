using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P7.RestHook.ClientManagement;
using P7.RestHook.ClientManagement.Models;
using P7.RestHook.Models;
using P7.RestHook.Store;

namespace P7.RestHook.InMemoryStore
{
    public partial class InMemoryRestHookClientManagementStore : IRestHookClientManagementStore
    {
        private RestHookDataResult<T> SuccessResult<T>(T data) where T:class
        {
            return RestHookDataResult<T>.SuccessResult(data);
        }
        private RestHookDataResult<T> FailedResult<T>(RestHookResultError error) where T : class
        {
            return RestHookDataResult<T>.FailedResult(error);
        }
        private RestHookResult SuccessResult()
        {
            return RestHookResult.SuccessResult;
        }
        private RestHookResult FailedResult(RestHookResultError error)
        {
            return RestHookResult.FailedResult(error);
        }
    }

    public partial class InMemoryRestHookClientManagementStore : IRestHookClientManagementStore
    {
        public Task<RestHookDataResult<IEnumerable<EventRecord>>> FindEventRecordsAsync(string userId, string clientId)
        {
            var clientRecordResult = FindHookClientAsync(userId, clientId).GetAwaiter().GetResult();
            if (!clientRecordResult.Success)
            {
                var result = FailedResult<IEnumerable<EventRecord>>(clientRecordResult.Error);
                return Task.FromResult(result);
            }

            var clientRecord = clientRecordResult.Data;
            var dataResult =
                SuccessResult<IEnumerable<EventRecord>>(clientRecordResult.Data.EventRecords);

            return Task.FromResult(dataResult);
        }

        public Task<RestHookDataResult<EventRecord>> AddEventRecordAsync(string userId, EventRecord record)
        {
            RestHookDataResult<EventRecord> result;
            RestHookDataResult<HookClient> clientRecordResult;
            var name = record.Name;
            var clientId = record.ClientId;
            clientRecordResult = FindHookClientAsync(userId, clientId).GetAwaiter().GetResult();
            if (!clientRecordResult.Success)
            {
                result = FailedResult<EventRecord>(clientRecordResult.Error);
                return Task.FromResult(result);
            }


            var clientRecord = clientRecordResult.Data;




            var foundHookRecord = clientRecord.EventRecords.FirstOrDefault(hr =>
                (hr.Name == record.Name));
            if (foundHookRecord != null)
            {
                result = FailedResult<EventRecord>(new RestHookResultError()
                {
                    ErrorCode = 1,
                    Message = $"{record.ClientId} already has this record"
                });
                return Task.FromResult(result);
            }


            clientRecord.EventRecords.Add(record);
            result = SuccessResult(record);
            return Task.FromResult(result);
        }

        public Task<RestHookResult> DeleteEventRecordAsync(string userId, string clientId, string name)
        {
            RestHookDataResult<HookClient> clientRecordResult;
            clientRecordResult = FindHookClientAsync(userId, clientId).GetAwaiter().GetResult();
            if (!clientRecordResult.Success)
            {
                var result = RestHookResult.FailedResult(clientRecordResult.Error);
                return Task.FromResult(result);
            }

            var clientRecord = clientRecordResult.Data;
            clientRecord.EventRecords = clientRecord.EventRecords.FindAll(x => x.Name != name);
            return Task.FromResult(SuccessResult());
        }
    }
}
