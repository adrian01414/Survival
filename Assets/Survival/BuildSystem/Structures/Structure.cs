using UnityEngine;

public abstract class Structure : MonoBehaviour
{
    public virtual void Awake()
    {
        gameObject.isStatic = true;
    }
}
