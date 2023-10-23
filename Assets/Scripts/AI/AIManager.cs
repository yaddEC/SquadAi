using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] private int              _aiReactionDelay; 
    [SerializeField] private float            _reloadingTime = 2f; 
    [SerializeField] private List<GameObject> _allies; 

    private int        _currentFrame;
    private GameObject _player;

    public Formation currentFormation { get; set; } 
    public static AIManager Instance  { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        currentFormation = Formation.NONE;
    }

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        foreach (GameObject ally in GameObject.FindGameObjectsWithTag("AlliesAI"))
            _allies.Add(ally);
    }

    void Update()
    {
        _currentFrame++;

        if (_currentFrame < _aiReactionDelay)
            return;

        _currentFrame = 0;

        foreach (GameObject ally in _allies)
        {
            UtilityBehavior bestBehavior = GetBestBehavior(ally);
            AIAgent aIAgent = ally.GetComponent<AIAgent>();

            if (aIAgent.currentBehavior != bestBehavior || aIAgent.currentBehavior == null)
                aIAgent.SetBehavior(bestBehavior);
        }
    }

    UtilityBehavior GetBestBehavior(GameObject ally) // Inspired by the dual utility reasoner
    {
        UtilityBehavior bestBehavior = null;
        float highestRank            = float.NegativeInfinity;
        float highestWeight          = float.NegativeInfinity;
        float totalWeight            = 0;

        // Get rank and weight to determine priority
        foreach (UtilityBehavior behavior in ally.GetComponents<UtilityBehavior>())
        {
            float currentRank   = behavior.GetRank();
            float currentWeight = behavior.GetWeight();

            if (currentWeight > 0) // Remove all weights that are equal to 0
            {
                totalWeight += currentWeight;

                if (currentRank > highestRank)
                {
                    // Keep in memory the highest rank
                    highestRank = currentRank;
                    highestWeight = currentWeight;
                }
            }
        }

        // Determine the highest rank category and eliminate options with lower rank
        List<UtilityBehavior> topRankedBehaviors = new List<UtilityBehavior>();
        foreach (UtilityBehavior behavior in ally.GetComponents<UtilityBehavior>())
        {
            if (behavior.GetRank() == highestRank)
                topRankedBehaviors.Add(behavior);
        }

        // Remove behaviors whose weight is significantly less than the best
        float threshold = 0.2f * highestWeight;
        topRankedBehaviors.RemoveAll(b => b.GetWeight() < threshold);

        // Weighted random selection among the remaining behaviors
        float randomWeight      = UnityEngine.Random.value * totalWeight;
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
