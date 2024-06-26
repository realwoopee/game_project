using System;
using Cinemachine;
using StarterAssets;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public CursorController cursorController;
    public ThirdPersonController playerController;
    public VehicleManager vehicleManager;
    public CinemachineVirtualCamera virtualCamera;
    public InputManager inputManager;
    public InventoryManager inventoryManager;
    // [SerializeField] public Gun gun;

    [field: SerializeField]
    public PlayerState PlayerState { get; private set; } = PlayerState.OnFoot;
    
    [field: SerializeField]
    public bool IsInventoryOpened { get => inventoryManager.IsInnerOpened; set => inventoryManager.IsInnerOpened = value; }

    // Start is called before the first frame update
    void Start()
    {
        inputManager.OnInteractPressed += OnInteract;
        //inputManager.OnShootPressed += Shoot;
        inputManager.OnReloadPressed += Reload;
    }

    void OnInteract()
    {
        switch (PlayerState)
        {
            case PlayerState.OnFoot:
                if (cursorController.Highlighted && cursorController.Highlighted == vehicleManager.transform.parent.gameObject)
                {
                    PutPlayerInVehicle(vehicleManager.gameObject);
                }
                break;
            case PlayerState.InVehicle:
                PutPlayerOutOfVehicle(vehicleManager.gameObject);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void PutPlayerInVehicle(GameObject vehicle)
    {
        playerController.gameObject.SetActive(false);
        virtualCamera.Follow = vehicle.gameObject.transform;
        vehicle.GetComponent<VehicleManager>().PlayerGotIn();
        PlayerState = PlayerState.InVehicle;
    }

    void PutPlayerOutOfVehicle(GameObject vehicle)
    {
        playerController.transform.position =
            vehicle.transform.parent.Find("PlayerLeavePosition").position;
        playerController.gameObject.SetActive(true);
        virtualCamera.Follow = playerController.transform.Find("PlayerCameraRoot").transform;
        vehicle.GetComponent<VehicleManager>().PlayerGotOut();
        PlayerState = PlayerState.OnFoot;

    }

    void Shoot()
    {
        if (playerController.Speed >= playerController.SprintSpeed || inventoryManager.IsInnerOpened)//or inventoryOpened
            return;

        playerController.SelectedGun.Fire();
    }

    void Reload()
    {
        if (!playerController.SelectedGun.IsReloading && playerController.SelectedGun.ShellsLeft != playerController.SelectedGun.MagSize)
            playerController.SelectedGun.Reload();
    }
    
    void Update()
    {
        if(inputManager.shootHeld)
            Shoot();
    }
}

public enum PlayerState
{
    OnFoot,
    InVehicle
}
