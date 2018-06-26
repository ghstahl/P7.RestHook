using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using P7.RestHook.Models;
using P7.RestHook.Store;

namespace P7.RestHook.InMemoryStore
{
    public class InMemoryRestHookStore: IRestHookStore
    {
        private List<HookRecord> _records;
        private static readonly object Lock = new object();
        public InMemoryRestHookStore()
        {
            _records = new List<HookRecord>();
        }

        public Task<RestHookStoreResult> DropAsync()
        {
            lock (Lock)
            {
                _records = new List<HookRecord>();
                return Task.FromResult(RestHookStoreResult.SuccessResult);
            }
        }

        public Task<RestHookStoreResult> UpsertAsync(HookRecord record)
        {
            lock (Lock)
            {
                if (record == null ||
                    string.IsNullOrWhiteSpace(record.ClientId) ||
                    string.IsNullOrWhiteSpace(record.EventName) ||
                    string.IsNullOrWhiteSpace(record.CallbackUrl))
                {
                    return Task.FromResult(new RestHookStoreResult()
                    {
                        Success = false,
                        Error = new RestHookStoreResultError()
                        {
                            ErrorCode = 1,
                            Message = "Input argument is bad"
                        }
                    });
                }

                var query = from item in _records
                    where item.EventName == record.EventName && item.ClientId == record.ClientId
                    select item;
                var foundRecord = query.FirstOrDefault();
                if (foundRecord == null)
                {
                    _records.Add(record);
                }
                else
                {
                    if (string.Compare(foundRecord.CallbackUrl, record.CallbackUrl,
                            StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        foundRecord.CallbackUrl = record.CallbackUrl;
                        foundRecord.ValidatedCallbackUrl = false;
                    }
                }
                return Task.FromResult(RestHookStoreResult.SuccessResult);
            }
        }
  

        public Task<RestHookStoreResult> DeleteAsync(HookRecord record)
        {
            lock (Lock)
            {
                if (record == null
                    ||
                    (string.IsNullOrWhiteSpace(record.ClientId) &&
                     string.IsNullOrWhiteSpace(record.EventName)))
                {
                    return Task.FromResult(new RestHookStoreResult()
                    {
                        Success = false,
                        Error = new RestHookStoreResultError()
                        {
                            ErrorCode = 1,
                            Message = "Input argument is bad"
                        }
                    });
                }

                if (record.ClientId != null || record.EventName != null)
                {
                    if (record.ClientId == null && record.EventName != null)
                    {
                        _records = _records.Where(x => x.EventName != record.EventName).ToList();

                    }
                    else if (record.ClientId != null && record.EventName == null)
                    {
                        _records = _records.Where(x => x.ClientId != record.ClientId).ToList();
                    }

                    if (record.ClientId != null && record.EventName != null)
                    {
                        _records = _records
                            .Where(x => (x.ClientId != record.ClientId && x.EventName != record.EventName)).ToList();
                    }
                }

                return Task.FromResult(RestHookStoreResult.SuccessResult);
            }
        }

        public async Task<IPage<HookRecord>> PageAsync(int pageSize, byte[] pagingState)
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
                return page;
            }
        }

        public async Task<IPage<HookRecord>> PageAsync(HookRecordQuery hookRecordQuery, int pageSize, byte[] pagingState)
        {
            lock (Lock)
            {
                if (hookRecordQuery == null
                    ||
                    (string.IsNullOrWhiteSpace(hookRecordQuery.ClientId) &&
                     string.IsNullOrWhiteSpace(hookRecordQuery.EventName)))
                {
                    return new Page<HookRecord>(null, null, new List<HookRecord>());
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
                return page;
            }
        }
    }
}
