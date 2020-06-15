using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField] //0=Triple Shot; 1=Speed; 2=Shields; 3=Ammo; 4=Health; 5=Missile; 6=Steer(Negative)
    private int _powerupID;
    [SerializeField]
    private AudioClip _audioClip;
    [SerializeField]
    private GameObject _explosionPrefab;
    private bool _pickupCollectorActive = false;
    private GameObject _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if(_player==null)
        {
            Debug.LogError(gameObject.name + ": Player is NULL.");
        }
    }
    // Update is called once per frame
    void Update()
    {
        _pickupCollectorActive = Input.GetKey(KeyCode.C);
        CalculateMovement();
    }

    void CalculateMovement()
    {
        if ((_pickupCollectorActive)&&(_player!=null))
        {
            Vector3 direction = (_player.transform.position - transform.position).normalized;
            transform.Translate(direction * _speed * 2.0f * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        
        if (transform.position.y < -6f) Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
            if (other.TryGetComponent(out Player player))
            {
                switch (_powerupID)
                {
                    case 0: 
                        player.TripleShotActive();
                        break;
                    case 1: 
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    case 3:
                        player.AmmoCollected();
                        break;
                    case 4:
                        player.HealthCollected();
                        break;
                    case 5:
                        player.MissileActive();
                        break;
                    case 6:
                        player.ReverseSteeringActive();
                        break;
                }
            }
            Destroy(this.gameObject);
        }

        if (other.CompareTag("Laser"))
        {
            Laser laser = other.GetComponent<Laser>();
            if (laser == null)
            {
                Debug.LogError(other.name + ": Laser component is NULL.");
            }
            if (laser.IsEnemyLaser)
            {
                Destroy(GetComponent<Collider2D>());
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                Destroy(other.gameObject);
                Destroy(this.gameObject, 0.25f);
            }

        }
    }
}
