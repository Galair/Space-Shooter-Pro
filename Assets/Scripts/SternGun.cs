using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SternGun : MonoBehaviour
{
    private float _speed = 6.0f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if (transform.position.y > 7.0f) Destroy(this.transform.gameObject);
    }
}
