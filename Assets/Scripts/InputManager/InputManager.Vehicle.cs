using UnityEngine;

public partial class InputManager
{
    public Vector2 vehicleControl;
    public void VehicleDriveInput(float driveValue) => vehicleControl.y = driveValue;
    public void VehicleSteerInput(float steerValue) => vehicleControl.x = steerValue;
}