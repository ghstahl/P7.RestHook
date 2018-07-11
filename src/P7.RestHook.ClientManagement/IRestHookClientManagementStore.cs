using System.Collections.Generic;
using System.Threading.Tasks;
using P7.RestHook.ClientManagement.Models;
using P7.RestHook.Models;
using P7.RestHook.Store;


namespace P7.RestHook.ClientManagement
{
    public interface IRestHookClientManagementStore
    {
        /// <summary>
        /// Upserts a new hook client user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>a hook user client record</returns>
        Task<RestHookDataResult<HookUser>> UpsertHookUserAsync(string userId);

        /// <summary>
        /// Deletes a new hook client user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<RestHookResult> DeleteHookUserAsync(string userId);

        /// <summary>
        /// fetches a hook user 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>a hook user client record</returns>
        Task<RestHookDataResult<HookUser>> FindHookUserAsync(string userId);
        /// <summary>
        /// returns all clients for a given user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<RestHookDataResult<List<HookClient>>> FindHookUserClientsAsync(string userId);

        /// <summary>
        /// Finds a user client record
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<RestHookDataResult<HookClient>> FindHookClientAsync(string userId, string clientId);

        /// <summary>
        /// creates a new HookClientWithHookRecords
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
        /// Find all the HookEvent for a given client
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<RestHookDataResult<IEnumerable<HookEvent>>> FindHookEventsAsync(string userId, string clientId);

        Task<RestHookDataResult<IEnumerable<HookUrl>>> FindConsumerHookEventCallbackUrlsAsync(
            string userId, string clientId,string eventName);

        /// <summary>
        ///  Add a new event record to a client
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <param name="eventName"></param>
        /// <returns></returns>
        Task<RestHookResult> AddProducesHookEventAsync(
            string userId, string clientId,string eventName);

        /// <summary>
        /// adds a consumer to an event
        /// </summary>
        /// <param name="producerUserId"></param>
        /// <param name="producerClientId"></param>
        /// <param name="eventName"></param>
        /// <param name="consumerUserId"></param>
        /// <param name="consumerClientId"></param>
        /// <param name="callbackUrl"></param>
        /// <returns></returns>
        Task<RestHookResult> AddConsumerHookEventAsync(
            string producerUserId, string producerClientId, string eventName,
            string consumerUserId, string consumerClientId, string callbackUrl);
        
        /// <summary>
        /// Finds an event record
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <param name="eventName"></param>
        /// <returns></returns>
        Task<RestHookDataResult<HookEvent>> FindHookEventAsync(string userId, string clientId, string eventName);

 
        /// <summary>
        /// Delete an event record
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<RestHookResult> DeleteHookEventAsync(string userId, string clientId, string name);
        Task<RestHookResult> DeleteConsumerHookEventAsync(string userId, string clientId, string name);
        Task<RestHookDataResult<HookUrl>> FindHookUrlAsync(string url);
    }
}
