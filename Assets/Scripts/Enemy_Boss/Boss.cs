using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEditor;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private GameObject _lasers, _frontGunBullet;
    private bool _checkIfLasersAlive = true;
    [SerializeField]
    private GameObject _phaseShield;
    [SerializeField]
    private AudioClip _explosionSound;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private GameObject[] _fires;

    private int _lives = 4;

    private Player _player;

    private Vector3 _destination = new Vector3(0f, 3f, 0f);
    private Vector3 _direction = Vector3.down;
    private float _speed = 3.0f;
    private bool _isInPosition = false;

    private void Start()
    {
        GameObject.Find("Player").TryGetComponent(out _player);
    }
    // Update is called once per frame
    void Update()
    {
        if(_checkIfLasersAlive && _lasers.transform.childCount == 0)
        {
            _phaseShield.SetActive(false);
            _checkIfLasersAlive = false;
            StartCoroutine(FrontGunFireRoutine());
        }
        transform.Translate(_direction * _speed * Time.deltaTime);
        if (!_isInPosition)
        {
            if (transform.position.y < _destination.y)
            {
                _isInPosition = true;
                _speed = 0.65f;
                StartCoroutine(MovementRoutine());
                StartCoroutine(LaseGunFireRoutine());
            }
        }
    }

    IEnumerator MovementRoutine()
    {
        while (true)
        {
            _direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
            yield return new WaitForSeconds(1.2f);
            _direction = (_destination - transform.position).normalized;
            yield return new WaitForSeconds(1.3f);
        }
    }

    private void Damage()
    {
        AudioSource.PlayClipAtPoint(_explosionSound, transform.position);
        if (_phaseShield.activeInHierarchy) return;
        _lives--;
        switch (_lives)
        {
            case 3:
                _fires[Random.Range(0, 2)].SetActive(true);
                break;
            case 2:
                _fires[0].SetActive(true);
                _fires[1].SetActive(true);
                break;
            case 1:
                _fires[2].SetActive(true);
                break;
            case 0:
                GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                explosion.transform.localScale = this.transform.localScale;
                StopAllCoroutines();
                if (_player != null) _player.Won();
                Destroy(this.gameObject, 0.25f);
                break;
            default:
                break;
        }
    }

    IEnumerator LaseGunFireRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        LaserGun[] laserguns = _lasers.GetComponentsInChildren<LaserGun>();
        foreach (LaserGun lasergun in laserguns)
        {
            lasergun.OpenFire();
            yield return new WaitForSeconds(Random.Range(0.2f, 1.0f));
        }
    }

    IEnumerator FrontGunFireRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        Vector3 position, direction, destination;
        float witchGun = 0.0f;
        GameObject frontGunBullet;
        while (true)
        {
            if (_player != null)
            {
                witchGun = (_player.transform.position - transform.position).x;
            }
            if (witchGun < 0)
            {
                position = new Vector3(-0.7f, -1.6f, 0f);
                direction = new Vector3(-1f, -1f, 0f).normalized;
                destination = Vector3.right;
            }
            else
            {
                position = new Vector3(0.7f, -1.6f, 0f);
                direction = new Vector3(1f, -1f, 0f).normalized;
                destination = Vector3.left;
            }

            for (int i = 0; i < 10; i++)
            {
                frontGunBullet = Instantiate(_frontGunBullet, transform.position + position, Quaternion.identity);
                if (frontGunBullet.TryGetComponent<FrontGunBullet>(out FrontGunBullet bullet))
                {
                    bullet.SetDirection(Vector3.RotateTowards(direction, destination, 0.12f * i, 0f));
                    yield return new WaitForSeconds(0.2f);
                }
            }
            yield return new WaitForSeconds(Random.Range(1.5f, 3.0f));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _player.Damage();
        }

        if (other.CompareTag("Laser"))
        {
            if (other.TryGetComponent<Laser>(out Laser laser) && laser.IsPlayerLaser)
            {
                Destroy(other.gameObject);
                Damage();
            }
        }

        if (other.CompareTag("Missile"))
        {
            Destroy(other.gameObject);
            Damage();
        }
    }
}
