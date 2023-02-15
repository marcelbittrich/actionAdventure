using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleController : MonoBehaviour
{
    PlayerInput _playerInput;
    float _steeringInput;
    bool _isAccelerating;
    bool _isBreaking;
    
    public float MaxMotorTorque;
    public float MaxSteeringAngle;
    public List<AxleInfo> AxleInfos;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerInput.VehicleControls.Steering.started += OnSteeringInput;
        _playerInput.VehicleControls.Steering.performed += OnSteeringInput;
        _playerInput.VehicleControls.Steering.canceled += OnSteeringInput;
        _playerInput.VehicleControls.Accelerate.started += OnAccelerate;
        _playerInput.VehicleControls.Accelerate.performed += OnAccelerate;
        _playerInput.VehicleControls.Accelerate.canceled += OnAccelerate;
        _playerInput.VehicleControls.Brake.started += OnBrake;
        _playerInput.VehicleControls.Brake.performed += OnBrake;
        _playerInput.VehicleControls.Brake.canceled += OnBrake;
    }

    public void OnSteeringInput(InputAction.CallbackContext context)
    {
        _steeringInput = context.ReadValue<float>();
    }

    public void OnAccelerate(InputAction.CallbackContext context)
    {
        _isAccelerating = context.ReadValueAsButton();
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        _isBreaking = context.ReadValueAsButton();
    }

    public void FixedUpdate()
    {
        float steering = MaxSteeringAngle * _steeringInput;
        float motorTorque = 0;
        if (_isAccelerating)
        {
            motorTorque = 800f;
        }
        if (_isBreaking)
        {
            motorTorque = -200f;
        }
        Debug.Log(motorTorque);
        foreach (AxleInfo axleInfo in AxleInfos)
        {
            if (axleInfo.IsSteering)
            {
                axleInfo.LeftWheel.steerAngle = steering;
                axleInfo.RightWheel.steerAngle = steering;
            }
            if (axleInfo.IsMotor)
            {
                axleInfo.LeftWheel.motorTorque = motorTorque;
                axleInfo.RightWheel.motorTorque = motorTorque;
            }
        }
    }

    private void OnEnable()
    {
        _playerInput.VehicleControls.Enable();
    }

    private void OnDisable()
    {
        _playerInput.VehicleControls.Disable();
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider LeftWheel;
    public WheelCollider RightWheel;
    public bool IsMotor;
    public bool IsSteering;
}
