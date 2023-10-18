using FSMMono;
using UnityEngine;

public class FollowPlayerBehavior : UtilityBehavior
{
    private GameObject _player;
    private DistanceToPlayer _distanceConsideration;
    private AIAgent _aiAgent;


    public override void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _aiAgent = gameObject.GetComponent<AIAgent>();
        _distanceConsideration = gameObject.AddComponent<DistanceToPlayer>();
        _distanceConsideration.desiredDistance = _aiAgent.distBetweenPlayerAllie;
        Considerations.Add(_distanceConsideration);

    }

    public override void UpdateBehavior()
    {
        _aiAgent.FollowPlayer();
    }

    public override void Reset()
    {
       
    }
}

