using System.Collections.Generic;
using P7.RestHook.Models;

namespace P7.RestHook.ClientManagement.Models
{
    public class HookClient
    {
        public HookClient()
        {
            HookRecords = new List<HookRecord>();
        }
        public string ClientId { get; set; }
        public string Description { get; set; }
        public List<HookRecord> HookRecords { get; set; }
    }

    public class HookUser
    {
        public List<HookClient> Clients { get; set; }
        public string UserId { get; set; }
    }
    public class HookUserClientRecord
    {
        public HookClient Client { get; set; }
        public string UserId { get; set; }
    }
}