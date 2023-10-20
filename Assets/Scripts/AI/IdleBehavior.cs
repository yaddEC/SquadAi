using FSMMono;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IdleBehavior : UtilityBehavior
{
    private IdleDelayConsideration _idleDelayConsideration;
    private ShouldNotFollowFormationConsideration _formationConsideration;

    [SerializeField]
    private float spinSpeed = 30f; //  (low value = speen/ high value = SPEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEN)

    public override void Start()
    {
        // init considerations
        _idleDelayConsideration = gameObject.AddComponent<IdleDelayConsideration>();
        _formationConsideration = gameObject.AddComponent<ShouldNotFollowFormationConsideration>();

        // add to the list
        Considerations.Add(_idleDelayConsideration);
        Considerations.Add(_formationConsideration);
    }

    public override void UpdateBehavior()
    {
        Debug.Log("Idle");
        // make the NPC speeeeeeenn when idling(will be replaced by advanced random movment)
        gameObject.transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }

    public override void Reset()
    {

    }
}