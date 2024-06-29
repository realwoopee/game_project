using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


public class InventoryValue : MonoBehaviour
{
    public InventoryState value;

    public AudioClip breakSound;
    
    private void Start()
    {
        var ammoCount = Random.Range(0, 20);

        if (value) return;
        
        value = ScriptableObject.CreateInstance<InventoryState>();
        
        value.pistolAmmoAmount = Random.Range(0, 30);
        value.shotgunAmmoAmount = Random.Range(0, 15);
        value.aptechasAmount = Random.Range(0, 3);
        value.componentAAmount = Random.Range(0, 10);
        value.componentBAmount = Random.Range(0, 5);
        value.componentCAmount = Random.Range(0, 2);
        value.fuelAmount = Random.Range(0, 100);
    }

    public void Consume()
    {
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        AudioSource.PlayClipAtPoint(breakSound, transform.position);
        yield return new WaitForSeconds(breakSound.length);
        Destroy(gameObject);
    }
}