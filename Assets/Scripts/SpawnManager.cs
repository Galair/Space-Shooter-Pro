using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private GameObject[] _rarePowerups;
    [SerializeField]
    private GameObject _enemyContainer;
    private bool _isPlayerAlive = true;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    public void StopSpawning()
    {
        _isPlayerAlive = false;
        _enemyContainer.SetActive(false);
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (_isPlayerAlive)
        {
            Instantiate(_enemyPrefab, new Vector3(Random.Range(-9f, 9f), 7.8f, 0), Quaternion.identity, _enemyContainer.transform);
            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        while (_isPlayerAlive)
        {
            // 10% chance for rare powerup
            if (Random.value > 0.9f)
            {
                Instantiate(_rarePowerups[Random.Range(0, _rarePowerups.Length)], new Vector3(Random.Range(-9f, 9f), 7.6f, 0), Quaternion.identity);
            }
            else
            {
                Instantiate(_powerups[Random.Range(0, _powerups.Length)], new Vector3(Random.Range(-9f, 9f), 7.6f, 0), Quaternion.identity);
            }
            yield return new WaitForSeconds(Random.Range(4f, 8f));
        }
    }
}
