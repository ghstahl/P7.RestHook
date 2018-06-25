namespace P7.RestHook.Store
{
    public class RestHookStoreResult
    {
        public static RestHookStoreResult SuccessResult => new RestHookStoreResult() {Success = true};
        public bool Success { get; set; }
        public RestHookStoreResultError Error { get; set; }
    }
}