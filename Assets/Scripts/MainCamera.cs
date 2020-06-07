using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField]
    private float _shakeMagnitude = 10.0f;
    private Vector3 _initialPosition;
    [SerializeField]
    private float _shakeDuration = 0.75f;
    private float _shakeTime;
    // Start is called before the first frame update
    void Start()
    {
        _initialPosition = transform.position;
        _shakeTime = _shakeDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (_shakeTime < _shakeDuration)
        {
            transform.position = _initialPosition + Random.insideUnitSphere * _shakeMagnitude * Time.deltaTime;
            _shakeTime += Time.deltaTime;
        }
        else
        {
            transform.position = _initialPosition;
        }
    }

    public void CameraShake()
    {
        _shakeTime = 0;
    }
}
