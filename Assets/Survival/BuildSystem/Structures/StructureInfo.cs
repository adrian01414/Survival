using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StructureInfo", menuName = "Scriptable Objects/StructureInfo")]
public class StructureInfo : ScriptableObject
{
    public Structure Structure;
    public StructurePreview StructurePreview;
}
