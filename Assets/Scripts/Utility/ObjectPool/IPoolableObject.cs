namespace BallisticSandbox.Utility.ObjectPool
{
    public interface IPoolableObject<T> where T : class
    {
        ulong InPoolID { get; }
        IObjectPool<T> Owner { get; }

        void OnCreateObject(ulong inPoolID, IObjectPool<T> owner);
        void OnGetObject();
        void OnReleaseObject();
        void OnDestroyObject();
    }
}
