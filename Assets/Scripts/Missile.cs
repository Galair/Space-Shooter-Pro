using UnityEngine;

public class Missile : MonoBehaviour
{
    private Transform _target;
    private bool _wasTargetLock = false;
    [SerializeField]
    private float _missileSpeed = 10.0f;
    [SerializeField]
    private float _rotationSpeed = 4.0f;
    private Vector3 _missileDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        _missileDirection = Vector3.up;
    }
    // Update is called once per frame
    void Update()
    {
        if(_wasTargetLock == false)
        {
            LockTarget();
        }
        CalculateMovement();
        if ((transform.position.x < -11f) || (transform.position.x > 11) || (transform.position.y < -6f) || (transform.position.y > 8))
        {
            Destroy(this.gameObject);
        }
    }

    private void CalculateMovement()
    {
        if (_target != null)
        {
            if (_target.position.y < -5.0f)
            {
                _target = null;
            }
            else
            {
                Vector3 targetDirection = (_target.position - transform.position);
                _missileDirection = Vector3.RotateTowards(_missileDirection, targetDirection, _rotationSpeed * Time.deltaTime, 0.0f);
                _missileDirection.z = 0.0f;
            }
        }
        transform.Translate(_missileDirection * _missileSpeed * Time.deltaTime);
    }

    private void LockTarget()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
        float minDistans = float.PositiveInfinity;
        int enemyToTarget = -1;
        float enemyDistance;
        for (int i = 0; i < enemys.Length; i++)
        {
            if (enemys[i].GetComponent<Collider2D>() && (enemys[i].transform.position.y > transform.position.y))
            {
                enemyDistance = Vector3.Distance(transform.position, enemys[i].transform.position);
                if (enemyDistance < minDistans)
                {
                    minDistans = enemyDistance;
                    enemyToTarget = i;
                }
            }
        }
        if (enemyToTarget > -1)
        {
            _target = enemys[enemyToTarget].transform;
            _wasTargetLock = true;
        }
    }
}
