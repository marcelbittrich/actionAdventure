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
    public float MaxBreakingTorque;
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
            motorTorque = MaxMotorTorque;
        }
        if (_isBreaking)
        {
            motorTorque = -MaxBreakingTorque;
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
            ApplyLocalPositionToVisuals(axleInfo.LeftWheel);
            ApplyLocalPositionToVisuals(axleInfo.RightWheel);
        }
    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0) return;
        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = Quaternion.Euler(
            rotation.eulerAngles.x,
            rotation.eulerAngles.y,
            rotation.eulerAngles.z + 90  // Currently needed to keep the needed rotation of the cylider.
        );
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
