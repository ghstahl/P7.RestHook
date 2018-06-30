using System.Threading.Tasks;
using P7.RestHook.ClientManagement.Models;
using P7.RestHook.Models;
using P7.RestHook.Store;


namespace P7.RestHook.ClientManagement
{
    public interface IRestHookClientManagementStore
    {
        /// <summary>
        /// fetches a hook user client record
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>a hook user client record</returns>
        Task<RestHookDataResult<HookUserClientsRecord>> FindHookUserClientsAsync(string userId);

        /// <summary>
        /// Finds a user client record
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<RestHookDataResult<HookUserClientRecord>> FindHookUserClientRecordAsync(string userId, string clientId);

        /// <summary>
        /// deletes a hook user client record
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>a hook user client record</returns>
        Task<RestHookResult> DeleteHookUserClientRecordAsync(string userId, string clientId);

        Task<RestHookDataResult<HookRecord>> FindHookRecordsAsync(string userId, string clientId, string hookRecordId);

        

        /// <summary>
        /// Creates a new hook client user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>a hook user client record</returns>
        Task<RestHookDataResult<HookUserClientsRecord>> CreateHookUserClientAsync(string userId);

        /// <summary>
        /// Deletes a new hook client user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<RestHookResult> DeleteHookUserClientAsync(string userId);

        /// <summary>
        /// Adds a new clientid to a record
        /// </summary>
        /// <param name="hookUserClientsRecord"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<RestHookResult> AddClientAsync(HookUserClientsRecord hookUserClientsRecord,
            ClientRecord clientRecord);
        /// <summary>
        /// updates a record in permenant storage
        /// </summary>
        /// <param name="hookUserClientsRecord"></param>
        /// <returns></returns>
        Task<RestHookResult> UpdateAsync(HookUserClientsRecord hookUserClientsRecord);
 
    }
}
