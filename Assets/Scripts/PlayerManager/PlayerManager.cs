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
    public PlayerInventoryController playerInventoryController;
    public HealthBarManager healthBar;
    // [SerializeField] public Gun gun;
    public StormManager stormManager;
    public StormCubeManager stormCubeManager;
    
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
        playerInventoryController.selectedGun = playerController.SelectedGun;
        playerInventoryController.playerInventoryState = playerInventoryState;
        inputManager.OnInteractPressed += OnInteract;
        //inputManager.OnShootPressed += Shoot;
        inputManager.OnReloadPressed += Reload;
        inputManager.OnHealPressed += OnHeal;
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
                        if (!playerInventoryState.shotgunEquipped && pack.value.shotgunEquipped)
                        {                        
                            playerInventoryState.shotgunEquipped = true;
                            playerController.SelectedGun = playerController.gameObject.transform.Find("Shotgun")
                                .GetComponent<Gun>();
                        }
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
        PlayerState = PlayerState.InVehicle;
        //playerController.gameObject.SetActive(false);
        cursorController.highlightEnabled = false;
        playerController.transform.localScale = Vector3.zero;
        virtualCamera.Follow = vehicle.gameObject.transform;
        vehicle.GetComponent<VehicleManager>().PlayerGotIn(playerInventoryState.fuelAmount);
        playerInventoryState.fuelAmount = 0;
    }

    void PutPlayerOutOfVehicle(GameObject vehicle)
    {
        playerController.transform.position = vehicleManager.transform.parent.Find("PlayerLeavePosition").position;
        playerController.transform.localScale = Vector3.one;
        PlayerState = PlayerState.OnFoot;
        cursorController.highlightEnabled = true;
        //playerController.gameObject.SetActive(true);
        virtualCamera.Follow = playerController.transform.Find("PlayerCameraRoot").transform;
        vehicle.GetComponent<VehicleManager>().PlayerGotOut();
    }

    void Shoot()
    {
        if(PlayerState == PlayerState.InVehicle) return;
        
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
    
    void ManageStorm()
    {
        if (!(stormManager.timeSinceLastDamage > 1) || !stormCubeManager.isPlayerInsideTheStorm) return;
        stormCubeManager.PlayHitSound();
        healthBar.TakeDamage(stormCubeManager.Damage());
        stormManager.timeSinceLastDamage = 0;
    }
    
    void Update()
    {
        if(inputManager.shootHeld)
            Shoot();
        
        playerInventoryController.selectedGun = playerController.SelectedGun;
        
        ManageStorm();
    }

    private void FixedUpdate()
    {
        if (PlayerState == PlayerState.InVehicle)
        {
            playerController.transform.position = vehicleManager.transform.parent.Find("PlayerLeavePosition").position;
        }
    }
}

public enum PlayerState
{
    OnFoot,
    InVehicle
}
