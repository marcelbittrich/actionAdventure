using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class CharacterFollowTarget : MonoBehaviour
{
    public GameObject FollowTarget;

    public bool InvertVerticalAxis = false;
    public bool InvertHorizontalAxis = false;
    public float RotationSpeedHorizontal = 50.0f;
    public float RotatingSpeedVertical = 50.0f;
    
    PlayerInput _playerInput;
    Vector2 _inputLook;

    private void Awake()
    {
        _playerInput = new PlayerInput();

        _playerInput.CharacterControls.Look.started += OnLook;
        _playerInput.CharacterControls.Look.performed += OnLook;
        _playerInput.CharacterControls.Look.canceled += OnLook;
    }

    void OnLook(InputAction.CallbackContext context)
    { 
        _inputLook = context.ReadValue<Vector2>();
    }

    void HandleCameraRotation()
    {
        Vector3 currentRotation = FollowTarget.transform.localEulerAngles;
        float invertHorizontal = InvertHorizontalAxis ? -1 : 1;
        float invertVertical = InvertVerticalAxis ? -1 : 1;

        float currentRotationDeltaHorizontal = invertHorizontal * _inputLook.x * RotationSpeedHorizontal * Time.deltaTime ;
        float currentRotationDeltaVertical = invertVertical * _inputLook.y * RotatingSpeedVertical * Time.deltaTime;

        currentRotation.x += currentRotationDeltaVertical;
        currentRotation.y += currentRotationDeltaHorizontal;
        currentRotation.z = 0;    

        //Clamp the Up/Down rotation
        if (currentRotation.x > 180 && currentRotation.x < 340)
        {
            currentRotation.x = 340;
        }
        else if (currentRotation.x < 180 && currentRotation.x > 40)
        {
            currentRotation.x = 40;
        }

        FollowTarget.transform.localEulerAngles = currentRotation;
    }

    void Update()
    {
        HandleCameraRotation();
    }

    private void OnEnable()
    {
        _playerInput.CharacterControls.Enable();

        // Reset FollowTarget Rotation to follow camera rotation.
        FollowTarget.transform.eulerAngles = Camera.main.transform.eulerAngles;
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }
}
