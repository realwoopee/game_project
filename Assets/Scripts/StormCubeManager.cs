using UnityEngine;

public class StormCubeManager : MonoBehaviour
{
    [SerializeField] private Timer _timer;
    private bool _isPlayerInsideTheStorm;
    [SerializeField] private GameObject wallEye;
    [SerializeField] private GameObject wallEyeMesh;
    [SerializeField] private AudioSource _stormAmbient;
    [SerializeField] private AudioSource _hitSound1;
    [SerializeField] private AudioSource _hitSound2;
    [SerializeField] private AudioSource _hitSound3;
    private int _stormHitCounter = 0;
    private Color32 _currentColor;
    [SerializeField] public bool isPlayerInsideTheStorm { get => _isPlayerInsideTheStorm; }
    private float _firstStageLength;
    [SerializeField] private float _lowerBorderForFirstStage;
    [SerializeField] private float _upperBorderForFirstStage;
    [SerializeField] private int _firstStageDamage;
    private float _secondStageLength;
    [SerializeField] private float _lowerBorderForSecondStage;
    [SerializeField] private float _upperBorderForSecondStage;
    [SerializeField] private int _secondStageDamage;
    private float _thirdStageLength;
    [SerializeField] private float _lowerBorderForThirdStage;
    [SerializeField] private float _upperBorderForThirdStage;
    [SerializeField] private int _thirdStageDamage;
    private float _fourthStageLength;
    [SerializeField] private float _lowerBorderForFourthStage;
    [SerializeField] private float _upperBorderForFourthStage;
    [SerializeField] private int _fourthStageDamage;
    [SerializeField] private int _endGameDamage;


    // Start is called before the first frame update
    void Start()
    {
        _firstStageLength = Random.Range(_lowerBorderForFirstStage, _upperBorderForFirstStage);
        _secondStageLength = Random.Range(_lowerBorderForSecondStage, _upperBorderForSecondStage);
        _thirdStageLength = Random.Range(_lowerBorderForThirdStage, _upperBorderForThirdStage);
        _fourthStageLength = Random.Range(_lowerBorderForFourthStage, _upperBorderForFourthStage);
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

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "PlayerArmature"){
            _isPlayerInsideTheStorm = false;
            CancelStormEffect();
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
    public int Damage()
    {
        Debug.Log("time" + _timer.InGameTime);
        if (_timer.InGameTime < _firstStageLength)
            return _firstStageDamage;
        else if (_timer.InGameTime < _secondStageLength)
            return _secondStageDamage;
        else if (_timer.InGameTime < _thirdStageDamage)
            return _thirdStageDamage;
        else if (_timer.InGameTime < _fourthStageDamage)
            return _fourthStageDamage;
        else
            return _endGameDamage;
    }
}
