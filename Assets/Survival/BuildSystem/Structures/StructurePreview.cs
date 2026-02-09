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

    private event Action<bool> OnAvailableForPlaceChanged;
    private bool _availableForPlace = true;
    public bool AvailableForPlace
    {
        get => _availableForPlace;
        set
        {
            _availableForPlace = value;
            OnAvailableForPlaceChanged?.Invoke(value);
        }
    }

    private void OnEnable() => OnAvailableForPlaceChanged += ChangePreviewMaterial;

    private void Awake() => _meshRenderer = GetComponent<MeshRenderer>();

    private void ChangePreviewMaterial(bool availableForPlace) => _meshRenderer.material = availableForPlace ? GreenMaterial : RedMaterial;

    // check overlap box collisions

    private void OnDisable() => OnAvailableForPlaceChanged -= ChangePreviewMaterial;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + AvailableCheckCubeCenter, AvailableCheckCubeSize);
    }
#endif
}
