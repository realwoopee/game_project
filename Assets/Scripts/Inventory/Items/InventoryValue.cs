using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class InventoryValue : MonoBehaviour
{
    public InventoryState value;

    private void Start()
    {
        var ammoCount = Random.Range(0, 20);

        if (value) return;
        
        value = ScriptableObject.CreateInstance<InventoryState>();
            
        value.pistolAmmoAmount = ammoCount;
        value.shotgunAmmoAmount = ammoCount / 3;
        value.aptechasAmount = Random.Range(0, 3);
        value.componentAAmount = Random.Range(0, 10);
        value.componentBAmount = Random.Range(0, 5);
        value.componentCAmount = Random.Range(0, 2);
        value.fuelAmount = Random.Range(0, 100);
    }

    public void Consume()
    {
        Destroy(gameObject);
    }
}