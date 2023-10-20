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
                AssignProtectionAgainstTurret();
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
                
                break;

            default:
                break;
        }
    }


    private void AssignProtectionAgainstTurret()
    {
        float turretDetectionRadius = 10f;
        List<AIAgent> agents = new List<AIAgent>(_agentDesiredPositions.Keys);

        Collider[] turretsNearPlayer = Physics.OverlapSphere(_player.transform.position, turretDetectionRadius, 1 << LayerMask.NameToLayer("Enemy"));

        // Sort turrets by distance to the player
        List<Collider> sortedTurrets = new List<Collider>(turretsNearPlayer);
        sortedTurrets.Sort((a, b) => Vector3.Distance(a.transform.position, _player.transform.position).CompareTo(Vector3.Distance(b.transform.position, _player.transform.position)));

        foreach (var turret in sortedTurrets)
        {
            Vector3 directionToTurret = turret.transform.position - _player.transform.position;
            RaycastHit hit;

            if (Physics.Raycast(_player.transform.position, directionToTurret, out hit))
            {
                Debug.DrawLine(_player.transform.position, hit.point, Color.red, 2f);

                if (hit.collider.gameObject.tag == "Enemy")
                {
                    Vector3 protectionPosition = Vector3.Lerp(_player.transform.position, turret.transform.position, 0.5f);

                    AIAgent closestAgent = null;
                    float closestDistance = float.MaxValue;
                    foreach (var agent in agents)
                    {
                        float distance = Vector3.Distance(agent.transform.position, protectionPosition);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestAgent = agent;
                        }
                    }

                    if (closestAgent != null)
                    {
                        _agentDesiredPositions[closestAgent] = protectionPosition;
                        Debug.DrawLine(closestAgent.transform.position, protectionPosition, Color.blue, 2f);
                        agents.Remove(closestAgent);
                    }
                }
            }
        }
    }

    private void DefaultFormation()
    {
        List<AIAgent> agents = new List<AIAgent>(_agentDesiredPositions.Keys);

        foreach (var agent in agents)
        {
            _agentDesiredPositions[agent] = agent.transform.position;
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

        for (int i = 0; i < agentsPerSide && agentIndex < agents.Count; i++)
        {
            _agentDesiredPositions[agents[agentIndex]] = startFront + _player.transform.right * (i * agentSpacing);
            agentIndex++;
        }
        for (int i = 0; i < agentsPerSide && agentIndex < agents.Count; i++)
        {
            _agentDesiredPositions[agents[agentIndex]] = startRight - _player.transform.forward * (i * agentSpacing);
            agentIndex++;
        }
        for (int i = 0; i < agentsPerSide && agentIndex < agents.Count; i++)
        {
            _agentDesiredPositions[agents[agentIndex]] = startBack - _player.transform.right * (i * agentSpacing);
            agentIndex++;
        }
        for (int i = 0; i < agentsPerSide && agentIndex < agents.Count; i++)
        {
            _agentDesiredPositions[agents[agentIndex]] = startLeft + _player.transform.forward * (i * agentSpacing);
            agentIndex++;
        }


        float circleRadius = desiredDistanceFromPlayer + 2f; 
        float angleStep = 360f / (agents.Count - agentIndex); 
        float currentAngle = 0;

        while (agentIndex < agents.Count)
        {
            Vector3 offset = new Vector3(Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0, Mathf.Cos(currentAngle * Mathf.Deg2Rad)) * circleRadius;
            _agentDesiredPositions[agents[agentIndex]] = _player.transform.position + offset;
            currentAngle += angleStep;
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
