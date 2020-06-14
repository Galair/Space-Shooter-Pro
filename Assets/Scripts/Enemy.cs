using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private int _enemyID = 0; //0=Enemy 1=Enemy_Green

    public enum EnemyMovementType : int
    {
        Down = 0,
        SideToSide = 1,

        MAX = SideToSide
    }

    [SerializeField]
    private EnemyMovementType _enemyMovementType = EnemyMovementType.Down;
    private float positionX;
    [SerializeField]
    private float _amplitude = 2.0f;
    [SerializeField]
    private float _frequency = 2.0f;

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
        positionX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isHit == false)
        {
            CalculateMovement();
            if (Time.time > _canFire) StartCoroutine(FireLaserRoutine());          
        }
    }

    private void CalculateMovement()
    {
        float newPositionY = transform.position.y - _speed * Time.deltaTime;
        switch (_enemyMovementType)
        {
            case EnemyMovementType.SideToSide:
                float offsetX = Mathf.Sin(Time.time * _frequency) * _amplitude;
                transform.position = new Vector3(positionX + offsetX, newPositionY, 0);
                break;
            default:
                transform.position = new Vector3(positionX, newPositionY, 0);
                break;
        }
        if (transform.position.y < -6.5f)
        {
            transform.position = new Vector3(Random.Range(-9.45f, 9.45f), 7.5f, 0);
            positionX = transform.position.x;
        }
    }

    public void SetEnemyMovementType(EnemyMovementType movementType, float speed = 4f, float amplitude = 1f, float frequency = 1f )
    {
        _enemyMovementType = movementType;
        _speed = speed;
        _amplitude = amplitude;
        _frequency = frequency;
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


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out Player player)) player.Damage();
            DestroyEnemy();
        }

        if (other.CompareTag("Laser"))
        {
            Laser laser = other.GetComponent<Laser>();
            if (laser == null)
            {
                Debug.LogError(other.name + ": Laser component is NULL.");
            }
            if (laser.IsEnemyLaser) //enemy laser
            {
                if (_frendlyFire == true)
                {
                    DestroyEnemy();
                }
            }
            else //player laser
            {
                Destroy(other.gameObject);
                if (_player != null)
                {
                    _player.AddScore(10);
                }
                DestroyEnemy();
            }
        }

        if (other.CompareTag("Missile"))
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            }
            DestroyEnemy();
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
