
using Microsoft.Extensions.DependencyInjection;
using P7.RestHook.InMemoryStore;
using P7.RestHook.InMemoryStore.Extensions;
using P7.RestHook.Store;

namespace UnitTest.RestHookStore.InMemory
{
    public static class HostContainer
    {
        private static ServiceProvider _serviceProvider;

        public static ServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    var serviceCollection = new ServiceCollection()
                        .AddLogging();

                    serviceCollection.AddInMemoryRestHook();
                    _serviceProvider = serviceCollection.BuildServiceProvider();
                }

                return _serviceProvider;
            }
        }
    }
}
