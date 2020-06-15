using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;

    private Player _player;
    private Animator _animator;

    [SerializeField]
    private AudioClip _explosionSound;

    [SerializeField]
    private GameObject _laserPrefab;
    private float _canFire = -1;
    private bool _isHit = false;
    [SerializeField]
    private bool _frendlyFire = false;

    [SerializeField]
    private int _enemyID = 0; //0=Enemy 1=Enemy_Green 2=Enemy_Smart 3=Enemy_Avoid

    public enum EnemyMovementType : int
    {
        Down = 0,
        SideToSide = 1,

        MAX = SideToSide
    }

    [SerializeField]
    private EnemyMovementType _enemyMovementType = EnemyMovementType.Down;
    private Vector3 _moveDirection = Vector3.down;
    [SerializeField]
    private float _sideToSideAngle = 45.0f;
    [SerializeField]
    private float _sideRotationSpeed = 1.0f;
    private bool _moveToRight = true;


    [SerializeField]
    private GameObject _shieldGameObject;

    [SerializeField]
    private float _proximitySensor = 4.0f;
    [SerializeField]
    private float _ramRotateSpeed = 0.5f;
    [SerializeField]
    private float _maxAngleToRam = 60.0f;
    private bool _ramModeActive = false;

    [SerializeField]
    private GameObject _sternGunPrefab;

    private bool _evasiveManeuvers = false;
    Coroutine _evasiveManeuversCoroutine;
    private Vector3 _evasiveDirection;

    // Start is called before the first frame update
    void Start()
    {
        if (!GameObject.FindGameObjectWithTag("Player").TryGetComponent<Player>(out _player))
        {
            Debug.LogError(gameObject.name + ": Player component is NULL.");
        }
        if(!TryGetComponent<Animator>(out _animator))
        {
            Debug.LogError(gameObject.name + ": Animator component is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isHit == false)
        {
            //if Smart Enemy
            if(_enemyID == 2)
            {
                CheckIfPlayerBehind();
            }
            if (_evasiveManeuvers)
            {
                CalculateEvasiveManeuvers();
            }
            else
            {
                CheckRamRange();
                CalculateMovement();
            }
            FireIfPowerupFront();
            if (Time.time > _canFire)
            {
                StartCoroutine(FireLaserRoutine());
            }
        }
    }

    private void CalculateEvasiveManeuvers()
    {
            _moveDirection = Vector3.RotateTowards(_moveDirection, _evasiveDirection, 15.0f * Time.deltaTime, 0f);
            transform.Translate(_moveDirection * 7.0f * Time.deltaTime);
    }
    private void CalculateMovement()
    {
        if (_ramModeActive == false)
        {
            switch (_enemyMovementType)
            {
                case EnemyMovementType.SideToSide:
                    float angle;
                    if (_moveToRight)
                    {
                        angle = Vector3.Angle(_moveDirection, Vector3.right);
                        _moveDirection = Vector3.RotateTowards(_moveDirection, Vector3.right, _sideRotationSpeed * Time.deltaTime, 0f);
                    }
                    else
                    {
                        angle = Vector3.Angle(_moveDirection, Vector3.left);
                        _moveDirection = Vector3.RotateTowards(_moveDirection, Vector3.left, _sideRotationSpeed * Time.deltaTime, 0f);
                    }
                    if (angle < _sideToSideAngle) _moveToRight = !_moveToRight;
                    break;
                default:
                    _moveDirection = Vector3.RotateTowards(_moveDirection, Vector3.down, _ramRotateSpeed * Time.deltaTime, 0f);
                    break;
            }
        }
        transform.Translate(_moveDirection * _speed * Time.deltaTime);
        Debug.DrawRay(transform.position, _moveDirection, Color.red);
        if (transform.position.y < -6.5f)
        {
            transform.position = new Vector3(Random.Range(-9.45f, 9.45f), 7.5f, 0);
            _ramModeActive = false;
            _moveDirection = Vector3.down;
        }
    }

    private void CheckRamRange()
    {
        Vector3 vectorToPlayer = _player.transform.position - transform.position;
        if ((vectorToPlayer.magnitude < _proximitySensor)&&(Vector3.Angle(Vector3.down,vectorToPlayer) < _maxAngleToRam))
        {
            _moveDirection = Vector3.RotateTowards(_moveDirection, vectorToPlayer, _ramRotateSpeed * Time.deltaTime, 0f);
            _ramModeActive = true;
        }
        else _ramModeActive = false;
    }

    private void CheckIfPlayerBehind()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0f, 1.2f, 0f), Vector2.up);
        if ((hit.collider != null) && (hit.transform.CompareTag("Player")))
        {
            Instantiate(_sternGunPrefab, transform.position + new Vector3(0f, 1.2f, 0f), Quaternion.identity);
        }
    }

    private bool FireIfPowerupFront()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0f, -1.2f, 0f), Vector2.down);
        if ((hit.collider != null) && (hit.transform.CompareTag("Powerup")))
        {
            _canFire = Time.time + Random.Range(3f, 7f);
            Instantiate(_laserPrefab, transform.position + new Vector3(0f, -1.8f, 0f), Quaternion.identity);
            return true;
        }
        return false;
    }

    public void LaserDetected(Vector3 position)
    {
        if (_evasiveManeuversCoroutine != null) StopCoroutine(_evasiveManeuversCoroutine);
        _evasiveManeuvers = true;
        if (position.x > transform.position.x) _evasiveDirection = Vector3.left;
        else _evasiveDirection = Vector3.right;
        _evasiveManeuversCoroutine = StartCoroutine(EvasiveManeuversRoutine());
    }

    IEnumerator EvasiveManeuversRoutine()
    {
        yield return new WaitForSeconds(0.25f);
        _moveDirection = Vector3.down;
        _evasiveManeuvers = false;
    }

    public void SetEnemyMovementType(EnemyMovementType movementType, float speed = 4f)
    {
        _enemyMovementType = movementType;
        _speed = speed;
    }

    IEnumerator FireLaserRoutine()
    {
        _canFire = Time.time + Random.Range(3f, 7f);
        Vector3 positionLeft = new Vector3(-0.24f, -1.8f, 0f);
        Vector3 positionRight = new Vector3(0.24f, -1.8f, 0f);
        Vector3 positionCenter = new Vector3(0f, -1.8f, 0f);

        switch (_enemyID)
        {
            case 0:
                if (Random.Range(0, 2) == 0)
                {
                    //fire left then right laser
                    Instantiate(_laserPrefab, transform.position + positionLeft, Quaternion.identity);
                    yield return new WaitForSeconds(Random.Range(0f, 0.5f));
                    Instantiate(_laserPrefab, transform.position + positionRight, Quaternion.identity);
                }
                else
                {
                    //fire right then left laser
                    Instantiate(_laserPrefab, transform.position + positionRight, Quaternion.identity);
                    yield return new WaitForSeconds(Random.Range(0f, 0.5f));
                    Instantiate(_laserPrefab, transform.position + positionLeft, Quaternion.identity);
                }
                break;
            case 1:
                Instantiate(_laserPrefab, transform.position + positionLeft, Quaternion.Euler(0, 0, -10));
                Instantiate(_laserPrefab, transform.position + positionRight, Quaternion.Euler(0, 0, 10));
                yield return new WaitForSeconds(0.2f);
                Instantiate(_laserPrefab, transform.position + positionCenter, Quaternion.identity);
                break;

        }
    }

    public void SetShieldActive(bool value)
    {
        _shieldGameObject.SetActive(value);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out Player player)) player.Damage();
            if (_shieldGameObject.activeInHierarchy) _shieldGameObject.SetActive(false);
            else DestroyEnemy();
        }

        if (other.CompareTag("Laser"))
        {
            Laser laser = other.GetComponent<Laser>();
            if (laser == null)
            {
                Debug.LogError(other.name + ": Laser component is NULL.");
            }
            if (laser.IsPlayerLaser)
            {
                Destroy(other.gameObject);
                if (_shieldGameObject.activeInHierarchy) _shieldGameObject.SetActive(false);
                else
                {
                    if (_player != null) _player.AddScore(10);
                    DestroyEnemy();
                }
            }
            else if (_frendlyFire == true) // enemy fredndly laser
            {
                Destroy(other.gameObject);
                if (_shieldGameObject.activeInHierarchy) _shieldGameObject.SetActive(false);
                else DestroyEnemy();
            }

        }

        if (other.CompareTag("Missile"))
        {
            Destroy(other.gameObject);
            if (_shieldGameObject.activeInHierarchy) _shieldGameObject.SetActive(false);
            else
            {
                if (_player != null)
                {
                    _player.AddScore(10);
                }

                DestroyEnemy();
            }
        }
    }

    void DestroyEnemy()
    {
        Destroy(GetComponent<Collider2D>());
        StopAllCoroutines();
        _speed = 0f;
        _isHit = true;
        _animator.SetTrigger("OnEnemyDeath");
        // Inactivate enemy thrusters
        for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(false);
        AudioSource.PlayClipAtPoint(_explosionSound, transform.position);
        Destroy(this.gameObject, 2.633f);
    }
}
