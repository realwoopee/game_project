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
    public bool isInventoryOpened { get => inventoryManager.IsInnerOpened; set => inventoryManager.IsInnerOpened = value; }

    // Start is called before the first frame update
    void Start()
    {
        inputManager.OnInteractPressed += OnInteract;
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

    public void ManageShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (playerController.Speed >= playerController.SprintSpeed || inventoryManager.IsInnerOpened)//or inventoryOpened
                return;

            playerController.SelectedGun.Fire();
        }
        if (Input.GetKeyDown(KeyCode.R) && !playerController.SelectedGun.IsReloading && !(playerController.SelectedGun.ShellsLeft == playerController.SelectedGun.MagSize))
            playerController.SelectedGun.Reload();

    }
    // Update is called once per frame
    void Update()
    {
        ManageShooting();
    }
}

public enum PlayerState
{
    OnFoot,
    InVehicle
}
