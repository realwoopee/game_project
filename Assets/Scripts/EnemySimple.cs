using UnityEngine;

public class EnemySimple : MonoBehaviour
{
    [SerializeField] GameObject _playerArmature;
    [SerializeField] PlayerManager _playerManager;
    [SerializeField] private int _damage;
    [SerializeField] private int _health;
    [SerializeField] private float _speed;
    private Animator _animator;
    private bool _isSeeingPlayer;
    private bool _isStaggered;
    private bool _isDead;
    private float _timeSinceLastAttack;
    private float _timeSinceLastHit;
    private CharacterController _controller;
    [SerializeField] float _visionDistance;
    // Start is called before the first frame update
    void Start()
    {
        _playerArmature = GameObject.Find("PlayerArmature");
        _playerManager = GameObject.FindObjectOfType<PlayerManager>();
        _isDead = false;
        _controller = GetComponent<CharacterController>();
        _isStaggered = false;
        _animator = GetComponent<Animator>();
        _animator.SetBool("isIdle", true);
        _animator.SetBool("isRunning", false);
    }

    // Update is called once per frame
    void Update()
    {
        _timeSinceLastAttack += Time.deltaTime;
        _timeSinceLastHit += Time.deltaTime;
        if (_timeSinceLastAttack > 2 && _timeSinceLastHit > 2)
            _isStaggered = false;
        else
            _isStaggered = true;
        TryToSpotPlayer();
        Attack();
        AliveCheck();
    }

    void FixedUpdate()
    {
        Move();
    }

    public void TryToSpotPlayer()
    {
        if (Vector3.Distance(_playerArmature.transform.position, transform.position) < _visionDistance)
            _isSeeingPlayer = true;
        else
            _isSeeingPlayer = false;
    }

    public void Move()
    {
        if (_isSeeingPlayer && !_isStaggered && !_isDead){
            _animator.SetBool("isRunning", true);
            _animator.SetBool("isIdle", false);
            Vector3 targetDirection = (_playerArmature.transform.position - transform.position).normalized;
            Vector3 newPosition = transform.position + (targetDirection * _speed * Time.deltaTime);
            transform.position = newPosition;
            _controller.Move(targetDirection * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, 0.0f, 0.0f) * Time.deltaTime);
        }
        else{
            _animator.SetBool("isRunning", false);
            if (!_isStaggered)
                _animator.SetBool("isIdle", true);

        }
    }
    public void Attack()
    {
        if (Vector3.Distance(transform.position, _playerArmature.transform.position) < 2 && _timeSinceLastAttack > 2 && !_isStaggered){
            _isStaggered = true;
            _animator.SetTrigger("Attack");
            _timeSinceLastAttack = 0;
            _playerManager.healthBar.TakeDamage(_damage);
        }
    }
    public void GetDamage(int damage)
    {
        _animator.SetTrigger("Hit");
        _animator.SetBool("isRunning", false);
        _animator.SetBool("isIdle", false);
        _timeSinceLastHit = 0;
        _health -= damage;
    }
    public void AliveCheck()
    {
        if (_health <= 0){
            _animator.SetTrigger("Agony");
            _isDead = true;
            Destroy(gameObject, 4);
        }
    }
}
