using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public CursorController cursorController;
    public ThirdPersonController playerController;
    public VehicleManager vehicleManager;
    public CinemachineVirtualCamera virtualCamera;
    public InputManager inputManager;
    public Gun gun;

    [field: SerializeField]
    public PlayerState PlayerState { get; private set; } = PlayerState.OnFoot;
    
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

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum PlayerState
{
    OnFoot,
    InVehicle
}
