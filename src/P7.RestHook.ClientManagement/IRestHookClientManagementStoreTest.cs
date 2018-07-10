using System.Threading.Tasks;
using P7.RestHook.Store;

namespace P7.RestHook.ClientManagement
{
    public interface IRestHookClientManagementStoreTest
    {
        /// <summary>
        /// Drops the database
        /// </summary>
        /// <returns></returns>
        Task<RestHookResult> DropAsync();
    }
}