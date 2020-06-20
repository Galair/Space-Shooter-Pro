using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontGunBullet : MonoBehaviour
{
    private Vector3 _direction = Vector3.down;
    private float _speed = 6.0f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);
        if (transform.position.y < -5.2f) Destroy(this.transform.gameObject);
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
    }
}
