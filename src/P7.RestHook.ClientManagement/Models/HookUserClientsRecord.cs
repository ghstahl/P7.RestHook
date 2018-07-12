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
        public List<HookClientWithHookRecords> HookClients { get; set; }
    }

    public class ProducerEvent
    {
        public string UserId { get; set; }
        public string ClientId { get; set; }
        public string EventName { get; set; }
    }

    public class ConsumerEvent
    {
        public string UserId { get; set; }
        public string ClientId { get; set; }
        public HookUrl HookUrl { get; set; }
    }

    public class HookUrl
    {
        public string Id { get; set;}
        public string Url { get; set; }
        public bool Validated { get; set; }
    }

    public class PublishRecord
    {
        public ProducerEvent ProducerEvent { get; set; }
        public ConsumerEvent ConsumerEvent { get; set; }
    }
    public class HookClient
    {
        public string ClientId { get; set; }
    }
    public class HookClientWithHookRecords
    {
        public HookClientWithHookRecords()
        {
            HookRecords = new List<HookRecord>();
        }
        public string ClientId { get; set; }
        public string Description { get; set; }
        public List<HookRecord> HookRecords { get; set; }
        public List<HookEvent> EventRecords { get; set; }
    }
    public class HookUser
    {
        public string UserId { get; set; }
    }
    public class HookUserWithClients
    {
        public List<HookClientWithHookRecords> Clients { get; set; }
        public string UserId { get; set; }
    }
    public class HookUserClientRecord
    {
        public HookClientWithHookRecords ClientWithHookRecords { get; set; }
        public string UserId { get; set; }
    }
}