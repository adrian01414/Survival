using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StructureInfo", menuName = "Scriptable Objects/StructureInfo")]
public class StructureInfo : ScriptableObject
{
    public Structure Structure;
    public StructurePreview StructurePreview;

    public List<ResourceCost> Cost;

    [Serializable]
    public class ResourceCost
    {
        [SerializeField] public ResourceType ResourceTypeEnum;
        public Type ResourceType;
        public int Amount;
    }

    private void OnEnable()
    {
        foreach(var item in Cost)
        {
            item.ResourceType = ResourceConverter.GetResourceType(item.ResourceTypeEnum);
        }
    }
}
