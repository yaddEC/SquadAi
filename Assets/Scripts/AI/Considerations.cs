using System.Collections.Generic;
using UnityEngine;

public interface IConsideration
{
    float EvaluateRank() { return 0; }
    float EvaluateBonus() { return 1; }
    float EvaluateMultiplier() { return 1; }
}
public class DistanceToPlayerConsideration : MonoBehaviour, IConsideration
{
    public float desiredDistance ;
    private GameObject _player;

    public void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");  
    }

    public float EvaluateRank()
    {
        float rank;
        float currentDistance = Vector3.Distance(gameObject.transform.position, _player.transform.position);
        rank = Mathf.Abs(desiredDistance - currentDistance) / desiredDistance;
        return rank+0.1f;
    }
}

public class ShouldNotFollowFormationConsideration : MonoBehaviour, IConsideration
{
    public float EvaluateMultiplier()
    {
        if (AIManager.Instance.currentFormation != Formation.NONE)
            return 0;

        return 1;
    }
}


public class ClosestVisibleEnemyConsideration : MonoBehaviour, IConsideration
{
    public  float   shootDistance = 11;
    private float   closestDistance;
    private AIAgent _selfAiAgent;

    private List<GameObject> _enemies = new List<GameObject>();

    public void Start()
    {
        _selfAiAgent = gameObject.GetComponent<AIAgent>();

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            _enemies.Add(enemy);
    }

    public float EvaluateRank()
    {
        GameObject closestShootableEnemy = null;
        closestDistance = shootDistance;

        int layerMask = ~LayerMask.GetMask("Allies");

        foreach (var enemy in _enemies)
        {
            float currentDistance = Vector3.Distance(gameObject.transform.position, enemy.transform.position);
            Vector3 direction     = enemy.transform.position - transform.position;

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.gameObject.tag == "Enemy" && currentDistance < shootDistance && (currentDistance < closestDistance || closestShootableEnemy == null))
                {
                    closestDistance = currentDistance;
                    closestShootableEnemy = enemy;
                }
            }
        }

        _selfAiAgent.targetPos = (closestShootableEnemy != null && !_selfAiAgent.isCovering) ? closestShootableEnemy.transform.position : _selfAiAgent.targetPos;
        float rank = (closestDistance + 0.1f / (shootDistance)) - 1;
        return (closestShootableEnemy == null || rank < 0.2f) ? 0 : rank;
    }

    public float EvaluateBonus()
    {
        if (EvaluateRank() > 0.8f)
            return 0.2f;

        return 0;
    }
}

public class ClosestVisibleEnemyFromPlayerConsideration : MonoBehaviour, IConsideration
{
    private List<GameObject> enemies = new List<GameObject>();
    private float      _closestDistance;
    private AIAgent    _selfAiAgent;
    private GameObject _player;
    private Collider   _selfCollider;

    public Vector3 closestEnemyPosition;
    public float   detectionDistance = 10; 

    public void Start()
    {
        _selfAiAgent = gameObject.GetComponent<AIAgent>();

        _player = GameObject.FindGameObjectWithTag("Player");
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            enemies.Add(enemy);
    }

    public float EvaluateRank()
    {
        GameObject closestVisibleEnemy = null;
        _closestDistance = detectionDistance;

        foreach (var enemy in enemies)
        {
            float currentDistance = Vector3.Distance(_player.transform.position, enemy.transform.position);
            Vector3 direction     = enemy.transform.position - _player.transform.position;

            if (Physics.Raycast(_player.transform.position, direction, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider != _selfCollider && hit.collider.gameObject.tag == "Enemy" && currentDistance < detectionDistance && (currentDistance < _closestDistance || closestVisibleEnemy == null))
                {
                    _closestDistance    = currentDistance;
                    closestVisibleEnemy = enemy;
                }
            }
        }

        closestEnemyPosition = closestVisibleEnemy != null ? closestVisibleEnemy.transform.position : Vector3.zero;

        float rank = (_closestDistance + 0.1f) / (detectionDistance) ;
        return (closestVisibleEnemy == null || rank < 0.2f) ? 0 : rank;
    }

