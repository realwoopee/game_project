using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    public InventoryState playerInventoryState;

    [HideInInspector]
    public RectTransform inventoryBox;

    public Dictionary<string, TextMeshProUGUI> labels = new Dictionary<string, TextMeshProUGUI>();
    
    public void Start()
    {
        inventoryBox = GetComponent<RectTransform>();
        inventoryBox.gameObject.SetActive(false);
        for(var i = 0; i < gameObject.transform.childCount; i++)
        {
            var child = gameObject.transform.GetChild(i);
            labels[child.gameObject.name] = child.gameObject.GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnGUI()
    {
        if(!playerInventoryState) return;
        
        labels[Labels.ammoAmount].text =
            $"Ammo: {(playerInventoryState.shotgunEquipped ? playerInventoryState.shotgunAmmoAmount : playerInventoryState.pistolAmmoAmount)}";
        
        labels[Labels.healsAmount].text =
            $"Heals: {(playerInventoryState.aptechasAmount)}";
        
        labels[Labels.componentAAmount].text =
            $"Component A: {(playerInventoryState.componentAAmount)}";
        
        labels[Labels.componentBAmount].text =
            $"Component B: {(playerInventoryState.componentBAmount)}";
        
        labels[Labels.componentCAmount].text =
            $"Component C: {(playerInventoryState.componentCAmount)}";
        
        labels[Labels.fuelAmount].text =
            $"Fuel: {(playerInventoryState.fuelAmount)}L";
    }

    public void SetVisible(bool state)
    {
        inventoryBox.gameObject.SetActive(state);
    }

    static class Labels
    {
        public static string ammoAmount = "Ammo Amount";
        public static string healsAmount = "Heals Amount";
        public static string componentAAmount = "Component A Amount";
        public static string componentBAmount = "Component B Amount";
        public static string componentCAmount = "Component C Amount";
        public static string fuelAmount = "Fuel Amount";
    }
}