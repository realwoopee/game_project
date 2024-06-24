using UnityEngine;

public class Firearm : Weapon, IShooting 
{
    //The angle that makes a cone shape from barrel into the space that shows possible bullet spread(in radians)
    public float AccuracyAngle { get; set;}

    private Ray ray;

    public bool Ads {
        get{
            if (Input.GetMouseButtonDown(1))
                return true;
            else
                return false;
        }
    }

    public void Fire()
    {
        Vector3 adsDirection = new Vector3(Mathf.Cos((-GameObject.FindGameObjectWithTag("Player").transform.eulerAngles.y + 90) * Mathf.Deg2Rad), 0, Mathf.Sin(( -GameObject.FindGameObjectWithTag("Player").transform.eulerAngles.y + 90) * Mathf.Deg2Rad));
        RaycastHit hit;
        ray = new Ray(GameObject.FindGameObjectWithTag("Player").transform.position, adsDirection.normalized);
        CheckForColliders();
        Debug.DrawRay(GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0f, 1f, 0f), adsDirection.normalized * 10, Color.yellow, 0.2f, true);
    }

public void CheckForColliders()
{
    Debug.Log("!");
    if (Physics.Raycast(ray, out RaycastHit hit))
    {
        Debug.Log(hit.collider.gameObject.name + " got hit!");
    }
}

    public void Hipfire()
    {
        throw new System.NotImplementedException();
    }
}
