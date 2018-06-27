using System;
using P7.RestHook.ClientManagement.Models;

namespace UnitTest.RestHookStore.Core
{
    static class Unique
    {
        public static string S => Guid.NewGuid().ToString("N");
        public static string Url => $"https://{S}.domain.com";
        public static ClientRecord ClientRecord => new ClientRecord() {ClientId = Unique.S, Description = Unique.S};
    }
}