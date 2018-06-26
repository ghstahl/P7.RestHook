namespace P7.RestHook.Store
{
    public class RestHookResult
    {
        public static RestHookResult SuccessResult => new RestHookResult() {Success = true};
        public bool Success { get; set; }
        public RestHookResultError Error { get; set; }
    }
}