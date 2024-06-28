using Unity.Burst.Intrinsics;
using UnityEngine;

public class StormManager : MonoBehaviour
{
    [SerializeField] private float _lowerLeftCornerX;
    [SerializeField] private float _lowerLeftCornerZ;
    [Tooltip("Seconds needed to cover whole map with storm.")]
    [SerializeField] private float _devourMapTime;
    [SerializeField] private AudioClip _desertAmbient;
    [SerializeField][Range(0, 10)]  private int _stormDPS;
    private float _secondsPassed;
    public float MillisecondsPassed { get => _secondsPassed; }
    [SerializeField] public int MapWidth;
    [SerializeField] public int MapHeight;
    [HideInInspector] public float timeSinceLastDamage;
    public float C { get => StormProgressAlongAxis(_secondsPassed); }
    public GameObject StormCube;
    

    // Start is called before the first frame update
    void Start()
    {
       _secondsPassed = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        //if (!isGameOnPause)
        StormCube.transform.position = new Vector3(_lowerLeftCornerX + C, StormCube.transform.position.y, _lowerLeftCornerZ + C);
        _secondsPassed += Time.deltaTime;
        timeSinceLastDamage += Time.deltaTime;
    }

    public float StormProgressAlongAxis(float time)
    {
        return time/_devourMapTime * (MapWidth * 1.0f);
    }
}
