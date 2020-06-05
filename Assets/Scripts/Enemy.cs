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
    private bool _isNotHit = true;
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
        CalculateMovement();
        if ((_isNotHit) && (Time.time > _canFire)) StartCoroutine(FireLaserRoutine());
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -5.37f) transform.position = new Vector3(Random.Range(-9.45f, 9.45f), 6.93f, 0);
    }

    IEnumerator FireLaserRoutine()
    {
        _canFire = Time.time + Random.Range(3f, 7f);
        Vector3 positionLeft = new Vector3(-0.24f, -1.8f, 0f);
        Vector3 positionRight = new Vector3(0.24f, -1.8f, 0f);
        GameObject enemyLaserLeft, enemyLaserRight;
        Laser laser;
        if (Random.Range(0,2)==0)
        {
            //fire left then right laser
            enemyLaserLeft = Instantiate(_laserPrefab, transform.position + positionLeft, Quaternion.identity);
            if (enemyLaserLeft.TryGetComponent<Laser>(out laser)) laser.SetEnemyLaser();
            yield return new WaitForSeconds(Random.Range(0f,0.5f));
            enemyLaserRight = Instantiate(_laserPrefab, transform.position + positionRight, Quaternion.identity);
            if (enemyLaserRight.TryGetComponent<Laser>(out laser)) laser.SetEnemyLaser();
        }
        else
        {
            //fire right then left laser
            enemyLaserRight = Instantiate(_laserPrefab, transform.position + positionRight, Quaternion.identity);
            if (enemyLaserRight.TryGetComponent<Laser>(out laser)) laser.SetEnemyLaser();
            yield return new WaitForSeconds(Random.Range(0f, 0.5f));
            enemyLaserLeft = Instantiate(_laserPrefab, transform.position + positionLeft, Quaternion.identity);
            if (enemyLaserLeft.TryGetComponent<Laser>(out laser)) laser.SetEnemyLaser();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(GetComponent<Collider2D>());
            StopAllCoroutines();
            _speed = 0f;
            _isNotHit = false;
            if (other.TryGetComponent(out Player player)) player.Damage();
            _animator.SetTrigger("OnEnemyDeath");
            // Inactivate enemy thrusters
            for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(false);
            AudioSource.PlayClipAtPoint(_explosionSound, transform.position);
            Destroy(this.gameObject, 2.633f);
        }

        if (other.CompareTag("Laser"))
        {
            Destroy(GetComponent<Collider2D>());
            StopAllCoroutines();
            _speed = 0f;
            _isNotHit = false;
            if (_player != null)
            {
                if (other.TryGetComponent<Laser>(out Laser laser) && laser.IsPlayerLaser)
                {
                    _player.AddScore(10);
                }
            }
            _animator.SetTrigger("OnEnemyDeath");
            // Inactivate enemy thrusters
            for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(false);
            AudioSource.PlayClipAtPoint(_explosionSound, transform.position);
            Destroy(other.gameObject);
            Destroy(this.gameObject, 2.633f);
        }
    }
}
