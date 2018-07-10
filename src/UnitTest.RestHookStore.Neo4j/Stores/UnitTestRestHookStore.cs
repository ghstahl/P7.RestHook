using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P7.RestHook.Store;

namespace UnitTest.RestHookStore.Neo4j.Stores
{
    [TestClass]
    public class UnitTestRestHookStore: UnitTest.RestHookStore.Core.Stores.UnitTestRestHookStore
    {
        public UnitTestRestHookStore() : base(
            HostContainer.ServiceProvider.GetService<IRestHookStoreTest>(), 
            HostContainer.ServiceProvider.GetService<IRestHookStore>())
        {
        }
    }
}
