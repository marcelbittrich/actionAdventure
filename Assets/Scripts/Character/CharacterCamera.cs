using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class CharacterCamera : MonoBehaviour
{
    public GameObject FollowTarget;

    public float RotationSpeedHorizontal = 50.0f;
    public float RotatingSpeedVertical = 50.0f;
    public float MaxVerticalAngle = 250.0f;
    public float MinVerticalAngle = 140.0f;
    

    PlayerInput _playerInput;
    Vector2 _inputLook;

    private void Awake()
    {
        _playerInput = new PlayerInput();

        _playerInput.CharacterControls.Look.started += OnLook;
        _playerInput.CharacterControls.Look.performed += OnLook;
        _playerInput.CharacterControls.Look.canceled += OnLook;
    }

    // 
    void OnLook(InputAction.CallbackContext context)
    { 
        _inputLook = context.ReadValue<Vector2>();
    }

    void HandleCameraRotation()
    {
        Vector3 currentRotation = FollowTarget.transform.eulerAngles;

        float currentRotationDeltaHorizontal = _inputLook.x * RotationSpeedHorizontal * Time.deltaTime;
        float currentRotationDeltaVertical = _inputLook.y * RotatingSpeedVertical * Time.deltaTime;

        currentRotation.y += currentRotationDeltaHorizontal;

        FollowTarget.transform.eulerAngles = currentRotation;


        FollowTarget.transform.rotation *= Quaternion.AngleAxis(currentRotationDeltaVertical, Vector3.right);

        var angles = FollowTarget.transform.localEulerAngles;
        angles.z = 0;

        var angle = FollowTarget.transform.localEulerAngles.x;

        //Clamp the Up/Down rotation
        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }

        FollowTarget.transform.localEulerAngles = angles;
    }

    void Update()
    {
        HandleCameraRotation();
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
