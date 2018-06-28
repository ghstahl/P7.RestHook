using System.Collections.Generic;

namespace P7.RestHook.ClientManagement.Models
{
    public class ClientRecord
    {
        public string ClientId { get; set; }
        public string Description { get; set; }
    }

    public class HookUserClientsRecord
    {
        public List<ClientRecord> Clients { get; set; }
        public string UserId { get; set; }
    }
    public class HookUserClientRecord
    {
        public ClientRecord Client { get; set; }
        public string UserId { get; set; }
    }
}