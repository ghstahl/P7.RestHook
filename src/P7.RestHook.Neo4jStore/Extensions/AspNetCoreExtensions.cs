using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using P7.RestHook.ClientManagement;
using P7.RestHook.Models;
using P7.RestHook.Store;

namespace P7.RestHook.Neo4jStore.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddNeo4jRestHook(this IServiceCollection services)
        {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<HookRecord, HookRecord>();
            });
            services.AddNeo4jRestHookOperationalStore();
            services.AddNeo4jRestHookUserClientManagementStore();
        }
        public static void AddNeo4jRestHookOperationalStore(this IServiceCollection services)
        {
            services.AddSingleton<IRestHookStore, Neo4JRestHookStore>();
            services.AddSingleton<IRestHookStoreTest, Neo4JRestHookStore>();
            
        }
        public static void AddNeo4jRestHookUserClientManagementStore(this IServiceCollection services)
        {
            services.AddSingleton<IRestHookClientManagementStoreTest, Neo4jRestHookClientManagementStore>();
            services.AddSingleton<IRestHookClientManagementStore, Neo4jRestHookClientManagementStore>();
        }
    }
}
