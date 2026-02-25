using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StructurePivotInfo : MonoBehaviour
{
    public Structure Structure;

    public Vector3 Direction { get; private set; }
    public int StructurePivotLayer { get; private set; }
    public Collider Collider { get; private set; }

    private Vector3 overlapPoint;

    private void Awake()
    {
        // Collider init
        Collider = GetComponent<Collider>();
        if (Collider is MeshCollider)
        {
            (Collider as MeshCollider).convex = true;
        }
        Collider.isTrigger = true;

        // Layer init
        StructurePivotLayer = LayerMask.NameToLayer("StructurePivot");
        gameObject.layer = StructurePivotLayer;

        // Direction init
        Direction = new Vector3(transform.localPosition.x > 0f ? 1f : (transform.localPosition.x < 0f ? -1f : 0f),
                                transform.localPosition.y > 0f ? 1f : (transform.localPosition.y < 0f ? -1f : 0f),
                                transform.localPosition.z > 0f ? 1f : (transform.localPosition.z < 0f ? -1f : 0f));
    }
}
