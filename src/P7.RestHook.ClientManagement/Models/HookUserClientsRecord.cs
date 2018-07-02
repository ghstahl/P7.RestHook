using System.Collections.Generic;
using P7.RestHook.Models;

namespace P7.RestHook.ClientManagement.Models
{
    public class EventProducer
    {
        public List<string> AllowedClients { get; set; }
        public List<string> SupportedEvents { get; set; }
    }

    public class EventConsumer
    {
        public List<HookClient> HookClients { get; set; }
    }
    public class HookClient
    {
        public HookClient()
        {
            HookRecords = new List<HookRecord>();
        }
        public string ClientId { get; set; }
        public string Description { get; set; }
        public List<HookRecord> HookRecords { get; set; }
        public List<EventRecord> EventRecords { get; set; }
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