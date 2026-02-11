using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Structure : MonoBehaviour
{
    public virtual void Awake()
    {
        gameObject.isStatic = true;
    }

    public abstract Vector3 CalculatePreviewOffset(Structure preview, StructurePivotInfo pivotInfo);
}
