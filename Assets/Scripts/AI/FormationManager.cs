using FSMMono;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FormationManager : MonoBehaviour
{
    public static FormationManager Instance { get; private set; }

    private Dictionary<AIAgent, Vector3> _agentDesiredPositions = new Dictionary<AIAgent, Vector3>();
    
    private GameObject _player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        // find all ai allies
        AIAgent[] agents = FindObjectsOfType<AIAgent>();
        foreach (var agent in agents)
        {
            // init their pos
            _agentDesiredPositions[agent] = agent.transform.position;
        }
    }

    void Update()
    {
        switch (AIManager.Instance.currentFormation)
        {
            case Formation.NONE:
                // no specific formation, followplayerbehavior
                break;

            case Formation.TURTLE:
                OrganizeTurtleFormation();
                break;

            case Formation.SHIELD:
                OrganizeShieldFormation();
                break;

            case Formation.SURROUND:
                OrganizeSurroundFormation();
                break;

            case Formation.MOUSE:
                OrganizeMouseFormation();
                break;

            case Formation.FREEZE:
                // nothing
                break;

            default:
                break;
        }
    }

private void OrganizeTurtleFormation()
{
    float agentSpacing = 1.5f;
    float perimeter = agentSpacing * _agentDesiredPositions.Count;
    float radius = perimeter / (2 * Mathf.PI);

    float angleStep = 360f / _agentDesiredPositions.Count;
    float currentAngle = 0;

        List<AIAgent> agents = new List<AIAgent>(_agentDesiredPositions.Keys);

        foreach (var agent in agents)
        {
            Vector3 offset = new Vector3(Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0, Mathf.Cos(currentAngle * Mathf.Deg2Rad)) * radius;
            _agentDesiredPositions[agent] = _player.transform.position + offset;
            currentAngle += angleStep;
        }
    }

    private void OrganizeShieldFormation()
    {
        
        float desiredDistanceFromPlayer = 5.0f;
        float agentSpacing = 1.5f;
        Vector3 shieldCenter = _player.transform.position + _player.transform.forward * desiredDistanceFromPlayer;
        float totalWidth = agentSpacing * (_agentDesiredPositions.Count - 1);
        Vector3 startLeft = shieldCenter - _player.transform.right * (totalWidth / 2);
        List<AIAgent> agents = new List<AIAgent>(_agentDesiredPositions.Keys);

        for (int i = 0; i < agents.Count; i++)
        {
            _agentDesiredPositions[agents[i]] = startLeft + _player.transform.right * (i * agentSpacing);
        }
    }


    private void OrganizeSurroundFormation()
    {
        float desiredDistanceFromPlayer = 3.0f;
        float agentSpacing = 1.5f;


        int agentsPerSide = _agentDesiredPositions.Count / 4;
        float totalWidth = agentSpacing * (agentsPerSide - 1);

        Vector3 forwardOffset = _player.transform.forward * desiredDistanceFromPlayer;
        Vector3 rightOffset = _player.transform.right * desiredDistanceFromPlayer;

        Vector3 startFront = _player.transform.position + forwardOffset - _player.transform.right * (totalWidth / 2);
        Vector3 startRight = _player.transform.position + rightOffset + _player.transform.forward * (totalWidth / 2);
        Vector3 startBack = _player.transform.position - forwardOffset + _player.transform.right * (totalWidth / 2);
        Vector3 startLeft = _player.transform.position - rightOffset - _player.transform.forward * (totalWidth / 2);

        List<AIAgent> agents = new List<AIAgent>(_agentDesiredPositions.Keys);

        int agentIndex = 0;

        for (int i = 0; i < agentsPerSide; i++)
        {
            _agentDesiredPositions[agents[agentIndex]] = startFront + _player.transform.right * (i * agentSpacing);
            agentIndex++;
        }
        for (int i = 0; i < agentsPerSide; i++)
        {
            _agentDesiredPositions[agents[agentIndex]] = startRight - _player.transform.forward * (i * agentSpacing);
            agentIndex++;
        }
        for (int i = 0; i < agentsPerSide; i++)
        {
            _agentDesiredPositions[agents[agentIndex]] = startBack - _player.transform.right * (i * agentSpacing);
            agentIndex++;
        }
        for (int i = 0; i < agentsPerSide; i++)
        {
            _agentDesiredPositions[agents[agentIndex]] = startLeft + _player.transform.forward * (i * agentSpacing);
            agentIndex++;
        }
    }


    private void OrganizeMouseFormation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 mouseWorldPosition;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Floor")))
        {
            mouseWorldPosition = hit.point;
        }
        else
        {
            return; // if ray doesnt hit
        }

        float agentSpacing = 1.5f;
        float perimeter = agentSpacing * _agentDesiredPositions.Count;
        float radius = perimeter / (2 * Mathf.PI);

        float angleStep = 360f / _agentDesiredPositions.Count;
        float currentAngle = 0;

        List<AIAgent> agents = new List<AIAgent>(_agentDesiredPositions.Keys);

        foreach (var agent in agents)
        {
            Vector3 offset = new Vector3(Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0, Mathf.Cos(currentAngle * Mathf.Deg2Rad)) * radius;
            _agentDesiredPositions[agent] = mouseWorldPosition + offset;
            currentAngle += angleStep;
        }
    }


    public Vector3 GetDesiredPositionForAgent(AIAgent agent)
    {
        if (_agentDesiredPositions.TryGetValue(agent, out Vector3 desiredPosition))
        {
            return desiredPosition;
        }
        return Vector3.zero;//damn i need to think what should happen if an ai dies too lmao
    }
}
