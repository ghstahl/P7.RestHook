﻿using System;
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
            services.AddSingleton<IRestHookStore, InMemoryRestHookStore>();
        }
        public static void AddInMemoryRestHookUserClientManagementStore(this IServiceCollection services)
        {
            services.AddSingleton<IRestHookClientManagementStore, InMemoryRestHookClientManagementStore>();
        }
    }
}
