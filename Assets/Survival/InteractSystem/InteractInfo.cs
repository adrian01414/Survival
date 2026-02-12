using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractInfo : MonoBehaviour
{
    public Component InteractableObject;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer(Keys.Layers.Interactable);
    }
}
