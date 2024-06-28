using System.Collections.Generic;
using UnityEngine;

public class StormCubeManager : MonoBehaviour
{
    private bool _isPlayerInsideTheStorm;
    private bool _isVanInsideTheStorm;
    [SerializeField] private GameObject wallEye;
    [SerializeField] private GameObject wallEyeMesh;
    [SerializeField] private AudioSource _stormAmbient;
    [SerializeField] private AudioSource _hitSound1;
    [SerializeField] private AudioSource _hitSound2;
    [SerializeField] private AudioSource _hitSound3;
    private int _stormHitCounter = 0;
    private Color32 _currentColor;
    [SerializeField] public bool isPlayerInsideTheStorm { get; set; }
    [SerializeField] public bool isVanInsideTheStorm { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        _stormAmbient = transform.Find("StormAmbient").gameObject.GetComponent<AudioSource>();
        _hitSound1 = transform.Find("StormHitSound1").gameObject.GetComponent<AudioSource>();
        _hitSound2 = transform.Find("StormHitSound2").gameObject.GetComponent<AudioSource>();
        _hitSound3 = transform.Find("StormHitSound3").gameObject.GetComponent<AudioSource>();
        _isPlayerInsideTheStorm = false;
        // wallEye.GetComponent<MeshRenderer>().material.color = new Color32(255, 178, 0, 0);
        wallEye.GetComponent<MeshRenderer>().material.color = Color.Lerp(wallEye.GetComponent<MeshRenderer>().material.color, new Color32(255, 178, 0, 0), 3f);
        // _currentColor = new Color32(255, 178, 0, 0);
    }


    private void OnTriggerEnter(Collider other)
    {
        // This function is called when another collider enters the trigger volume
        Debug.Log($"Object {other.name} entered the trigger.");

        if (other.name == "PlayerArmature"){
            _isPlayerInsideTheStorm = true;
            ApplyStormEffect();
        }

        if (other.name == "Van"){
            _isVanInsideTheStorm = true;
            _isPlayerInsideTheStorm = false;
            ApplyStormEffect();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "PlayerArmature"){
            CancelStormEffect();
            _isPlayerInsideTheStorm = false;
        }

        if (other.name == "Van"){
            CancelStormEffect();
            _isVanInsideTheStorm = false;
            _isPlayerInsideTheStorm = false;
        }

        // This function is called when another collider exits the trigger volume
        Debug.Log($"Object {other.name} exited the trigger.");
    }

    public void ApplyStormEffect()
    {
        wallEye.GetComponent<MeshRenderer>().material.color = new Color32(255, 178, 0, 72);
        _stormAmbient.Play();
    }
    public void CancelStormEffect()
    {
        wallEye.GetComponent<MeshRenderer>().material.color = new Color32(255, 178, 0, 0);
        _stormAmbient.Stop();
    }
    public void PlayHitSound()
    {
        if (_stormHitCounter % 3 == 0)
            _hitSound1.Play();
        if (_stormHitCounter % 3 == 1)
            _hitSound2.Play();
        if (_stormHitCounter % 3 == 2)
            _hitSound3.Play();
        _stormHitCounter++;
    }

}
