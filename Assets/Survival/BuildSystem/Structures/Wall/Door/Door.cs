using UnityEngine;

public class Door : Wall, IInteractable
{
    public Collider DoorCollider;
    [HideInInspector] public Vector3 playerPosition;

    private Animator _animator;
    private bool _isOpen = false;
    private int _currentOpenDirection;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

    }

    public void Interact()
    {
        if (_isOpen)
        {
            Close();
        } else
        {
            Open();
        }
    }

    private void Open()
    {
        _isOpen = true;

        DoorCollider.gameObject.SetActive(false);

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
        _isOpen = false;

        DoorCollider.gameObject.SetActive(true);
        
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
