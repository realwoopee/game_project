using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [HideInInspector]
    public InventoryState playerInventoryState;

    [HideInInspector]
    public Gun selectedGun;

    public TextMeshProUGUI currentAmmoAmount;
    public TextMeshProUGUI shotgunAmmoAmount;
    public TextMeshProUGUI pistolAmmoAmount;
    public TextMeshProUGUI componentAAmount;
    public TextMeshProUGUI componentBAmount;
    public TextMeshProUGUI componentCAmount;
    public TextMeshProUGUI fuelAmount;
    public TextMeshProUGUI aptechkaAmount;
    public TextMeshProUGUI shellsLeftAmount;

    private void Update()
    {
        if(!playerInventoryState) return;

        if(selectedGun)
            shellsLeftAmount.text = selectedGun.ShellsLeft.ToString();
        currentAmmoAmount.text = playerInventoryState.AmmoAmount.ToString();
        shotgunAmmoAmount.text = playerInventoryState.shotgunAmmoAmount.ToString();
        pistolAmmoAmount.text = playerInventoryState.pistolAmmoAmount.ToString();
        componentAAmount.text = playerInventoryState.componentAAmount.ToString();
        componentBAmount.text = playerInventoryState.componentBAmount.ToString();
        componentCAmount.text = playerInventoryState.componentCAmount.ToString();
        aptechkaAmount.text = playerInventoryState.aptechasAmount.ToString();
        fuelAmount.text = playerInventoryState.fuelAmount.ToString();
    }
}