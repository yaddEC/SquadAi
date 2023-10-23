using UnityEngine;


public class IdleBehavior : UtilityBehavior
{
    private IdleDelayConsideration                _idleDelayConsideration;
    private ShouldNotFollowFormationConsideration _formationConsideration;

    [SerializeField]
    private float spinSpeed = 30f; //  (Low value = speen/ High value = SPEEN!)

    public override void Start()
    {
        // Init considerations
        _idleDelayConsideration = gameObject.AddComponent<IdleDelayConsideration>();
        _formationConsideration = gameObject.AddComponent<ShouldNotFollowFormationConsideration>();

        // Add to the list
        Considerations.Add(_idleDelayConsideration);
        Considerations.Add(_formationConsideration);
    }

    public override void UpdateBehavior()
    {
        // Make the NPC speeeeeeenn when idling(will be replaced by advanced random movment)
        gameObject.transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }

    public override void Reset()
    {

    }
}