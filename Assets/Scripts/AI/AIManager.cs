using FSMMono;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> allies;

    // to avoid updating the AIs every frame, we add a number of n frame, so that the change of behavior is calculated each n frame 
    [SerializeField]
    private int _aiReactionDelay;
    private int _currentFrame;
    private GameObject _player; 
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        foreach (GameObject ally in GameObject.FindGameObjectsWithTag("AlliesAI"))
        {
            allies.Add(ally);
        }

    }

    void Update()
    {
        _currentFrame++;

        if (_currentFrame < _aiReactionDelay)
            return;

        _currentFrame = 0;

        foreach (GameObject ally in allies)
        {
            UtilityBehavior bestBehavior = GetBestBehavior(ally);
            AIAgent aIAgent = ally.GetComponent<AIAgent>();

            if (aIAgent.CurrentBehavior != bestBehavior || aIAgent.CurrentBehavior == null)
            {
                aIAgent.SetBehavior(bestBehavior);
            }

        }
    }

    UtilityBehavior GetBestBehavior(GameObject ally) // inspired by the dual utility reasoner
    {
        UtilityBehavior bestBehavior = null;
        float highestRank = float.NegativeInfinity;
        float totalWeight = 0;
        float highestWeight = float.NegativeInfinity;

        // get rank and weight to determine priority
        foreach (UtilityBehavior behavior in ally.GetComponents<UtilityBehavior>())
        {
            float currentRank = behavior.GetRank();
            float currentWeight = behavior.GetWeight();

            if (currentWeight > 0) // remove all weights that are equal to 0
            {
                totalWeight += currentWeight;

                if (currentRank > highestRank)
                {
                    // keep in memory the highest rank
                    highestRank = currentRank;
                    highestWeight = currentWeight;
                }
            }
        }

        // determine the highest rank category and eliminate options with lower rank
        List<UtilityBehavior> topRankedBehaviors = new List<UtilityBehavior>();
        foreach (UtilityBehavior behavior in ally.GetComponents<UtilityBehavior>())
        {
            if (behavior.GetRank() == highestRank)
                topRankedBehaviors.Add(behavior);
        }

        // remove behaviors whose weight is significantly less than the best
        float threshold = 0.2f * highestWeight;  
        topRankedBehaviors.RemoveAll(b => b.GetWeight() < threshold);

        // weighted random selection among the remaining behaviors
        float randomWeight = UnityEngine.Random.value * totalWeight;
        float accumulatedWeight = 0;
        foreach (UtilityBehavior behavior in topRankedBehaviors)
        {
            accumulatedWeight += behavior.GetWeight();
            if (accumulatedWeight >= randomWeight)
            {
                bestBehavior = behavior;
                break;
            }
        }

        return bestBehavior;
    }

}