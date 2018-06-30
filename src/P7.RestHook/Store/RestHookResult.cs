using System.Runtime.InteropServices.ComTypes;

namespace P7.RestHook.Store
{
    public class NullData { }
    public class RestHookResult : RestHookDataResult<NullData>
    {
        public static RestHookResult SuccessResult => new RestHookResult()
        {
            Success = true
        };

        public static RestHookResult FailedResult(RestHookResultError error) => new RestHookResult()
        {
            Success = false,
            Error = error
        };
    }
    public class RestHookDataResult<T> where T: class
    {
        public static RestHookDataResult<T> SuccessResult(T data) => new RestHookDataResult<T>()
        {
            Success = true,
            Data = data
        };
        public static RestHookDataResult<T> FailedResult(RestHookResultError error) => new RestHookDataResult<T>()
        {
            Success = false,
            Error = error
        };
        public bool Success { get; set; }
        public T Data { get; set; }
        public RestHookResultError Error { get; set; }
    }
}