using System;
using UnityEngine;

public partial class InputManager
{
    /// x = steer; y = drive
    public Vector2 vehicleControl;

    public void VehicleDriveInput(float driveValue) => vehicleControl.y = driveValue;
    public void VehicleSteerInput(float steerValue) => vehicleControl.x = steerValue;
    
    public event Action OnHandbrakePressed;
    public event Action OnHandbrakeReleased;
    public bool handbrakeHeld;
    private bool _lastHandbrakeState = false;

    public void HandbrakeInput(bool newValue)
    {
        switch ((_lastHandbrakeState, newValue))
        {
            case (false, true):
                handbrakeHeld = true;
                OnHandbrakePressed?.Invoke();
                break;
            case (true, false):
                handbrakeHeld = false;
                OnHandbrakeReleased?.Invoke();
                break;
            default:
                break;
        }

        _lastHandbrakeState = newValue;
    }
    
}