using System.Threading.Tasks;

namespace P7.RestHook.Store
{
    public interface IRestHookStoreTest
    {    
        /// <summary>
        /// Drops the database
        /// </summary>
        /// <returns></returns>
        Task<RestHookResult> DropAsync();
    }
}