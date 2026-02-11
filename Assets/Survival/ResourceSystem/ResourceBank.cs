using System;
using System.Collections.Generic;

public class ResourceBank
{
    private Dictionary<Type, int> _resources = new();
    public IReadOnlyDictionary<Type, int> Resources => _resources;

    public void SetResource<T>(int resourceCount) where T : Resource
    {
        var resourceType = typeof(T);

        if (!_resources.ContainsKey(resourceType))
        {
            _resources.Add(resourceType, resourceCount);
        } else
        {
            _resources[resourceType] = resourceCount;
        }
    }
}
