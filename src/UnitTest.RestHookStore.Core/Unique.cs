using System;
using P7.RestHook.ClientManagement.Models;

namespace UnitTest.RestHookStore.Core
{
    static class Unique
    {
        public static string G => Guid.NewGuid().ToString();
        public static string S => Guid.NewGuid().ToString("N");
        public static string Url => $"https://{S}.domain.com";
        public static HookClient HookClient => new HookClient() {ClientId = Unique.S, Description = Unique.S};
    }
}