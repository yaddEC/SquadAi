using FSMMono;
using UnityEngine;

public class FollowFormationBehavior : UtilityBehavior
{
    private GameObject _player;
    private ShouldFollowFormation _formationConsideration;
    private AIAgent _aiAgent;


    public override void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _aiAgent = gameObject.GetComponent<AIAgent>();
        _formationConsideration = gameObject.AddComponent<ShouldFollowFormation>();
        Considerations.Add(_formationConsideration);

    }

    public override void UpdateBehavior()
    {
        Vector3 desiredPosition = FormationManager.Instance.GetDesiredPositionForAgent(_aiAgent);
        _aiAgent.MoveTo(desiredPosition);
    }

    public override void Reset()
    {
       
    }
}

