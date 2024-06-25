using System.Collections;
using UnityEngine;

//code taken from https://www.youtube.com/watch?v=cI3E7_f74MA
[RequireComponent(typeof(Animator))]
public class Gun : MonoBehaviour
{
    [SerializeField]
    private int PalletQuantity = 10;
    [SerializeField]
    private bool AddBulletSpread = true;
    [SerializeField]
    private Vector3 HipfireDirectionVariance;
    [SerializeField]
    private Vector3 FireDirectionVariance;
    [SerializeField]
    private Vector3 PalletSpreadVariance;
    [SerializeField]
    private ParticleSystem ShootingSystem;
    [SerializeField]
    private Transform BulletSpawnPoint;
    [SerializeField]
    private ParticleSystem ImpactParticleSystem;
    [SerializeField]
    private TrailRenderer BulletTrail;
    [SerializeField]
    private float ShootDelay = 0.5f;
    [SerializeField]
    private LayerMask Mask;
    private Animator Animator;
    private float LastShotTime;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
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
        if (LastShotTime + ShootDelay < Time.time)
        {
            //TODO: implement object pool
            //https://www.youtube.com/watch?v=zyzqA_CPz2E
            //or
            //https://www.youtube.com/watch?v=fsDE_mO4RZM

            Animator.SetBool("IsShooting", true);
            ShootingSystem.Play();
            Vector3 direction = GetDirection();

            for (int i = 0; i < PalletQuantity; i++)
            {
                Vector3 spread = GetPalletSpread(direction);
                ray = new Ray(GameObject.FindGameObjectWithTag("Player").transform.position, spread);
                CheckForColliders(ray);
                Debug.DrawRay(GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0f, 1f, 0f), spread * 10, Color.yellow, 0.2f, true);

                if (Physics.Raycast(BulletSpawnPoint.position, spread, out RaycastHit hit, float.MaxValue, Mask))
                {
                    TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
                    StartCoroutine(SpawnTrail(trail, hit));
                    LastShotTime = Time.time;
                }
            }
        }
    }

    public void CheckForColliders(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log(hit.collider.gameObject.name + " got hit!");
        }
    }

    private Vector3 GetDirection()
    {
        // Vector3 direction = GameObject.FindGameObjectWithTag("Player").transform.forward;
        Vector3 direction = BulletSpawnPoint.forward;
        if (AddBulletSpread)
        {
            if (Ads)
            {
                direction += new Vector3(
                    Random.Range(-FireDirectionVariance.x, FireDirectionVariance.x),
                    Random.Range(-FireDirectionVariance.y, FireDirectionVariance.y),
                    Random.Range(-FireDirectionVariance.z, FireDirectionVariance.z)
                );
            }
            else
            {
                direction += new Vector3(
                    Random.Range(-HipfireDirectionVariance.x, HipfireDirectionVariance.x),
                    Random.Range(-HipfireDirectionVariance.y, HipfireDirectionVariance.y),
                    Random.Range(-HipfireDirectionVariance.z, HipfireDirectionVariance.z)
                );
            }
            direction.Normalize();
        }
        return direction;
    }

    private Vector3 GetPalletSpread(Vector3 direction)
    {
        Debug.Log(direction.ToString());
        Debug.Log(PalletSpreadVariance.x);
        float test1 = Random.Range(-PalletSpreadVariance.x, PalletSpreadVariance.x);
        float test2 = Random.Range(-PalletSpreadVariance.y, PalletSpreadVariance.y);
        float test3 = Random.Range(-PalletSpreadVariance.z, PalletSpreadVariance.z);
        // Debug.Log(test1);
        // Debug.Log(test2);
        // Debug.Log(test3);
        direction += new Vector3(
            Random.Range(-PalletSpreadVariance.x, PalletSpreadVariance.x),
            Random.Range(-PalletSpreadVariance.y, PalletSpreadVariance.y),
            Random.Range(-PalletSpreadVariance.z, PalletSpreadVariance.z)
        );
        Debug.Log(direction.ToString());
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
        Animator.SetBool("IsShooting", false);
        Trail.transform.position = Hit.point;
        Instantiate(ImpactParticleSystem, Hit.point, Quaternion.LookRotation(Hit.normal));

        Destroy(Trail.gameObject, Trail.time);
    }
}