using FSMMono;
using UnityEngine;

public class FollowFormationBehavior : UtilityBehavior
{
    private GameObject _player;
    private ShouldFollowFormationConsideration _formationConsideration;
    private AIAgent _aiAgent;


    public override void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _aiAgent = gameObject.GetComponent<AIAgent>();
        _formationConsideration = gameObject.AddComponent<ShouldFollowFormationConsideration>();
        Considerations.Add(_formationConsideration);


    }

    public override void UpdateBehavior()
    {
        Debug.Log("Formation");
        Vector3 desiredPosition = FormationManager.Instance.GetDesiredPositionForAgent(_aiAgent);
        _aiAgent.MoveTo(desiredPosition);
    }


}

