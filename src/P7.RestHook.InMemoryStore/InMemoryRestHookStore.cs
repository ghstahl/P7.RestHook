using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using P7.RestHook.Models;
using P7.RestHook.Store;

namespace P7.RestHook.InMemoryStore
{
    public class InMemoryRestHookStore: IRestHookStore, IRestHookStoreTest
    {
        private List<HookRecord> _records;
        private static readonly object Lock = new object();
        public InMemoryRestHookStore()
        {
            _records = new List<HookRecord>();
        }

        public Task<RestHookResult> DropAsync()
        {
            lock (Lock)
            {
                _records = new List<HookRecord>();
                return Task.FromResult(RestHookResult.SuccessResult);
            }
        }

        public Task<RestHookResult> UpsertAsync(HookRecord record)
        {
            lock (Lock)
            {
                if (record == null ||
                    string.IsNullOrWhiteSpace(record.Id) ||
                    string.IsNullOrWhiteSpace(record.ClientId) ||
                    string.IsNullOrWhiteSpace(record.EventName) ||
                    string.IsNullOrWhiteSpace(record.CallbackUrl))
                {
                    return Task.FromResult(new RestHookResult()
                    {
                        Success = false,
                        Error = new RestHookResultError()
                        {
                            ErrorCode = 1,
                            Message = "Input argument is bad"
                        }
                    });
                }
             
                var query = from item in _records
                    where item.Id == record.Id 
                    select item;
                var foundRecord = query.FirstOrDefault();
                if (foundRecord == null)
                {
                    var recordDto = Mapper.Map<HookRecord>(record);
                    _records.Add(recordDto);
                }
                else
                {
                    if (string.Compare(foundRecord.CallbackUrl, record.CallbackUrl,
                            StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        foundRecord.CallbackUrl = record.CallbackUrl;
                        foundRecord.ValidatedCallbackUrl = false;
                    }

                    foundRecord.ClientId = record.ClientId;
                    foundRecord.EventName = record.EventName;
                    
                }
                return Task.FromResult(RestHookResult.SuccessResult);
            }
        }

        public Task<RestHookDataResult<HookRecord>> FindByIdAsync(string id)
        {
            lock (Lock)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Task.FromResult(
                        RestHookDataResult<HookRecord>.FailedResult(
                            new RestHookResultError()
                        {
                            ErrorCode = 1,
                            Message = "Input argument is bad"
                        }));
                }

                var record = _records.FirstOrDefault(x => x.Id == id);
                return Task.FromResult(RestHookDataResult<HookRecord>.SuccessResult(record));
            }
        }

        public Task<RestHookResult> DeleteAsync(HookRecord record)
        {
            lock (Lock)
            {
                if (record == null || string.IsNullOrWhiteSpace(record.ClientId))
                {
                    return Task.FromResult(
                        RestHookResult.FailedResult(new RestHookResultError()
                        {
                            ErrorCode = 1,
                            Message = "Input argument is bad"
                        }));
                }


                if (record.Id != null)
                {
                    _records = _records.Where(x => x.Id != record.Id).ToList();
                }
                else if (record.ClientId == null && record.EventName != null)
                {
                    _records = _records.Where(x => x.EventName != record.EventName).ToList();
                }
                else if (record.ClientId != null && record.EventName == null)
                {
                    _records = _records.Where(x => x.ClientId != record.ClientId).ToList();
                }
                else if (record.ClientId != null && record.EventName != null)
                {
                    _records =
                        (_records.Where(r => !(r.ClientId == record.ClientId && r.EventName == record.EventName)))
                        .ToList();
                }


                return Task.FromResult(RestHookResult.SuccessResult);
            }
        }

        public async Task<RestHookDataResult<IPage<HookRecord>>> PageAsync(int pageSize, byte[] pagingState)
        {
            lock (Lock)
            {
                byte[] currentPagingState = pagingState;
                var ps = pagingState.DeserializePageState();

                var q = from x in (_records.Skip(ps.CurrentIndex).Take(pageSize))
                    select x;
                var slice = q.ToList();
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

        public async Task<RestHookDataResult<IPage<HookRecord>>> PageAsync(HookRecordQuery hookRecordQuery, int pageSize, byte[] pagingState)
        {
            lock (Lock)
            {
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
                    var q = (from x in _records
                        where x.ClientId == hookRecordQuery.ClientId
                        select x).Skip(ps.CurrentIndex).Take(pageSize);
                    slice = q.ToList();
                }
                else if (string.IsNullOrWhiteSpace(hookRecordQuery.ClientId) &&
                         !string.IsNullOrWhiteSpace(hookRecordQuery.EventName))
                {
                    var q = (from x in _records
                        where x.EventName == hookRecordQuery.EventName
                        select x).Skip(ps.CurrentIndex).Take(pageSize);
                    slice = q.ToList();

                }
                else if (!string.IsNullOrWhiteSpace(hookRecordQuery.ClientId) &&
                         !string.IsNullOrWhiteSpace(hookRecordQuery.EventName))
                {
                    var q = (from x in _records
                        where x.ClientId == hookRecordQuery.ClientId && x.EventName == hookRecordQuery.EventName
                        select x).Skip(ps.CurrentIndex).Take(pageSize);
                    slice = q.ToList();

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
}
