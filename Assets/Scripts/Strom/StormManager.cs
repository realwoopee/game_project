using UnityEngine;

public class StormManager : MonoBehaviour
{
    [SerializeField] private int _lowerLeftCornerX;
    [SerializeField] private int _lowerLeftCornerZ;
    [Tooltip("Seconds needed to cover whole map with storm.")]
    [SerializeField] private float _devourMapTime;
    [SerializeField][Range(0, 10)]  private int _stormDPS;
    private float _millisecondsPassed;
    public float MillisecondsPassed { get => _millisecondsPassed; }
    [SerializeField] public int MapWidth;
    [SerializeField] public int MapHeight;
    [HideInInspector] public float timeSinceLastDamage;
    public float C { get => StormProgressAlongAxis(_millisecondsPassed); }

    // Start is called before the first frame update
    void Start()
    {
       _millisecondsPassed = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        //if (!isGameOnPause)
        Debug.Log("Storm Update Called");
        _millisecondsPassed += Time.deltaTime;
        timeSinceLastDamage += Time.deltaTime;
        Vector3 startingPoint = new Vector3(C - _lowerLeftCornerX, 1, _lowerLeftCornerZ);
        Vector3 direciton = new Vector3(-0.7f, 0f, 0.7f);
        Debug.DrawRay(startingPoint, direciton * 100, Color.red, 0.2f, true);
    }

    public float StormProgressAlongAxis(float time)
    {
        // Debug.Log("time: " + time + " _DMT: " + _devourMapTime);
        return (int)(time/_devourMapTime * 2 * MapWidth);
    }
}
