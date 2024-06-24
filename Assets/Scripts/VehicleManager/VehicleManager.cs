using System;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public PrometeoCarController CarController;

    public void Start()
    {
        CarController.vehicleActive = false;
    }

    public void PlayerGotIn()
    {
        CarController.vehicleActive = true;
    }

    public void PlayerGotOut()
    {
        CarController.vehicleActive = false;
    }
}