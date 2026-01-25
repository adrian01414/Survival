using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(Animator), typeof(CharacterController), typeof(CharacterControllerMovement))]
public class PlayerAnimationController : NetworkBehaviour
{
    private Animator _animator;
    private CharacterController _characterController;
    private CharacterControllerMovement _characterControllerMovement;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private void OnEnable()
    {
        _characterControllerMovement.OnFell += FallAnimation;
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

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_characterController.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_characterController.center), FootstepAudioVolume);
        }
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
        _characterControllerMovement.OnFell -= FallAnimation;
    }
}
