using System.Threading.Tasks;
using P7.RestHook.ClientManagement.Models;
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
        Task<HookUserClientsRecord> FindHookUserClientAsync(string userId);

        /// <summary>
        /// Creates a new hook client user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>a hook user client record</returns>
        Task<HookUserClientsRecord> CreateHookUserClientAsync(string userId);

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
