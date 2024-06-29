using System;
using System.Collections;
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
    public HealthBarBehaviour healthBar;
    // [SerializeField] public Gun gun;
    
    public InventoryState playerInventoryState;

    [field: SerializeField]
    public PlayerState PlayerState { get; private set; } = PlayerState.OnFoot;

    public bool inventoryOpen = false;

    private void Awake()
    {
        playerInventoryState ??= ScriptableObject.CreateInstance<InventoryState>();
    }

    // Start is called before the first frame update
    void Start()
    {
        inventoryManager.SetPlayerInventoryState(playerInventoryState);
        inputManager.OnInteractPressed += OnInteract;
        //inputManager.OnShootPressed += Shoot;
        inputManager.OnReloadPressed += Reload;
        inputManager.OnHealPressed += OnHeal;
        InventoryHandlingInit();
    }

    void OnInteract()
    {
        switch (PlayerState)
        {
            case PlayerState.OnFoot:
                if (cursorController.Highlighted)
                {
                    if (cursorController.Highlighted == vehicleManager.transform.parent.gameObject)
                    {
                        PutPlayerInVehicle(vehicleManager.gameObject);
                    }

                    if (cursorController.Highlighted.TryGetComponent<InventoryValue>(out var pack))
                    {
                        var ammoAmount = playerInventoryState.shotgunEquipped ? pack.value.shotgunAmmoAmount : pack.value.pistolAmmoAmount;
                        playerInventoryState.AmmoAmount += ammoAmount;
                        playerInventoryState.aptechasAmount += pack.value.aptechasAmount;
                        playerInventoryState.componentAAmount += pack.value.componentAAmount;
                        playerInventoryState.componentBAmount += pack.value.componentBAmount;
                        playerInventoryState.componentCAmount += pack.value.componentCAmount;
                        playerInventoryState.fuelAmount += pack.value.fuelAmount;
                        pack.Consume();
                    }
                }
                break;
            case PlayerState.InVehicle:
                PutPlayerOutOfVehicle(vehicleManager.gameObject);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void OnHeal()
    {
        if(playerInventoryState.aptechasAmount <= 0) return;

        playerInventoryState.aptechasAmount--;
        healthBar.Heal(15);
    }

    void PutPlayerInVehicle(GameObject vehicle)
    {
        playerController.gameObject.SetActive(false);
        virtualCamera.Follow = vehicle.gameObject.transform;
        vehicle.GetComponent<VehicleManager>().PlayerGotIn(playerInventoryState.fuelAmount);
        playerInventoryState.fuelAmount = 0;
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
        if (playerController.Speed >= playerController.SprintSpeed )//|| inventoryManager.IsInnerOpened)//or inventoryOpened
            return;

        if(playerController.SelectedGun.CanFire)
            playerController.SelectedGun.Fire();
    }

    void Reload()
    {
        if (playerController.SelectedGun.IsReloading) return;
        
        var neededAmount = playerController.SelectedGun.MagSize - playerController.SelectedGun.ShellsLeft;
        if (neededAmount <= 0) return;

        var amountToReload = Mathf.Min(neededAmount, playerInventoryState.AmmoAmount);
        
        if(amountToReload <= 0) return;
        
        StartCoroutine(ReloadSequence(amountToReload));
        return;

        IEnumerator ReloadSequence(int amount)
        {
            playerInventoryState.AmmoAmount -= amount;
            yield return playerController.SelectedGun.Reload(amount);
        }
    }
    
    void Update()
    {
        if(inputManager.shootHeld)
            Shoot();
    }

    void InventoryHandlingInit()
    {
        inputManager.OnInventoryOpenClosePressed += ToggleInventory;
    }

    void ToggleInventory()
    {
        var newInventoryState = !inventoryOpen;
        inventoryManager.SetPlayerInventoryVisible(newInventoryState);
        cursorController.gameObject.SetActive(!newInventoryState);
        inventoryOpen = newInventoryState;
    }
}

public enum PlayerState
{
    OnFoot,
    InVehicle
}
