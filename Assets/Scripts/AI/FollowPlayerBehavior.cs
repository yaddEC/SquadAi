using FSMMono;
using UnityEngine;

public class FollowPlayerBehavior : UtilityBehavior
{
    private GameObject _player;
    private DistanceToPlayerConsideration _distanceConsideration;
    private ShouldNotFollowFormationConsideration _formationConsideration;
    private AIAgent _aiAgent;


    public override void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _aiAgent = gameObject.GetComponent<AIAgent>();
        _distanceConsideration = gameObject.AddComponent<DistanceToPlayerConsideration>();
        _distanceConsideration.desiredDistance = _aiAgent.distBetweenPlayerAllie;
        Considerations.Add(_distanceConsideration);
        _formationConsideration = gameObject.AddComponent<ShouldNotFollowFormationConsideration>();
        Considerations.Add(_formationConsideration);

    }

    public override void UpdateBehavior()
    {
        Debug.Log("Follow");
        _aiAgent.FollowPlayer();
    }

    public override void Reset()
    {
       
    }
}

