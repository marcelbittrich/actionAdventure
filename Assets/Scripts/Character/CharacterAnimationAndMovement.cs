using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterAnimationAndMovement : MonoBehaviour
{
    Animator _animator;
    CharacterController _characterController;
    PlayerInput _playerInput;

    // Movement Variables.
    Vector2 _moveInput;
    Vector3 _velocity;

    public float WalkSpeed = 1.0f;
    public float RunSpeed = 2.0f;

    bool _isRunning;
    bool _isMoving;
    float _horizontalVelocity;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>(); 
        _playerInput = new PlayerInput();

        _playerInput.CharacterControls.Move.started += OnMoveInput;
        _playerInput.CharacterControls.Move.performed += OnMoveInput;
        _playerInput.CharacterControls.Move.canceled += OnMoveInput;
        _playerInput.CharacterControls.Run.started += OnRunInput;
        _playerInput.CharacterControls.Run.performed += OnRunInput;
        _playerInput.CharacterControls.Run.canceled += OnRunInput;

    }

    void OnMoveInput (InputAction.CallbackContext context)      
    { 
        _moveInput = context.ReadValue<Vector2>();
    }
    void OnRunInput(InputAction.CallbackContext context)
    {
        _isRunning = context.ReadValueAsButton();
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
        _horizontalVelocity = _velocity.magnitude;


        if (_horizontalVelocity > 0.1)
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
        _animator.SetFloat("VelocityZ", _horizontalVelocity);
        _animator.SetBool("IsMoving", _isMoving);
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt = transform.position + new Vector3 (_velocity.x, 0, _velocity.z);
        transform.LookAt(positionToLookAt);
    }

    private void OnAnimatorMove()
    {
        Vector3 animationVelocity = _animator.deltaPosition;
        animationVelocity.y = _velocity.y * Time.deltaTime;
        _characterController.Move(animationVelocity);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
