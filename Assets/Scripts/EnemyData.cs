using UnityEngine;

[CreateAssetMenu(fileName ="Data", menuName ="ScriptableObjects/Enemy", order =1)]
public class EnemyData : ScriptableObject
{

    public int Health;
    public int Damage; 
    public float Speed;

}
