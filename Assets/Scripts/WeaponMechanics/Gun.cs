using System.Collections;
using StarterAssets;
using UnityEngine;

//code taken from https://www.youtube.com/watch?v=cI3E7_f74MA
[RequireComponent(typeof(Animator))]
public class Gun : MonoBehaviour
{
    [SerializeField]
    private int _palletQuantity = 10;
    [SerializeField]
    private bool _addBulletSpread = true;
    [SerializeField]
    private int _damage = 10;
    [SerializeField]
    private Vector3 _hipfireDirectionVariance = new Vector3(0.1f, 0.05f, 0.1f);
    [SerializeField]
    private Vector3 _fireDirectionVariance = new Vector3(0.01f, 0.01f, 0.01f);
    [SerializeField]
    private Vector3 _palletSpreadVariance = new Vector3(0.1f, 0.05f, 0.1f);

    [SerializeField]
    private ParticleSystem _shootingSystem;
    [SerializeField]
    private Transform _bulletSpawnPoint;
    [SerializeField]
    private ParticleSystem _impactParticleSystem;
    [SerializeField]
    private TrailRenderer _bulletTrail;
    [SerializeField]
    private float _shootDelay = 0.5f;
    [SerializeField]
    private LayerMask _mask;
    private Animator _animator;
    private float _lastShotTime;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public bool Ads
    {
        get
        {
            if (Input.GetMouseButton(1))
                return true;
            else
                return false;
        }
    }

    public void Fire()
    {
        Ray ray;
        if (!(_lastShotTime + _shootDelay < Time.time))
        {
            return;
        }
        //TODO: implement object pool
        //https://www.youtube.com/watch?v=zyzqA_CPz2E
        //or
        //https://www.youtube.com/watch?v=fsDE_mO4RZM

        _lastShotTime = Time.time;
        _animator.SetBool("IsShooting", true);
        _shootingSystem.Play();
        Vector3 direction = GetDirection();

        for (int i = 0; i < _palletQuantity; i++)
        {
            Vector3 spread = GetPalletSpread(direction);
            ray = new Ray(GameObject.FindGameObjectWithTag("Player").transform.position, spread);
            CheckForColliders(ray);
            Debug.DrawRay(GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0f, 1f, 0f), spread * 10, Color.yellow, 0.2f, true);

            if (Physics.Raycast(_bulletSpawnPoint.position, spread, out RaycastHit hit, float.MaxValue, _mask))
            {
                TrailRenderer trail = Instantiate(_bulletTrail, _bulletSpawnPoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hit));
                _lastShotTime = Time.time;
            }
        }
    }

    public void CheckForColliders(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out EnemyAdvanced enemy))
            {
                Debug.Log(hit.collider.gameObject.name + " got hit!");
                enemy.GetDamage(_damage);
            }
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = GameObject.FindGameObjectWithTag("Player").transform.forward;
        // Vector3 direction = BulletSpawnPoint.forward;
        if (_addBulletSpread)
        {
            if (Ads)
            {
                direction += new Vector3(
                    Random.Range(-_fireDirectionVariance.x, _fireDirectionVariance.x),
                    Random.Range(-_fireDirectionVariance.y, _fireDirectionVariance.y),
                    Random.Range(-_fireDirectionVariance.z, _fireDirectionVariance.z)
                );
            }
            else
            {
                direction += new Vector3(
                    Random.Range(-_hipfireDirectionVariance.x, _hipfireDirectionVariance.x),
                    Random.Range(-_hipfireDirectionVariance.y, _hipfireDirectionVariance.y),
                    Random.Range(-_hipfireDirectionVariance.z, _hipfireDirectionVariance.z)
                );
            }
            direction.Normalize();
        }
        return direction;
    }

    private Vector3 GetPalletSpread(Vector3 direction)
    {
        direction += new Vector3(
            Random.Range(-_palletSpreadVariance.x, _palletSpreadVariance.x),
            Random.Range(-_palletSpreadVariance.y, _palletSpreadVariance.y),
            Random.Range(-_palletSpreadVariance.z, _palletSpreadVariance.z)
        );
        direction.Normalize();
        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, RaycastHit Hit)
    {
        float time = 0;
        Vector3 StartPosition = Trail.transform.position;

        while (time < 1)
        {
            Trail.transform.position = Vector3.Lerp(StartPosition, Hit.point, time);
            time += Time.deltaTime / Trail.time;

            yield return null;
        }
        _animator.SetBool("IsShooting", false);
        Trail.transform.position = Hit.point;
        Instantiate(_impactParticleSystem, Hit.point, Quaternion.LookRotation(Hit.normal));

        Destroy(Trail.gameObject, Trail.time);
    }
}