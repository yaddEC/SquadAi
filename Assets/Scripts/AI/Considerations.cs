using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConsideration
{
    float EvaluateRank();
    float EvaluateBonus() { return 1; }
    float EvaluateMultiplier() { return 1; }
}
public class DistanceToPlayer : MonoBehaviour, IConsideration
{
    [SerializeField]
    public float desiredDistance = 5.0f;

    public float EvaluateRank()
    {
        float rank;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float currentDistance = Vector3.Distance(gameObject.transform.position, player.transform.position);
        rank = Mathf.Abs(desiredDistance - currentDistance) / desiredDistance;
        return rank;
    }

}
public class ProximityToPlayer : MonoBehaviour, IConsideration
{
    public float EvaluateRank()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float currentDistance = Vector3.Distance(gameObject.transform.position, player.transform.position);

        // Ensure we don't divide by zero
        if (currentDistance == 0)
            return float.MaxValue;

        return 1.0f / currentDistance;
    }
}

