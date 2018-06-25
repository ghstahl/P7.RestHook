﻿using System;
using System.Text;
using System.Threading.Tasks;
using P7.RestHook.Models;

namespace P7.RestHook.Store
{
    public interface IRestHookStore
    {
        /// <summary>
        /// Drops the database
        /// </summary>
        /// <returns></returns>
        Task<RestHookStoreResult> DropAsync();
        /// <summary>
        /// Insert or Update a hook
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        Task<RestHookStoreResult> UpsertAsync(HookRecord record);

        /// <summary>
        /// Deletes a hook record based upon the inputs.
        /// If you only pass ClientId all records for that ClientId will be deleted.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        Task<RestHookStoreResult> DeleteAsync(HookRecord record);

        /// <summary>
        /// Pages all HookRecords
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pagingState"></param>
        /// <returns>A Page Record</returns>
        Task<IPage<HookRecord>> PageAsync(int pageSize,
            byte[] pagingState);

        /// <summary>
        /// Pages all HookRecords, based upon an input present in hookRecordQuery
        /// </summary>
        /// <param name="hookRecordQuery"></param>
        /// <param name="pageSize"></param>
        /// <param name="pagingState"></param>
        /// <returns>A Page Record</returns>
        Task<IPage<HookRecord>> PageAsync(HookRecord hookRecordQuery,int pageSize,
            byte[] pagingState);

    }
}