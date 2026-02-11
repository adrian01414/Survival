using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Structure : MonoBehaviour
{
    public abstract Vector3 CalculatePreviewOffset(Structure preview, StructurePivotInfo pivotInfo);
}
