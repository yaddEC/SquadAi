using FSMMono;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public interface IConsideration
{
    float EvaluateRank() { return 0; }
    float EvaluateBonus() { return 1; }
    float EvaluateMultiplier() { return 1; }
}
public class DistanceToPlayerConsideration : MonoBehaviour, IConsideration
{
    [SerializeField]
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
    private List<GameObject> enemies = new List<GameObject>();
    public float shootDistance = 11;
    private float closestDistance ;
    private AIAgent _selfAiAgent;

    public void Start()
    {
        _selfAiAgent = gameObject.GetComponent<AIAgent>();
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemies.Add(enemy);
        }
    }

    public float EvaluateRank()
    {
        GameObject closestShootableEnemy = null;
        closestDistance = shootDistance;

        int layerMask = ~LayerMask.GetMask("Allies");

        foreach (var enemy in enemies)
        {
            float currentDistance = Vector3.Distance(gameObject.transform.position, enemy.transform.position);
            Vector3 direction = enemy.transform.position - transform.position;

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.gameObject.tag == "Enemy" && currentDistance < shootDistance && (currentDistance < closestDistance || closestShootableEnemy == null))
                {
                    closestDistance = currentDistance;
                    closestShootableEnemy = enemy;
                }
            }
        }

        _selfAiAgent.targetPos = (closestShootableEnemy != null && !_selfAiAgent.IsCovering) ? closestShootableEnemy.transform.position : _selfAiAgent.targetPos;
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
    public float detectionDistance = 10; 
    private float closestDistance;
    private AIAgent _selfAiAgent;
    public Vector3 closestEnemyPosition;
    private GameObject _player;
    private Collider _selfCollider;

    private float evaluationDelay = 0.5f;
    private float lastEvaluationTime = 0f;
    private float previousRank = 0f;
    private float bufferDistance = 0.5f;


    public void Start()
    {
        _selfAiAgent = gameObject.GetComponent<AIAgent>();

        _player = GameObject.FindGameObjectWithTag("Player");
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemies.Add(enemy);
        }
    }

    public float EvaluateRank()
    {
        GameObject closestVisibleEnemy = null;
        closestDistance = detectionDistance;

        foreach (var enemy in enemies)
        {
            float currentDistance = Vector3.Distance(_player.transform.position, enemy.transform.position);
            Vector3 direction = enemy.transform.position - _player.transform.position;

            if (Physics.Raycast(_player.transform.position, direction, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider != _selfCollider && hit.collider.gameObject.tag == "Enemy" && currentDistance < detectionDistance && (currentDistance < closestDistance || closestVisibleEnemy == null))
                {
                    closestDistance = currentDistance;
                    closestVisibleEnemy = enemy;
                }
            }
        }

        closestEnemyPosition = closestVisibleEnemy != null ? closestVisibleEnemy.transform.position : Vector3.zero;

        float rank = (closestDistance + 0.1f) / (detectionDistance) ;
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
    [SerializeField]
    private float detectionRadius =7.0f; 
    private GameObject _player;
    Collider[] enemiesNearPlayer;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public float EvaluateRank()
    {
        enemiesNearPlayer = Physics.OverlapSphere(_player.transform.position, detectionRadius, 1 << LayerMask.NameToLayer("Enemy"));

        if (enemiesNearPlayer.Length > 0)
        {
            return 1.0f;
        }
        return 0.0f;
    }

    public float EvaluateBonus()
    {
        // increase weight based on number of enemies
        return 1.05f * enemiesNearPlayer.Length;
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
            if (!_selfAiAgent.isGunLoaded || _selfAiAgent.IsCovering ||( AIManager.Instance.currentFormation != Formation.NONE && AIManager.Instance.currentFormation != Formation.FREEZE))
               return 0;
            else
                return 1;
     
    }

}

public class IdleDelayConsideration : MonoBehaviour, IConsideration
{
    [SerializeField]
    private float idleDelay = 2.0f;  

    private float timeWhenStopped;
    private bool hasStopped;
    private Vector3 lastCheckedPosition;
    private float moveThreshold = 0.1f;

    private AIAgent _selfAiAgent;

    public void Start()
    {
        _selfAiAgent = gameObject.GetComponent<AIAgent>();
    }

    public float EvaluateRank()
    {
        float currentDistance = Vector3.Distance(gameObject.transform.position, lastCheckedPosition);

        if (!hasStopped && currentDistance <= moveThreshold)
        {
            hasStopped = true;
            timeWhenStopped = Time.time;
        }
        else if (currentDistance > moveThreshold)
        {
            hasStopped = false;
        }

        lastCheckedPosition = gameObject.transform.position;

        

        if (hasStopped && (Time.time - timeWhenStopped) > idleDelay)
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
        {
            timeWhenStopped = Time.time;
        }
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
    private PlayerAgent _player;
    [SerializeField]
    private float criticalHealthThreshold = 0.2f; 
    [SerializeField]
    private float threatDistance = 5f; 

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAgent>();
    }

    public float EvaluateRank()
    {
        float baseRank = 1.5f- 1.5f*( _player.CurrentHP / _player.MaxHP);
        return baseRank ;
    }

    public float EvaluateBonus()
    {
        // if player health is critically low
        if (_player.CurrentHP / _player.MaxHP <= criticalHealthThreshold)
        {
            return 1.2f; 
        }
        return 0;
    }

    public float EvaluateMultiplier()
    {
        // if an enemy is too close to the AI, reduce weight slightly as AI is under threat
        Collider[] threats = Physics.OverlapSphere(transform.position, threatDistance, 1 << LayerMask.NameToLayer("Enemy"));
        if (threats.Length > 0)
        {
            return 0.8f; 
        }
        return 1;
    }
}

