using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using P7.RestHook.ClientManagement;
using P7.RestHook.Store;

namespace P7.RestHook.InMemoryStore.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static void AddInMemoryRestHook(this IServiceCollection services)
        {
            services.AddSingleton<IRestHookStore, InMemoryRestHookStore>();
            services.AddSingleton<IRestHookClientManagementStore, InMemoryRestHookClientManagementStore>();
        }
    }
}
