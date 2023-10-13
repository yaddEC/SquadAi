using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IdleBehavior : UtilityBehavior
{
    private ProximityToPlayer proximityConsideration;

    [SerializeField]
    private float spinSpeed = 30f; //  (low value = speen/ high value = SPEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEN)

    public override void Start()
    {
        // init considerations
        proximityConsideration = gameObject.AddComponent<ProximityToPlayer>();
        // add to the list
        Considerations.Add(proximityConsideration);
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