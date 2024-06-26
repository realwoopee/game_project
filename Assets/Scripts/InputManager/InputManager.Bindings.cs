using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputManager
{
    public void OnMove(InputValue value) => MoveInput(value.Get<Vector2>());
    public void OnLook(InputValue value) => LookInput(value.Get<Vector2>());
    public void OnVehicleDrive(InputValue value) => VehicleDriveInput(value.Get<float>());
    public void OnVehicleSteer(InputValue value) => VehicleSteerInput(value.Get<float>());
    public void OnInteract(InputValue value) => InteractInput(value.isPressed);
    public void OnSprint(InputValue value) => SprintInput(value.isPressed);
    public void OnOpenClose(InputValue value) => InventoryOpenCloseInput(value.isPressed);
    public void OnAds(InputValue value) => AdsInput(value.isPressed);
    public void OnShoot(InputValue value) => ShootInput(value.isPressed);
    public void OnReload(InputValue value) => ReloadInput(value.isPressed);
}