using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField] //0 = Triple Shot; 1 = Speed; 2 = Shields; 3 = Ammo; 4 = Health; 5 = Missile
    private int _powerupID;
    [SerializeField]
    private AudioClip _audioClip;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
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
                }
            }
            Destroy(this.gameObject);
        }
    }
}
