using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P7.RestHook.ClientManagement;

namespace UnitTest.RestHookStore.Neo4j.Stores
{
    [TestClass]
    public class UnitTestRestHookClientStore : UnitTest.RestHookStore.Core.Stores.UnitTestRestHookClientStore
    {
        public UnitTestRestHookClientStore() :
            base(
                HostContainer.ServiceProvider.GetService<IRestHookClientManagementStoreTest>(),
                HostContainer.ServiceProvider.GetService<IRestHookClientManagementStore>()
            )
        {
        }
    }
}
