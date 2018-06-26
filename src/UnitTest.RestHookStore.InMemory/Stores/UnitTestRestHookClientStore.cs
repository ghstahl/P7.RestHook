using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using P7.RestHook.ClientManagement;
using P7.RestHook.Store;

namespace UnitTest.RestHookStore.InMemory.Stores
{
    [TestClass]
    public class UnitTestRestHookClientStore : UnitTest.RestHookStore.Core.Stores.UnitTestRestHookClientStore
    {
        public UnitTestRestHookClientStore() : base(HostContainer.ServiceProvider.GetService<IRestHookClientManagementStore>())
        {
        }
    }
}
