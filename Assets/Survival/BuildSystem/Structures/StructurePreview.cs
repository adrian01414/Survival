using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class StructurePreview : MonoBehaviour
{
    public Material GreenMaterial;
    public Material RedMaterial;

    private MeshRenderer _meshRenderer;

    private void Awake() => _meshRenderer = GetComponent<MeshRenderer>();

    public void ChangePreviewMaterial(bool availableForPlace) => _meshRenderer.material = availableForPlace ? GreenMaterial : RedMaterial;
}
