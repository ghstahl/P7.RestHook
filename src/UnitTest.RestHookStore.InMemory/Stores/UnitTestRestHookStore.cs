using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using P7.RestHook.Store;

namespace UnitTest.RestHookStore.InMemory.Stores
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
