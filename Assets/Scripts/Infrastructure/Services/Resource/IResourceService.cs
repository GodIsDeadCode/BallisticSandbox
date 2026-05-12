using UnityEngine;

namespace BallisticSandbox.Infrastructure.Services.Resource
{
    public interface IResourceService
    {
        void ClearCache();
        TResource LoadResource<TResource>(string path) where TResource : Object;
        bool TryLoadResource<TResource>(string path, out TResource resource) where TResource : Object;
    }
}