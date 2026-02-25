using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

[RequireComponent(typeof(CharacterControllerMovement))]
public class PlayerNetworkController : NetworkBehaviour
{
    [SerializeField] private Transform _cameraTransform; //
    [SerializeField] private Transform _rigParentTransform; //
    [SerializeField] private Transform _rigTransform; //
    [SerializeField] private Transform _playerMeshTransform; //

    [SerializeField] private Vector3 pivotOffset = new Vector3(0f, 0f, 0f);

    private Vector2 _moveAxis;

    // Dependencies
    private InputManager _inputManager;
    private CharacterControllerMovement _characterControllerMovement;

    private void Awake()
    {
        _characterControllerMovement = GetComponent<CharacterControllerMovement>();
        _inputManager = InputManager.Instance;
        _inputManager.GameplayDefault_Jump.performed += OnJumpPerformed;
        _inputManager.GameplayDefault_Jump.canceled += OnJumpCanceled;
        _inputManager.GameplayDefault_Sprint.performed += OnSprintPerformed;
        _inputManager.GameplayDefault_Sprint.canceled += OnSprintCanceled;
    }

    private void Start()
    {
        if (!isOwned)
        {
            _cameraTransform.gameObject.SetActive(false); //
        } else
        {
            //_rigTransform.localPosition = pivotOffset;
            _playerMeshTransform.gameObject.SetActive(false); //
        }
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        _characterControllerMovement.SprintPerform();
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        _characterControllerMovement.SprintCancel();
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        _characterControllerMovement.JumpPerform();
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        _characterControllerMovement.JumpCancel();
    }

    private void Update()
    {
        if (!isOwned) return;

        _moveAxis = _inputManager.MoveAxis;

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

    private void OnDisable()
    {
        _inputManager.GameplayDefault_Jump.performed -= OnJumpPerformed;
        _inputManager.GameplayDefault_Jump.canceled -= OnJumpCanceled;
        _inputManager.GameplayDefault_Sprint.performed -= OnSprintPerformed;
        _inputManager.GameplayDefault_Sprint.canceled -= OnSprintCanceled;
    }
}
