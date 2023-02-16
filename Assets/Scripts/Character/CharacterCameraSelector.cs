using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CharacterCameraSelector : MonoBehaviour
{
    PlayerInput _playerInput;

    public CinemachineFreeLook FollowCamera;
    public CinemachineVirtualCamera AimCamera;

    bool _isAiming = false;

    private void Awake()
    {
        _playerInput = new PlayerInput();

        _playerInput.CharacterControls.Aim.started += OnAim;
        _playerInput.CharacterControls.Aim.performed += OnAim;
        _playerInput.CharacterControls.Aim.canceled += OnAim;
    }

    private void OnAim(InputAction.CallbackContext context) 
    {
        float aimTriggerPosition = context.ReadValue<float>();
        _isAiming = aimTriggerPosition > 0.5;

        FollowCamera.gameObject.SetActive(!_isAiming);
        AimCamera.gameObject.SetActive(_isAiming);
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
