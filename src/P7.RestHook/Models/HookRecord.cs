namespace P7.RestHook.Models
{
    public class HookRecordQuery
    {
        public string ClientId { get; set; }
        public string EventName { get; set; }
    }
    public partial class HookRecord
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string CallbackUrl { get; set; }
        public bool ValidatedCallbackUrl { get; set; }
        public string EventName { get; set; }
    }
}