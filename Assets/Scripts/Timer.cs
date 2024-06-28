using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    private float _timer;
    public float InGameTime{get => _timer;}
    // Start is called before the first frame update
    void Start()
    {
        
        _timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_gameManager.isPaused)
            _timer += Time.deltaTime;
    }
}