    public float EvaluateBonus()
    {
        if (EvaluateRank() > 0.8f)
            return 1.2f;

        return 1;
    }
}

public class EnemyNearConsideration : MonoBehaviour, IConsideration
{
    [SerializeField] private float _detectionRadius =7.0f; 
    private GameObject _player;
    private Collider[] _enemiesNearPlayer;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public float EvaluateRank()
    {
        _enemiesNearPlayer = Physics.OverlapSphere(_player.transform.position, _detectionRadius, 1 << LayerMask.NameToLayer("Enemy"));

        if (_enemiesNearPlayer.Length > 0)
            return 1.0f;

        return 0.0f;
    }

    public float EvaluateBonus()
    {
        // increase weight based on number of enemies
        return 1.05f * _enemiesNearPlayer.Length;
    }
}

public class CanShootConsideration : MonoBehaviour, IConsideration
{
    private AIAgent _selfAiAgent;

    public void Start()
    {
        _selfAiAgent = gameObject.GetComponent<AIAgent>();
    }

    public float EvaluateMultiplier()
    {
        if (!_selfAiAgent.isGunLoaded || _selfAiAgent.isCovering ||( AIManager.Instance.currentFormation != Formation.NONE && AIManager.Instance.currentFormation != Formation.FREEZE))
            return 0;
        else
            return 1;
    }
}

public class IdleDelayConsideration : MonoBehaviour, IConsideration
{
    [SerializeField] private float idleDelay = 2.0f;  

    private float   _moveThreshold = 0.1f;
    private float   _timeWhenStopped;
    private bool    _hasStopped;
    private Vector3 _lastCheckedPosition;
    private AIAgent _selfAiAgent;

    public void Start()
    {
        _selfAiAgent = gameObject.GetComponent<AIAgent>();
    }

    public float EvaluateRank()
    {
        float currentDistance = Vector3.Distance(gameObject.transform.position, _lastCheckedPosition);

        if (!_hasStopped && currentDistance <= _moveThreshold)
        {
            _hasStopped = true;
            _timeWhenStopped = Time.time;
        }
        else if (currentDistance > _moveThreshold)
        {
            _hasStopped = false;
        }

        _lastCheckedPosition = gameObject.transform.position;

        if (_hasStopped && (Time.time - _timeWhenStopped) > idleDelay)
        {
            return 1;
        }
        else
        {
            return 0f;  
        }
    }

    public float EvaluateMultiplier()
    {
        if (!_selfAiAgent.isGunLoaded)
            _timeWhenStopped = Time.time;

        return 1;
    }
}


public class ShouldFollowFormationConsideration : MonoBehaviour, IConsideration
{
    float EvaluateRank() { return 1; }

    public float EvaluateMultiplier()
    {
        if (AIManager.Instance.currentFormation != Formation.NONE)
            return 1;

        return 0;
    }
}

public class PlayerHealthConsideration : MonoBehaviour, IConsideration
{
    [SerializeField] private float _criticalHealthThreshold = 0.2f; 
    [SerializeField] private float _threatDistance          = 5f; 

    private PlayerAgent _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAgent>();
    }

    public float EvaluateRank()
    {
        float baseRank = 1.5f- 1.5f*( _player.currentHP / _player.maxHP);
        return baseRank ;
    }

    public float EvaluateBonus()
    {
        // if player health is critically low
        if (_player.currentHP / _player.maxHP <= _criticalHealthThreshold)
            return 1.2f; 

        return 0;
    }

    public float EvaluateMultiplier()
    {
        // if an enemy is too close to the AI, reduce weight slightly as AI is under threat
        Collider[] threats = Physics.OverlapSphere(transform.position, _threatDistance, 1 << LayerMask.NameToLayer("Enemy"));
        if (threats.Length > 0)
            return 0.8f;

        return 1;
    }
}

