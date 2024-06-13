using UnityEngine;

[CreateAssetMenu(fileName ="Data", menuName ="ScriptableObjects/Weapon", order =2)]
public class WeaponData : ScriptableObject
{
    public string type { get; }
    public int Damage;
    public float Range;
    public float Accuracy;
    public int PalletQuantity;
    public int FireRate;
    
}
