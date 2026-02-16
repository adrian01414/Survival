using Mirror;
using UnityEngine;

public class Door : Wall, IInteractable
{
    public Collider DoorCollider;
    [HideInInspector] public Vector3 playerPosition;

    [SyncVar(hook = nameof(OnDoorStateChanged))] private bool _isOpen = false;

    private Animator _animator;
    private int _currentOpenDirection;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        if (!isServer) return;

        _isOpen = !_isOpen;
    }

    private void OnDoorStateChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            Open();
        } else
        {
            Close();
        }
    }

    private void Open()
    {
        DoorCollider.isTrigger = true;

        Vector3 localOpenerPosition = transform.InverseTransformPoint(playerPosition);
        if(localOpenerPosition.z > 0)
        {
            _currentOpenDirection = -1;
            _animator.Play("DoorOpenBack");
        }
        else
        {
            _currentOpenDirection = 1;
            _animator.Play("DoorOpenForward");
        }
    }

    private void Close()
    {
        DoorCollider.isTrigger = false;

        Vector3 localOpenerPosition = transform.InverseTransformPoint(playerPosition);
        
        if (_currentOpenDirection > 0)
        {
            _animator.Play("DoorCloseForward");
        }
        else
        {
            _animator.Play("DoorCloseBack");
        }
    }
}
