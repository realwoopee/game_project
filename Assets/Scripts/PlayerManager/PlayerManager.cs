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
    public StormManager stormManager;
    public HealthBarManager health;

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
    public void ManageStorm(){
        Vector3 playerPosition = playerController.transform.position;
        float someDistance = stormManager.StormProgressAlongAxis(stormManager.MillisecondsPassed);
        float timeSinceLastHitByStorm = stormManager.StormProgressAlongAxis(stormManager.timeSinceLastDamage);
        Debug.Log("x: " + playerPosition.x + " to " + (stormManager.MapWidth - someDistance));
        Debug.Log("y: " + playerPosition.y + " to " + (stormManager.MapHeight - someDistance));
        Debug.Log(timeSinceLastHitByStorm);
        if (playerPosition.x < stormManager.MapWidth - someDistance && playerPosition.z < stormManager.MapHeight - someDistance && timeSinceLastHitByStorm > 1000)
            health.TakeDamage(5);
    }
    // Update is called once per frame
    void Update()
    {
        ManageShooting();
        ManageStorm();
    }
}

public enum PlayerState
{
    OnFoot,
    InVehicle
}
