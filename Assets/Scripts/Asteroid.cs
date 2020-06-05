using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 15.0f;
    [SerializeField]
    private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!GameObject.Find("Spawn_Manager").TryGetComponent<SpawnManager>(out _spawnManager))
        {
            Debug.LogError(gameObject.name + ": Spawn_Manager is NULL.");
        }   
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Laser"))
        {
            Destroy(GetComponent<Collider2D>());
            _spawnManager.StartSpawning();
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            Destroy(this.gameObject, 0.25f);
        }
    }
}
