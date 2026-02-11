using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class StructurePreview : MonoBehaviour
{
    public Material GreenMaterial;
    public Material RedMaterial;

    [Header("Overlap Box Settings")]
    public Vector3 AvailableCheckCubeCenter;
    public Vector3 AvailableCheckCubeSize;

    private MeshRenderer _meshRenderer;

    private void Awake() => _meshRenderer = GetComponent<MeshRenderer>();

    public void ChangePreviewMaterial(bool availableForPlace) => _meshRenderer.material = availableForPlace ? GreenMaterial : RedMaterial;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + AvailableCheckCubeCenter, AvailableCheckCubeSize);
    }
#endif
}
