using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallisticSandbox.Infrastructure.Services.Resource
{
    public class ResourceService : IResourceService
    {
        private readonly Dictionary<string, UnityEngine.Object> _chache;

        public ResourceService()
        {
            _chache = new Dictionary<string, UnityEngine.Object>();
        }

        public bool TryLoadResource<TResource>(string path, out TResource resource) where TResource : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path is not correct.", nameof(path));

            resource = LoadResourceInternal<TResource>(path);
            return resource != null;
        }

        public TResource LoadResource<TResource>(string path) where TResource : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path is not correct.", nameof(path));

            if (_chache.TryGetValue(path, out UnityEngine.Object resource))
                return (TResource)resource;

            resource = LoadResourceInternal<TResource>(path);
            if (resource == null)
                throw new ArgumentNullException($"Resource of type {typeof(TResource).Name} from {path} does not loaded..");

            _chache[path] = resource;
            return (TResource)resource;
        }

        public void ClearCache()
        {
            _chache.Clear();
        }

        private TResource LoadResourceInternal<TResource>(string path) where TResource : UnityEngine.Object
        {
            TResource resource = Resources.Load<TResource>(path);
            return resource;
        }
    }
}
