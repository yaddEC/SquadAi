using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public interface IConsideration
{
    float EvaluateRank() { return 1; }
    float EvaluateBonus() { return 1; }
    float EvaluateMultiplier() { return 1; }
}
public class DistanceToPlayer : MonoBehaviour, IConsideration
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
        return rank;
    }

    public float EvaluateMultiplier()
    {
        if (AIManager.Instance.currentFormation != Formation.NONE)
            return 0;
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
            return 1.0f;  
        }
        else
        {
            return 0.0f;  
        }
    }
}
public class ShouldFollowFormation : MonoBehaviour, IConsideration
{
    public float EvaluateMultiplier()
    {
        if (AIManager.Instance.currentFormation != Formation.NONE)
            return 1;
        return 0;
    }
}

