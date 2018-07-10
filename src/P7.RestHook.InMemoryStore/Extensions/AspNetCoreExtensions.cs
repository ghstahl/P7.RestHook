using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using P7.RestHook.ClientManagement;
using P7.RestHook.Models;
using P7.RestHook.Store;

namespace P7.RestHook.InMemoryStore.Extensions
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
            services.AddSingleton<InMemoryRestHookStore>();

            services.AddSingleton<IRestHookStore>(p =>
            {
                return (IRestHookStore) p.GetService(typeof(InMemoryRestHookStore));
            });
            services.AddSingleton<IRestHookStoreTest>(p =>
            {
                return (IRestHookStoreTest) p.GetService(typeof(InMemoryRestHookStore));
            });

        }

        public static void AddInMemoryRestHookUserClientManagementStore(this IServiceCollection services)
        {
            services.AddSingleton<InMemoryRestHookClientManagementStore>();

            services.AddSingleton<IRestHookClientManagementStore>(p =>
            {
                return (IRestHookClientManagementStore)p.GetService(typeof(InMemoryRestHookClientManagementStore));
            });
            services.AddSingleton<IRestHookClientManagementStoreTest>(p =>
            {
                return (IRestHookClientManagementStoreTest)p.GetService(typeof(InMemoryRestHookClientManagementStore));
            });
        }
    }
}
