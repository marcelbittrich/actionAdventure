using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CharacterAnimationAndMovement : MonoBehaviour
{
    Animator _animator;
    CharacterController _characterController;
    PlayerInput _playerInput;
    GameObject _followTarget;

    // Movement Variables.
    Vector2 _moveInput;
    Vector3 _velocity;

    public float WalkSpeed = 1.0f;
    public float RunSpeed = 2.0f;
    public float SpeedTargetToCurrentDelta = 1.0f;
    public float SpeedSmoothTime = 0.2f;
    public float RotatingSpeed = 5.0f;
    public float AimRotatingSpeed = 20.0f;

    bool _isRunning;
    bool _isMoving;
    bool _isAiming;
    float _horizontalScalarVelocity;
    float _smoothedScalarVelocity = 0;
    Vector2 _smoothedAimVelocity = Vector2.zero;
    Vector2 _smoothedAimVelocityDelta;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _followTarget = GameObject.Find("FollowTarget");
        _playerInput = new PlayerInput();

        _playerInput.CharacterControls.Move.started += OnMoveInput;
        _playerInput.CharacterControls.Move.performed += OnMoveInput;
        _playerInput.CharacterControls.Move.canceled += OnMoveInput;
        _playerInput.CharacterControls.Run.started += OnRunInput;
        _playerInput.CharacterControls.Run.performed += OnRunInput;
        _playerInput.CharacterControls.Run.canceled += OnRunInput;
        _playerInput.CharacterControls.Aim.started += OnAim;
        _playerInput.CharacterControls.Aim.performed += OnAim;
        _playerInput.CharacterControls.Aim.canceled += OnAim;
    }

    void OnMoveInput(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }
    void OnRunInput(InputAction.CallbackContext context)
    {
        _isRunning = context.ReadValueAsButton();
    }
    private void OnAim(InputAction.CallbackContext context)
    {
        float aimTriggerPosition = context.ReadValue<float>();
        _isAiming = aimTriggerPosition > 0.5;
    }

    void HandleMovement()
    {
        float currentMaxVelocity = _isRunning ? RunSpeed : WalkSpeed;

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        _velocity = forward * currentMaxVelocity * _moveInput.y + right * currentMaxVelocity * _moveInput.x;
        _horizontalScalarVelocity = _velocity.magnitude;

        _smoothedScalarVelocity = Mathf.SmoothDamp(_smoothedScalarVelocity, _horizontalScalarVelocity, ref SpeedTargetToCurrentDelta, SpeedSmoothTime);
        _smoothedAimVelocity = Vector2.SmoothDamp(_smoothedAimVelocity, _moveInput, ref _smoothedAimVelocityDelta, SpeedSmoothTime);

        if (_horizontalScalarVelocity > 0.1)
        {
            _isMoving = true;
        }
        else
        {
            _isMoving = false;
        }
    }

    void HandleGravity()
    {
        float gravity = -9.81f;
        float gravityOnGround = -0.5f;

        if (_characterController.isGrounded)
        {
            _velocity.y = gravityOnGround;
        }
        else
        {
            _velocity.y = gravity;
        }
    }

    void HandleAnimation()
    {
        _animator.SetFloat("ScalarVelocity", _smoothedScalarVelocity);
        _animator.SetFloat("AimMoveY", _smoothedAimVelocity.y);
        _animator.SetFloat("AimMoveX", _smoothedAimVelocity.x);
        _animator.SetBool("IsMoving", _isMoving);
        _animator.SetBool("IsAiming", _isAiming);
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        // Save Follow Target Foward to reaply after rotating when aiming.
        Vector3 followTargetFowrard = _followTarget.transform.forward;

        Vector3 aimDirection = _followTarget.transform.forward;
        aimDirection.y = 0;
        aimDirection = aimDirection.normalized;

        Debug.DrawLine(_followTarget.transform.position, _followTarget.transform.position + transform.forward, Color.blue);
        Debug.DrawLine(_followTarget.transform.position, _followTarget.transform.position + aimDirection, Color.red);

        if (!_isAiming)
        {
            positionToLookAt = transform.position + new Vector3(_velocity.x, 0, _velocity.z);

            Vector3 lookDirection = transform.position + transform.forward;
            Vector3 currentFacing = Vector3.Lerp(lookDirection, positionToLookAt, RotatingSpeed * Time.deltaTime);

            transform.LookAt(currentFacing);
        }
        else
        {
            positionToLookAt = transform.position + aimDirection;

            Vector3 lookDirection = transform.position + transform.forward;
            Vector3 currentFacing = Vector3.Lerp(lookDirection, positionToLookAt, AimRotatingSpeed * Time.deltaTime);
            
            transform.LookAt(currentFacing);

            // Apply saved Follow Target Foward after parent rotation to keep the camera direction.
            _followTarget.transform.forward = followTargetFowrard;
        }
    }

    private void OnAnimatorMove()
    {
        Vector3 animationVelocity = _animator.deltaPosition;
        animationVelocity.y = _velocity.y * Time.deltaTime;
        _characterController.Move(animationVelocity);
    }

    void Update()
    {
        HandleMovement();
        HandleGravity();
        HandleRotation();
        HandleAnimation();

    }

    private void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }
}
