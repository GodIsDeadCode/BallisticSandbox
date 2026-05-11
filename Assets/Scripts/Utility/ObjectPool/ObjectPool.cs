using System;
using System.Collections.Generic;

namespace BallisticSandbox.Utility.ObjectPool
{
    public class ObjectPool<T> : IObjectPool<T> where T : class, IPoolableObject<T>
    {
        private const int DefaultPoolSize = 10;
        private const int DefaultMaxPoolSize = 1000;
        private const int DefaultExpansionStep = 10;


        private readonly Stack<T> _inactiveObjects;
        private readonly Dictionary<ulong, T> _activeObjects;
        private readonly Func<T> _factory;
        private readonly Action<T> _getAction;
        private readonly Action<T> _releaseAction;
        private readonly Action<T> _destroyAction;
        private readonly int _poolSize;
        private readonly int _maxPoolSize;
        private readonly int _expansionStep;
        private readonly bool _allowExpansion;
        private readonly PoolExhaustedBehavior _poolExhaustedBehavior;

        private ulong _idGiver;

        public int CountInactiveObjects => _inactiveObjects.Count;
        public int CountActiveObjects => _activeObjects.Count;
        public int CountAllObjects => CountActiveObjects + CountInactiveObjects;

        public ObjectPool(
            Func<T> factory,
            Action<T> getAction = null,
            Action<T> releaseAction = null,
            Action<T> destroyAction = null,
            int poolSize = DefaultPoolSize,
            bool allowExpansion = true,
            int maxPoolSize = DefaultMaxPoolSize,
            int expansionStep = DefaultExpansionStep,
            PoolExhaustedBehavior poolExhaustedBehavior = PoolExhaustedBehavior.ReturnNull)
        {

            if (factory == null)
                throw new ArgumentNullException(nameof(factory), "Factory cannot be null.");

            if (poolSize <= 0)
                poolSize = DefaultPoolSize;

            if (maxPoolSize <= 0)
                maxPoolSize = DefaultMaxPoolSize;

            if (poolSize > maxPoolSize)
                poolSize = maxPoolSize;

            if (expansionStep <= 0)
                expansionStep = DefaultExpansionStep;

            _factory = factory;
            _getAction = getAction;
            _releaseAction = releaseAction;
            _destroyAction = destroyAction;
            _poolSize = poolSize;
            _allowExpansion = allowExpansion;
            _maxPoolSize = maxPoolSize;
            _expansionStep = expansionStep;
            _poolExhaustedBehavior = poolExhaustedBehavior;

            _inactiveObjects = new Stack<T>(maxPoolSize);
            _activeObjects = new Dictionary<ulong, T>(maxPoolSize);
            _poolExhaustedBehavior = poolExhaustedBehavior;
        }

        public T GetObject()
        {
            if (CountInactiveObjects > 0)
                return ActiveObject(_inactiveObjects.Pop());

            if (_allowExpansion && CountAllObjects < _maxPoolSize)
            {
                Expand();

                if (CountInactiveObjects > 0)
                    return ActiveObject(_inactiveObjects.Pop());
            }

            if (_poolExhaustedBehavior == PoolExhaustedBehavior.ThrowException)
                throw new InvalidOperationException($"[ObjectPool<{typeof(T).Name}>] Pool exhausted " +
                                                    $"(max={_maxPoolSize}, active={CountActiveObjects}).");

            return null;
        }

        public void ReleaseObject(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj), "Object cannot be null.");

            if (!ReferenceEquals(this,obj.Owner))
                throw new InvalidOperationException($"[ObjectPool<{typeof(T).Name}>] Object does not belong to this pool.");

            if (!_activeObjects.ContainsKey(obj.InPoolID))
                throw new InvalidOperationException($"[ObjectPool<{typeof(T).Name}>] Object (ID={obj.InPoolID}) is not active.");

            _activeObjects.Remove(obj.InPoolID);
            DiactiveObject(obj);
        }

        public T GetActiveObject(ulong id)
        {
            if (_activeObjects.TryGetValue(id, out T obj))
                return obj;

            return null;
        }

        public void Dispose()
        {
            Clear();

            while (CountActiveObjects > 0)
            {
                T obj = _inactiveObjects.Pop();
                _destroyAction?.Invoke(obj);
                obj.OnDestroyObject();
            }

            _idGiver = 0;
        }

        public void Clear()
        {
            List<T> list = new List<T>(_activeObjects.Values);
            foreach (T obj in list)
                ReleaseObject(obj);
        }

        private void Expand()
        {
            int canCreate = _maxPoolSize - CountAllObjects;
            int toCreate = Math.Min(_expansionStep, canCreate);

            for (int i = 0; i < canCreate; i++)
                DiactiveObject(CreateObject());
        }

        private T ActiveObject(T obj)
        {
            _activeObjects.Add(obj.InPoolID, obj);
            obj.OnGetObject();
            _getAction?.Invoke(obj);

            return obj;
        }

        private void DiactiveObject(T obj)
        {
            obj.OnReleaseObject();
            _releaseAction?.Invoke(obj);
            _inactiveObjects.Push(obj);

        }

        private T CreateObject()
        {
            T obj = _factory.Invoke();
            if (obj == null)
                throw new InvalidOperationException($"[ObjectPool<{typeof(T).Name}>] Factory returned null.");

            obj.OnCreateObject(_idGiver++, this);
            return obj;
        }
    }

    public enum PoolExhaustedBehavior
    {
        ReturnNull,
        ThrowException
    }
}
