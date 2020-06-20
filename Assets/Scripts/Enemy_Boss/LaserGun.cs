using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : MonoBehaviour
{
    [SerializeField]
    private GameObject _explosionPrefab;

    [SerializeField]
    private GameObject _laserPrefab;
    private float _canFire = Mathf.Infinity;
    [SerializeField]
    private GameObject _fire;

    private int lives = 1;

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _canFire)
        {
            _canFire = Time.time + Random.Range(3f, 7f);
            Vector3 position = new Vector3(0f, -2.3f, 0f);
            Instantiate(_laserPrefab, transform.position + position, Quaternion.identity);
        }
    }

    public void OpenFire()
    {
        _canFire = -1;
    }

     void Damage()
    {
        lives--;
        _fire.SetActive(true);
        if(lives < 0)
        {
            GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            explosion.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            Destroy(this.gameObject, 0.25f);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
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
