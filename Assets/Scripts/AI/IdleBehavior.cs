using FSMMono;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IdleBehavior : UtilityBehavior
{
    private IdleDelayConsideration _idleDelayConsideration;

    [SerializeField]
    private float spinSpeed = 30f; //  (low value = speen/ high value = SPEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEN)

    public override void Start()
    {
        // init considerations
        _idleDelayConsideration = gameObject.AddComponent<IdleDelayConsideration>();

        // add to the list
        Considerations.Add(_idleDelayConsideration);
    }

    public override void UpdateBehavior()
    {
        // make the NPC speeeeeeenn when idling(will be replaced by advanced random movment)
        gameObject.transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }

    public override void Reset()
    {

    }
}