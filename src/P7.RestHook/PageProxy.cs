using System.Collections;
using System.Collections.Generic;
using P7.RestHook.Store;

namespace P7.RestHook
{
    public class Page<T> : IPage<T>
    {
        public List<T> RawData { get; set; }


        public Page(byte[] currentPagingState, byte[] pagingState, List<T> rawData)
        {
            CurrentPagingState = currentPagingState;
            PagingState = pagingState;
            RawData = rawData;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return RawData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            RawData.Add(item);
        }

        public void Clear()
        {
            RawData.Clear();
        }

        public bool Contains(T item)
        {
            return RawData.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            RawData.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return RawData.Remove(item);
        }

        public int Count
        {
            get { return RawData.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public byte[] CurrentPagingState { get; private set; }
        public byte[] PagingState { get; private set; }
    }
}