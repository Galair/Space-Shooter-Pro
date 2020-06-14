using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _enemyPrefabs; //0=Enemy 1=Enemy_Green
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private GameObject[] _rarePowerups;
    [SerializeField]
    private GameObject _enemyContainer;
    private bool _isPlayerAlive = true;
    private int _enemyWaveCounter = 1;
    private bool _currentWaveEnded = false;
    private Coroutine currentWaveRoutine;
    private UIManager _uiManager;

    private void Start()
    {
        if (!GameObject.Find("Canvas").TryGetComponent<UIManager>(out _uiManager))
        {
            Debug.LogError(gameObject.name + ": The UIManager on Canvas is NULL.");
        }
    }

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
        yield return new WaitForSeconds(2f);
        while (_isPlayerAlive) {
            
            _currentWaveEnded = false;
            _uiManager.ShowWaveNumber(_enemyWaveCounter, _enemyWaveCounter * 2);
            currentWaveRoutine = StartCoroutine(SpawnEnemysWaveRoutine(_enemyWaveCounter*2));
            while ((_currentWaveEnded == false) || (_enemyContainer.transform.childCount > 0))
            {
                yield return new WaitForSeconds(1f);
            }
            _enemyWaveCounter++;
        }
    }

    IEnumerator SpawnEnemysWaveRoutine(int enemysToSpawn)
    {
        yield return new WaitForSeconds(2.0f);
        int currentEnemy = enemysToSpawn;
        Vector3 enemyPosition;
        int enemyID;
        GameObject enemy;
        do
        {
            enemyPosition = new Vector3(Random.Range(-9f, 9f), 7.8f, 0);
            enemyID = Random.Range(0, _enemyPrefabs.Length);
            enemy = Instantiate(_enemyPrefabs[enemyID], enemyPosition, Quaternion.identity, _enemyContainer.transform);
            if (enemy.TryGetComponent<Enemy>(out Enemy enemyScript))
            {
                Enemy.EnemyMovementType enemyMovementType = (Enemy.EnemyMovementType)Random.Range(0, (int)Enemy.EnemyMovementType.MAX + 1);
                float enemySpeed = Random.Range(3f, 4f);
                float enemyAmplitude = Random.Range(1f, 2f);
                float enemyFrequency = Random.Range(1f, 2f);
                enemyScript.SetEnemyMovementType(enemyMovementType, enemySpeed, enemyAmplitude, enemyFrequency);
                currentEnemy--;
            }
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        } while (_isPlayerAlive && (currentEnemy > 0));
        _currentWaveEnded = true;
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
