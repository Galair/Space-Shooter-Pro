using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _normalSpeed = 4.0f;
    [SerializeField]
    private float _speedBoostMultiplier = 1.5f;
    [SerializeField]
    private float _thrusterBoostMultiplier = 2.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private float _fireRate = 0.2f;
    private float _canFire = -1.0f;
    [SerializeField]
    private int _lives = 3;
    
    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private int _shieldStrength = 0;
    private Coroutine _tripleShotCoroutine;
    private Coroutine _speedBoostCoroutine;
    [SerializeField]
    private GameObject _shieldGameObject, _thrusterBoostGameObject;
    [SerializeField]
    GameObject[] _engines;

    [SerializeField]
    private int _score;
    private UIManager _uiManager;

    [SerializeField]
    private AudioClip _laserSound, _explosionSound, _ammoOutSound;

    [SerializeField]
    private int _maxAmmoCount = 15;
    private int _ammoCount;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Vector3.zero;
        if (!GameObject.Find("Spawn_Manager").TryGetComponent<SpawnManager>(out _spawnManager))
        {
            Debug.LogError(gameObject.name + ": The Spawn Manager is NULL.");
        }
        if (!GameObject.Find("Canvas").TryGetComponent<UIManager>(out _uiManager))
        {
            Debug.LogError(gameObject.name + ": The UIManager on Canvas is NULL.");
        }
        _uiManager.SetMaxAmmoCount(_maxAmmoCount);
        _ammoCount = _maxAmmoCount;
        _uiManager.UpdateAmmo(_ammoCount);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && (Time.time > _canFire))
        {
            if (_ammoCount == 0)
            {
                AudioSource.PlayClipAtPoint(_ammoOutSound, transform.position);
            }
            else
            {
                _uiManager.UpdateAmmo(--_ammoCount);
                FireLaser();
            }
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        Vector3 velocity = direction * _normalSpeed;
        
        if (_isSpeedBoostActive) velocity *= _speedBoostMultiplier;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocity *= _thrusterBoostMultiplier;
            _thrusterBoostGameObject.SetActive(true);
        }
        else
        {
            _thrusterBoostGameObject.SetActive(false);
        }
        
        transform.Translate(velocity * Time.deltaTime);
        
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);
        if (transform.position.x > 11.3) transform.position = new Vector3(-11.3f, transform.position.y, 0);
        else if (transform.position.x < -11.3) transform.position = new Vector3(11.3f, transform.position.y, 0);
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }
        AudioSource.PlayClipAtPoint(_laserSound, transform.position);
    }

    public void Damage()
    {
        if(_shieldStrength > 0)
        {
            _uiManager.UpdateShield(--_shieldStrength);
            if (_shieldGameObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer shield))
            {
                shield.color = new Color(1f, 1f, 1f, _shieldStrength / 3.0f);
            }
            return;
        }

        _uiManager.UpdateLive(--_lives);
        switch (_lives)
        {
            case 2: 
                _engines[Random.Range(0, 2)].SetActive(true);
                break;
            case 1: 
                _engines[0].SetActive(true);
                _engines[1].SetActive(true);
                break;
            case 0:
                _spawnManager.StopSpawning();
                Destroy(transform.gameObject);
                break;
            default: 
                break;
        }
    }

    public void TripleShotActive()
    {
        if (_tripleShotCoroutine != null) StopCoroutine(_tripleShotCoroutine);
        _isTripleShotActive = true;
        _tripleShotCoroutine = StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        if (_speedBoostCoroutine != null) StopCoroutine(_speedBoostCoroutine);
        _isSpeedBoostActive = true;
        _speedBoostCoroutine = StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isSpeedBoostActive = false;
    }

    public void ShieldActive()
    {
        _shieldStrength = 3;
        _uiManager.UpdateShield(_shieldStrength);
        if (_shieldGameObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer shield))
        {
            shield.color = new Color(1f, 1f, 1f, _shieldStrength / 3.0f);
        }
    }

    public void AddScore(int scoreToAdd)
    {
        _score += scoreToAdd;
        _uiManager.UpdateScore(_score);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Laser"))
        {
            if (other.TryGetComponent<Laser>(out Laser laser) && laser.IsEnemyLaser)
            {
                Destroy(other.gameObject);
                Damage();
                AudioSource.PlayClipAtPoint(_explosionSound, transform.position);
            }
        }
        
    }
}
