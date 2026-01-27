using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(Animator), typeof(CharacterController), typeof(CharacterControllerMovement))]
public class PlayerAnimationController : NetworkBehaviour
{
    private Animator _animator;
    private CharacterController _characterController;
    private CharacterControllerMovement _characterControllerMovement;

    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private void OnEnable()
    {
        _characterControllerMovement.OnFalling += FallAnimation;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _characterControllerMovement = GetComponent<CharacterControllerMovement>();
    }

    private void Start()
    {
        AssignAnimationIDs();
    }

    private void Update()
    {
        if (!isOwned) return;

        _animator.SetBool(_animIDGrounded, _characterController.isGrounded);
        var currentVelocity = _characterControllerMovement.Velocity.magnitude;
        _animator.SetFloat(_animIDSpeed, currentVelocity);

        if (_characterController.isGrounded)
        {
            _animator.SetBool(_animIDFreeFall, false);
        }
    }

    private void FallAnimation()
    {
        if (!isOwned) return;

        _animator.SetBool(_animIDFreeFall, true);
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void OnDisable()
    {
        _characterControllerMovement.OnFalling -= FallAnimation;
    }
}
