using UnityEngine;

public class FollowPlayerBehavior : UtilityBehavior
{
    private AIAgent    _aiAgent;
    private GameObject _player;
    private DistanceToPlayerConsideration _distanceConsideration;
    private ShouldNotFollowFormationConsideration _formationConsideration;

    public override void Start()
    {
        _player  = GameObject.FindGameObjectWithTag("Player");
        _aiAgent = gameObject.GetComponent<AIAgent>();
        _distanceConsideration = gameObject.AddComponent<DistanceToPlayerConsideration>();

        _distanceConsideration.desiredDistance = _aiAgent.distBetweenPlayerAllie;
        Considerations.Add(_distanceConsideration);

        _formationConsideration = gameObject.AddComponent<ShouldNotFollowFormationConsideration>();
        Considerations.Add(_formationConsideration);

    }

    public override void UpdateBehavior()
    {
        _aiAgent.FollowPlayer();
    }

    public override void Reset()
    {
       
    }
}

