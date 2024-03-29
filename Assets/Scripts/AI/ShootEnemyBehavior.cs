using UnityEngine;

public class ShootEnemyBehavior : UtilityBehavior
{
    private GameObject _player;
    private AIAgent    _aiAgent;
    private CanShootConsideration _canShoot;
    private ClosestVisibleEnemyConsideration _closestEnemy;


    public override void Start()
    {
        _player  = GameObject.FindGameObjectWithTag("Player");
        _aiAgent = gameObject.GetComponent<AIAgent>();

        _closestEnemy = gameObject.AddComponent<ClosestVisibleEnemyConsideration>();
        Considerations.Add(_closestEnemy);

        _canShoot = gameObject.AddComponent<CanShootConsideration>();
        Considerations.Add(_canShoot);
    }

    public override void UpdateBehavior()
    {
        Vector3 targetPosition = new Vector3(_aiAgent.targetPos.x, 0, _aiAgent.targetPos.z);
        _aiAgent.ShootToPosition(targetPosition);
    }
}

