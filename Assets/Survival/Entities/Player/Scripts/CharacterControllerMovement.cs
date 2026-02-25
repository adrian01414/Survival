using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerMovement : MonoBehaviour
{
    public event Action OnFalling;

    // SerializeField private fields
    [Header("Move Settings")]
    [SerializeField, Min(0f)] private float _moveSpeed = 5f;
    [SerializeField, Min(0f)] private float _sprintSpeed = 10f;
    [SerializeField, Min(0.1f)] private float _changeSpeedTime = 10f;

    [Header("Jump Settings")]
    [SerializeField, Min(0f)] private float _jumpHeight = 2f;
    [SerializeField, Min(0f)] private float _jumpKeyImpact = 5f;
    [SerializeField, Min(0f)] private float _checkOverheadObstacleDistance = 0.1f;
    [SerializeField, Min(0f)] private float _overheadObstacleImpact = 100f;
    [SerializeField, Range(0f, 1f)] private float _coyoteTime = 0.1f;
    [SerializeField, Range(0.2f, 5f)] private float _upSpeedMultiplier = 1f;
    [SerializeField, Range(0.2f, 5f)] private float _fallSpeedMultiplier = 1f;

    [Header("Input Impact Settings")]
    [SerializeField, Range(0f, 5f)] private float _inputAirImpact = 1f;
    [SerializeField, Range(0f, 10f)] private float _inputGroundImpact = 1f;
    
    #region Public properties
    public float MoveSpeed
    {
        get => _moveSpeed;
        set => _moveSpeed = value >= 0 ? value :
            throw new ArgumentOutOfRangeException(nameof(value), "Move speed can't be less than 0");
    }

    public float SprintSpeed
    {
        get => _sprintSpeed;
        set
        {
            if (value < _moveSpeed)
                throw new ArgumentOutOfRangeException(nameof(value), "Sprint speed can't be less than move speed");
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Sprint speed can't be less than 0");
            _sprintSpeed = value;
        }
    }

    public float ChangeSpeedTime
    {
        get => _changeSpeedTime;
        set => _changeSpeedTime = value >= 0.1f ? value :
            throw new ArgumentOutOfRangeException(nameof(value), "Change speed time can't be less than 0.1");
    }

    public float JumpHeight
    {
        get => _jumpHeight;
        set => _jumpHeight = value >= 0 ? value :
            throw new ArgumentOutOfRangeException(nameof(value), "Jump height can't be less than 0");
    }

    public float JumpKeyImpact
    {
        get => _jumpKeyImpact;
        set => _jumpKeyImpact = value >= 0 ? value :
            throw new ArgumentOutOfRangeException(nameof(value), "Jump key impact can't be less than 0");
    }

    public float CheckOverheadObstacleDistance
    {
        get => _checkOverheadObstacleDistance;
        set => _checkOverheadObstacleDistance = value >= 0 ? value :
            throw new ArgumentOutOfRangeException(nameof(value), "Check overhead obstacle distance can't be less than 0");
    }

    public float OverheadObstacleImpact
    {
        get => _overheadObstacleImpact;
        set => _overheadObstacleImpact = value >= 0 ? value :
            throw new ArgumentOutOfRangeException(nameof(value), "Overhead obstacle impact can't be less than 0");
    }

    public float CoyoteTime
    {
        get => _coyoteTime;
        set => _coyoteTime = value >= 0 ? value :
            throw new ArgumentOutOfRangeException(nameof(value), "Coyote time can't be less than 0");
    }

    public float UpSpeedMultiplier
    {
        get => _upSpeedMultiplier;
        set => _upSpeedMultiplier = value >= 0.2f ? value :
            throw new ArgumentOutOfRangeException(nameof(value), "Up speed multiplier can't be less than 0.2");
    }

    public float FallSpeedMultiplier
    {
        get => _fallSpeedMultiplier;
        set => _fallSpeedMultiplier = value >= 0.2f ? value :
            throw new ArgumentOutOfRangeException(nameof(value), "Fall speed multiplier can't be less than 0.2");
    }

    public float InputAirImpact
    {
        get => _inputAirImpact;
        set => _inputAirImpact = value >= 0 ? value :
            throw new ArgumentOutOfRangeException(nameof(value), "Input air impact can't be less than 0");
    }

    public float InputGroundImpact
    {
        get => _inputGroundImpact;
        set => _inputGroundImpact = value >= 0 ? value :
            throw new ArgumentOutOfRangeException(nameof(value), "Input ground impact can't be less than 0");
    }
    #endregion

    // Dependencies
    private CharacterController _characterController;

    // Dynamic private variables
    [Header("Dynamic variables")]
    public Vector3 Velocity { get; private set; }

    private float _currentSpeed;
    private float _targetSpeed;
    private float _playerVelocityY;
    private float _coyoteTimer = 0f;
    private bool _isJumpKeyPressed = false;
    private bool _isOverheadObstacle = false;
     
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _currentSpeed = MoveSpeed;
        _targetSpeed = MoveSpeed;
    }

    private void Update()
    {
        CheckOverheadObstacle();
        Jump();
        ApplyCoyoteTime();
        ApplyGravity();

        _characterController.Move(Velocity * Time.deltaTime);
    }

    Vector3 currentMoveDirection;
    public void Move(Vector3 moveDirection)
    {
        moveDirection.Normalize();

        if(_characterController.isGrounded)
            _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, Time.deltaTime * _changeSpeedTime);

        float currentInputImpact;

        currentInputImpact = _characterController.isGrounded ?
            InputGroundImpact :
            (moveDirection.magnitude > 0 ? InputAirImpact : 0f);
        
        currentMoveDirection = Vector3.Lerp(currentMoveDirection,
            moveDirection,
            currentInputImpact * Time.deltaTime);

        if (currentMoveDirection.magnitude < 1E-5) currentMoveDirection = Vector3.zero;

        Velocity = currentMoveDirection * _currentSpeed;
    }

    public void SprintPerform() => _targetSpeed = SprintSpeed;

    public void SprintCancel() => _targetSpeed = MoveSpeed;

    public void JumpPerform()
    {
        if (_characterController.isGrounded || _coyoteTimer < CoyoteTime)
        {
            _playerVelocityY = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y / UpSpeedMultiplier);
            _coyoteTimer = CoyoteTime;
        }
        _isJumpKeyPressed = true;
    }

    public void JumpCancel() => _isJumpKeyPressed = false;

    private void ApplyCoyoteTime()
    {
        if (!_characterController.isGrounded)
            _coyoteTimer += Time.deltaTime;
    }

    private void CheckOverheadObstacle()
    {
        float height = _characterController.height;
        Vector3 center = transform.position + _characterController.center;

        float topPointY = center.y +
            (height * 0.5f) +
            _characterController.skinWidth;

        Vector3 topCenter = new Vector3(center.x, topPointY, center.z);

        _isOverheadObstacle = Physics.Raycast(topCenter, Vector3.up, out RaycastHit hit, CheckOverheadObstacleDistance) ? true : false;
    }

    private void Jump()
    {
        if (_playerVelocityY > 0f)
        {
            if (_isOverheadObstacle)
                _playerVelocityY = Mathf.Lerp(_playerVelocityY, 0f, Time.deltaTime * _overheadObstacleImpact);

            if (!_isJumpKeyPressed)
                _playerVelocityY = Mathf.Lerp(_playerVelocityY, 0f, Time.deltaTime * _jumpKeyImpact);
        }

        float currentJumpSpeed = _playerVelocityY < 0f ? FallSpeedMultiplier : UpSpeedMultiplier;

        //_characterController.Move(Vector3.up * _playerVelocityY * Time.deltaTime * currentJumpSpeed);
        Velocity = new Vector3(Velocity.x, _playerVelocityY * currentJumpSpeed, Velocity.z);
    }

    private void ApplyGravity()
    {
        if (_characterController.isGrounded)
        {
            _coyoteTimer = 0f;
            if (_playerVelocityY < -2f)
                _playerVelocityY = -2f;
        }
        else
        {
            _playerVelocityY += Physics.gravity.y * Time.deltaTime;
            OnFalling?.Invoke();
        }
    }
}
