using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private float _speed = 8.0f;
    [SerializeField]
    private bool _isEnemyLaser = false;
    public bool IsEnemyLaser { get => _isEnemyLaser; }
    public bool IsPlayerLaser { get => !_isEnemyLaser; }

    // Update is called once per frame
    void Update()
    {
        if (_isEnemyLaser)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
            if (transform.position.y < -5.2f) Destroy(this.transform.gameObject);
        }
        else //is Player Laser
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
            if (transform.position.y > 7.0f) Destroy(this.transform.gameObject);
        }
    }

    public void SetEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    private void OnDestroy()
    {
        // Update method won't destroy parent when all lasers in tripleshot hit enemies before out of screen 
        // if laser is last child of tripleShot destroy also tripleShot
        if (transform.parent != null)
        {
            if (transform.parent.childCount < 2) Destroy(transform.parent.gameObject);
        }
    }
}
