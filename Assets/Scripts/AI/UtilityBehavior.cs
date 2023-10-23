using System.Collections.Generic;
using UnityEngine;

public abstract class UtilityBehavior : MonoBehaviour
{
    public List<IConsideration> Considerations { get; set; } = new List<IConsideration>();

    public  float GetRank()
    {
        float maxRank = float.MinValue;

        foreach (var consideration in Considerations)
            maxRank = Mathf.Max(maxRank, consideration.EvaluateRank());

        return maxRank;
    }

    public float GetWeight()
    {
        float totalBonus = 0;
        float productMultiplier = 1;

        foreach (var consideration in Considerations)
        {
            totalBonus += consideration.EvaluateBonus();
            productMultiplier *= consideration.EvaluateMultiplier();
        }

        return totalBonus * productMultiplier;
    }

    public virtual void Start()
    {
        
    }

    public virtual void Reset()
    {
        
    }

    public virtual void UpdateBehavior()
    {
        
    }

    public virtual void Trigger()
    {
        
    }
}
