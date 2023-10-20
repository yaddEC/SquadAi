using FSMMono;
using UnityEngine;

public class HealPlayerBehavior : UtilityBehavior
{
    private GameObject _player;
    private PlayerHealthConsideration _playerHealth;
    private ShouldNotFollowFormationConsideration _formationConsideration;
    private AIAgent _aiAgent;
    private PlayerAgent _playerAgent;


    public override void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerAgent = _player.GetComponent<PlayerAgent>(); 
        _aiAgent = gameObject.GetComponent<AIAgent>();

        _formationConsideration = gameObject.AddComponent<ShouldNotFollowFormationConsideration>();

        _playerHealth = gameObject.AddComponent<PlayerHealthConsideration>();
        Considerations.Add(_playerHealth);
        Considerations.Add(_formationConsideration);

    }

    public override void UpdateBehavior()
    {
        float dist = Vector3.Distance(_player.transform.position, transform.position);

        Debug.Log(dist);

        if (dist < 1.6 && _playerAgent.CurrentHP< _playerAgent.MaxHP)
        {
            _playerAgent.CurrentHP += 0.1f;
        }
        else 
        { 
        
            _aiAgent.MoveTo(_player.transform.position);
        }
        
        

        Debug.Log(_playerAgent.CurrentHP);

    }


}

