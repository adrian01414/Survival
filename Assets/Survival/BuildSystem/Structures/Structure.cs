using Mirror;
using UnityEngine;

public abstract class Structure : NetworkBehaviour
{
    public Transform SizePivot;

    public abstract Vector3 CalculatePreviewOffset(Structure preview, StructurePivotInfo pivotInfo);
}
