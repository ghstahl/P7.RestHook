using System;
using Microsoft.Extensions.DependencyInjection;
using Neo4jClient;
using P7.RestHook.Neo4jStore.Extensions;

namespace UnitTest.RestHookStore.Neo4j
{
    public static class HostContainer
    {
        private static ServiceProvider _serviceProvider;
        private static IGraphClient GetGraphClient()
        {
            var graphClient = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "password");
            graphClient.Connect();
            return graphClient;
        }
        public static ServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    var serviceCollection = new ServiceCollection()
                        .AddLogging();

                    serviceCollection.AddSingleton(GetGraphClient());
                    serviceCollection.AddInMemoryRestHookOperationalStore();

                    _serviceProvider = serviceCollection.BuildServiceProvider();
                }

                return _serviceProvider;
            }
        }
    }
}
