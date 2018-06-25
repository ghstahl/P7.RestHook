namespace P7.RestHook.Models
{
    public class HookRecord
    {
        public string ClientId { get; set; }
        public string CallbackUrl { get; set; }
        public bool ValidatedCallbackUrl { get; set; }
        public string EventName { get; set; }
    }
}