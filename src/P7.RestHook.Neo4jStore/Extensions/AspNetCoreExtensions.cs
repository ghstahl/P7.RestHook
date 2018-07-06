using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using P7.RestHook.ClientManagement;
using P7.RestHook.Models;
using P7.RestHook.Store;

namespace P7.RestHook.Neo4jStore.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddInMemoryRestHook(this IServiceCollection services)
        {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<HookRecord, HookRecord>();
            });
            services.AddInMemoryRestHookOperationalStore();
            services.AddInMemoryRestHookUserClientManagementStore();
        }
        public static void AddInMemoryRestHookOperationalStore(this IServiceCollection services)
        {
            services.AddSingleton<IRestHookStore, Neo4JRestHookStore>();
        }
        public static void AddInMemoryRestHookUserClientManagementStore(this IServiceCollection services)
        {
            services.AddSingleton<IRestHookClientManagementStore, Neo4jRestHookClientManagementStore>();
        }
    }
}
