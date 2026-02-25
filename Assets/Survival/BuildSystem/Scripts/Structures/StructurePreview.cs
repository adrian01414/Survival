using System;
using System.Collections.Generic;
using UnityEngine;

public class StructurePreview : MonoBehaviour
{
    public List<MeshRenderer> MeshRenderers;
    public Transform SizePivot;

    public Material GreenMaterial;
    public Material RedMaterial;

    public void ChangePreviewMaterial(bool availableForPlace)
    {
        foreach (var renderer in MeshRenderers)
        {
            renderer.material = availableForPlace ? GreenMaterial : RedMaterial;
        }
    }
}
