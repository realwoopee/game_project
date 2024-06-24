using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Character Input Values")] public Vector2 move;
    public Vector2 look;
    public bool sprint;

    public event Action OnInteractPressed;
    public event Action OnInteractReleased;
    public bool interactHeld;

    public Vector2 vehicleControl;

    public bool cursorHidden;

    public void Start()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions.FindActionMap("Player").Enable();
        //playerInput.actions.FindActionMap("Vehicle").Enable();
    }

    public void OnMove(InputValue value) => MoveInput(value.Get<Vector2>());

    public void OnLook(InputValue value) => LookInput(value.Get<Vector2>());

    public void OnVehicleDrive(InputValue value) => VehicleDriveInput(value.Get<float>());

    public void OnVehicleSteer(InputValue value) => VehicleSteerInput(value.Get<float>());

    public void OnInteract(InputValue value) => InteractInput(value.isPressed);

    public void OnSprint(InputValue value) => SprintInput(value.isPressed);

    public void MoveInput(Vector2 newMoveDirection) => move = newMoveDirection;

    public void VehicleDriveInput(float driveValue) =>
        vehicleControl.y = driveValue;

    public void VehicleSteerInput(float steerValue) =>
        vehicleControl.x = steerValue;

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void SprintInput(bool newSprintState) => sprint = newSprintState;


    private bool _lastInteractState = false;

    public void InteractInput(bool newValue)
    {
        switch ((_lastInteractState, newValue))
        {
            case (false, true):
                interactHeld = true;
                OnInteractPressed?.Invoke();
                break;
            case (true, false):
                interactHeld = false;
                OnInteractReleased?.Invoke();
                break;
            default:
                break;
        }

        _lastInteractState = newValue;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            SetCursorHiddenState(cursorHidden);
    }

    private void SetCursorHiddenState(bool isHidden) => Cursor.visible = !isHidden;
}