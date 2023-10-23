using UnityEngine;

public class ProtectPlayerBehavior : UtilityBehavior
{
    private GameObject             _player;
    private CanShootConsideration  _canShoot;
    private EnemyNearConsideration _enemyNear;
    private ShouldNotFollowFormationConsideration _formationConsideration;
    private AIAgent _aiAgent;

    public float positionThreshold = 0.5f;

    public override void Start()
    {
        _player  = GameObject.FindGameObjectWithTag("Player");
        _aiAgent = gameObject.GetComponent<AIAgent>();

        //FIRST APPROACH, only focus 1 enemy at a time
        /* 
            _closestEnemy = gameObject.AddComponent<ClosestVisibleEnemyFromPlayer>();
            Considerations.Add(_closestEnemy);
        */

        _enemyNear =  gameObject.AddComponent<EnemyNearConsideration>();
        Considerations.Add(_enemyNear);

        _formationConsideration = gameObject.AddComponent<ShouldNotFollowFormationConsideration>();
        Considerations.Add(_formationConsideration);

    }

    public override void UpdateBehavior()
    {
        //FIRST APPROACH, only focus 1 enemy at a time
        /*
            Vector3 midpoint = (_player.transform.position + _closestEnemy.closestEnemyPosition) / 2;
            if (Vector3.Distance(transform.position, midpoint) > positionThreshold)
            {
                    _aiAgent.MoveTo(midpoint);
            }
        */

        //SECOND APPROACH, uses formation manager
        _aiAgent.MoveTo(FormationManager.Instance.GetDesiredPositionForAgent(_aiAgent));
    }
}

