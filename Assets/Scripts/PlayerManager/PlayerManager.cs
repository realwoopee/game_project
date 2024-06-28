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
    public StormCubeManager stormCubeManager;
    public HealthBarManager healthBar;

    [field: SerializeField]
    [HideInInspector] public bool isInCar { get; private set; } = false;
    public bool isInventoryOpened { get => inventoryManager.IsInnerOpened; set => inventoryManager.IsInnerOpened = value; }

    // Start is called before the first frame update
    void Start()
    {
        stormCubeManager = stormManager.StormCube.GetComponent<StormCubeManager>();
        inputManager.OnInteractPressed += OnInteract;
    }

    void OnInteract()
    {
        if (!isInCar)
        {
            if (cursorController.Highlighted && cursorController.Highlighted == vehicleManager.transform.parent.gameObject)
            {
                PutPlayerInVehicle(vehicleManager.gameObject);
            }
        }
        else if (isInCar)
        {
            PutPlayerOutOfVehicle(vehicleManager.gameObject);
        }

    }

    void PutPlayerInVehicle(GameObject vehicle)
    {
        isInCar = true;
        // playerController.gameObject.SetActive(false);
        virtualCamera.Follow = vehicle.gameObject.transform;
        vehicle.GetComponent<VehicleManager>().PlayerGotIn();
        playerController.transform.localScale = new Vector3(0,0,0);
    }

    void PutPlayerOutOfVehicle(GameObject vehicle)
    {
        isInCar = false;
        // playerController.gameObject.SetActive(true);
        playerController.transform.localScale = new Vector3(1,1,1);
        virtualCamera.Follow = playerController.transform.Find("PlayerCameraRoot").transform;
        vehicle.GetComponent<VehicleManager>().PlayerGotOut();
        playerController.transform.position =
            vehicle.transform.parent.Find("PlayerLeavePosition").position;
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
        Debug.Log("iPITS" + stormCubeManager.isPlayerInsideTheStorm);
        if (stormManager.timeSinceLastDamage > 1 && stormCubeManager.isPlayerInsideTheStorm)
        {
            stormCubeManager.PlayHitSound();
            healthBar.TakeDamage(stormCubeManager.Damage());
            stormManager.timeSinceLastDamage = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ManageShooting();
        ManageStorm();
    }

    void FixedUpdate()
    {
        if (isInCar)
        {
            playerController.transform.position =new Vector3(
               vehicleManager.gameObject.transform.position.x - 3,
               3f,
               vehicleManager.gameObject.transform.position.z - 3);
        }
    }
}
