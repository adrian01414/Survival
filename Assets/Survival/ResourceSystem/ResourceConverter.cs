using System;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceConverter
{
    private static Dictionary<ResourceType, Type> _types = new()
    {
        {ResourceType.Wood, typeof(WoodResource) }
    };

    public static Type GetResourceType(ResourceType resourceType)
    {
        return _types[resourceType];
    }
}

public enum ResourceType
{
    Wood
}