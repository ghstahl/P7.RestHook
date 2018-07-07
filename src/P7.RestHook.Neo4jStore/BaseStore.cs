using System;
using Neo4jClient;

namespace P7.RestHook.Neo4jStore
{
    public abstract class BaseStore : IDisposable
    {
        private bool _disposed;
        protected IGraphClient GraphClient { get; }
      
        protected BaseStore(IGraphClient graphClient)
        {
            GraphClient = graphClient;
           
        }

        protected void Dispose(bool isDisposing)
        {
            _disposed = true;
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}