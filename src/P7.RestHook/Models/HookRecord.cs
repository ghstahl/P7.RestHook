namespace P7.RestHook.Models
{
    public class HookRecordQuery
    {
        public string ClientId { get; set; }
        public string EventName { get; set; }
    }
    public class HookRecord
    {
        public string ClientId { get; set; }
        public string CallbackUrl { get; set; }
        public bool ValidatedCallbackUrl { get; set; }
        public string EventName { get; set; }
    }
}