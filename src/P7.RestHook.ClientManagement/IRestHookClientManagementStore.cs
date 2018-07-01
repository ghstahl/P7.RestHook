﻿using System.Collections.Generic;
using System.Threading.Tasks;
using P7.RestHook.ClientManagement.Models;
using P7.RestHook.Models;
using P7.RestHook.Store;


namespace P7.RestHook.ClientManagement
{
    public interface IRestHookClientManagementStore
    {
        /// <summary>
        /// Creates a new hook client user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>a hook user client record</returns>
        Task<RestHookDataResult<HookUser>> CreateHookUserAsync(string userId);

        /// <summary>
        /// Deletes a new hook client user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<RestHookResult> DeleteHookUserAsync(string userId);
        /// <summary>
        /// fetches a hook user client record
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>a hook user client record</returns>
        Task<RestHookDataResult<HookUser>> FindHookUserAsync(string userId);

        /// <summary>
        /// Finds a user client record
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<RestHookDataResult<HookClient>> FindHookClientAsync(string userId, string clientId);

        /// <summary>
        /// creates a new HookClient
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<RestHookDataResult<HookClient>> CreateHookClientAsync(string userId);

        /// <summary>
        /// deletes a hook user client record
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<RestHookResult> DeleteHookClientAsync(string userId, string clientId);

        /// <summary>
        /// Find a HookRecords by ClientId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<RestHookDataResult<IEnumerable<HookRecord>>> FindHookRecordsAsync(string userId, string clientId);

        /// <summary>
        /// Adds a HookRecord
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hookRecord"></param>
        /// <returns></returns>
        Task<RestHookDataResult<HookRecord>> AddHookRecordAsync(string userId, HookRecord hookRecord);

        /// <summary>
        /// Deletes a HookRecord
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <param name="hookRecordId"></param>
        /// <returns></returns>
        Task<RestHookResult> DeleteHookRecordAsync(string userId, string clientId, string hookRecordId);


        


        /// <summary>
        /// updates a record in permenant storage
        /// </summary>
        /// <param name="hookUserClientsRecord"></param>
        /// <returns></returns>
        Task<RestHookResult> UpdateAsync(HookUser hookUserClientsRecord);
 
    }
}
