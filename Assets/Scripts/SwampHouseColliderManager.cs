using UnityEngine;

public class SwampHouseColliderManager : MonoBehaviour
{


    public void OnCollisionStay(Collider other)
    {
        if (other.name == "PlayerArmature")
        {
            var playerRigidbody = GameObject.Find("PlayerArmature").GetComponent<Rigidbody>();
            playerRigidbody.AddForce(0, 10, 0);
        }
    }
}
