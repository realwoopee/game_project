using System;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public PrometeoCarController CarController;

    public float leftFuelAmount;

    public void Start()
    {
        CarController.vehicleActive = false;
    }

    private void FixedUpdate()
    {
        if (CarController.vehicleActive)
            leftFuelAmount -= (float)(0.2 * Time.deltaTime);
        if (leftFuelAmount > 0) return;
        
        leftFuelAmount = 0;
        CarController.vehicleActive = false;
    }

    public void PlayerGotIn(int fuelAmountToTransfer)
    {
        CarController.vehicleActive = true;
        leftFuelAmount += fuelAmountToTransfer;
    }

    public void PlayerGotOut()
    {
        CarController.vehicleActive = false;
    }
}