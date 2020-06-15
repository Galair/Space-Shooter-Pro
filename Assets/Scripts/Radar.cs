using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    private Enemy _enemy;

    private void Start()
    {
        if (!transform.parent.gameObject.TryGetComponent<Enemy>(out _enemy))
        {
            Debug.LogError(this.name + ": Enemy component on parent is NULL.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Laser"))
        {
            if(!other.TryGetComponent<Laser>(out Laser laser))
            {
                Debug.LogError(other.name + ": Laser component is NULL.");
            }
            if (laser.IsPlayerLaser) _enemy.LaserDetected(other.transform.position);
        }   
    }
}
