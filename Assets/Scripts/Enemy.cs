using StarterAssets;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _speed;
    [SerializeField] private int _health;
    [SerializeField] private EnemyData _data;
    private Transform _targetPosition;
    private bool _targetInRange;

    void Start()
    {
        _targetPosition = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Swarm();
    }

    void Swarm()
    {
        if (_targetPosition == null)
        {
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition.position, _speed * Time.deltaTime);
    }
}