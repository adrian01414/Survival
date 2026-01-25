using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(PlayerInput), typeof(CharacterControllerMovement))]
public class PlayerNetworkController : NetworkBehaviour
{
    [SerializeField] private Transform _cameraTransform; //
    [SerializeField] private Transform _rigParentTransform; //
    [SerializeField] private Transform _rigTransform; //
    [SerializeField] private Transform _playerMeshTransform; //

    [SerializeField] private Vector3 pivotOffset = new Vector3(0f, 0f, 0f);

    private CharacterControllerMovement _characterControllerMovement;

    private Vector3 _moveAxis;

    private void Awake()
    {
        _characterControllerMovement = GetComponent<CharacterControllerMovement>();
    }

    private void Start()
    {
        if (!isOwned)
        {
            _cameraTransform.gameObject.SetActive(false); //
        } else
        {
            _rigTransform.localPosition = pivotOffset;
            //_playerMeshTransform.gameObject.SetActive(false); //
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isOwned)
        {
            return;
        }
        _moveAxis = context.ReadValue<Vector2>();
    }
    
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _characterControllerMovement.SprintPerform();
        }
        if (context.canceled)
        {
            _characterControllerMovement.SprintCancel();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!isOwned)
        {
            return;
        }
        if (context.performed)
        {
            _characterControllerMovement.JumpPerform();
        }
        else if (context.canceled)
        {
            _characterControllerMovement.JumpCancel();
        }
    }

    private void Update()
    {
        if (!isOwned) return;
        
        Move();

        _rigParentTransform.rotation = Quaternion.LookRotation(GetForward());
        //float bowRotationX = Mathf.Clamp(_cameraTransform.rotation.eulerAngles.x, -60f, 60f);
    }

    private void Move()
    {
        _characterControllerMovement.Move(GetForward() * _moveAxis.y + GetRight() * _moveAxis.x);
    }

    private Vector3 GetForward()
    {
        Vector3 forward = _cameraTransform.forward;
        forward.y = 0f;

        return forward.normalized;
    }

    private Vector3 GetRight()
    {
        Vector3 right = _cameraTransform.right;
        right.y = 0f;

        return right.normalized;
    }
}
