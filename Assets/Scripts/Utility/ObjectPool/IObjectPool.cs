namespace BallisticSandbox.Utility.ObjectPool
{
    public interface IObjectPool<T> where T : class
    {
        int CountActiveObjects { get; }
        int CountInactiveObjects { get; }
        int CountAllObjects { get; }

        void Clear();
        T GetObject();
        void ReleaseObject(T obj);
    }
}
