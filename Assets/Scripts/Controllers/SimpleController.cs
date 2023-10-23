using System;
using UnityEngine;
using System.Collections.Generic;

public class SimpleController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 6f;

    private PlayerAgent  _player;
    private Camera       _viewCamera;
    private Vector3      _velocity;

    private Action<Vector3> _onMouseLeftClicked;
    private Action<Vector3> _onMouseRightClicked;

    public List<AIAgent> listAgent = new List<AIAgent>();

    void Start ()
    {
        _player = GetComponent<PlayerAgent>();
        listAgent.AddRange(FindObjectsOfType<AIAgent>());
        _viewCamera = Camera.main;

        _onMouseLeftClicked += _player.ShootToPosition;

        _onMouseRightClicked += _player.NPCShootToPosition;
    }
    void Update ()
    {
        int floorLayer = 1 << LayerMask.NameToLayer("Floor");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastInfo;
        Vector3 targetPos = Vector3.zero;
        if (Physics.Raycast(ray, out raycastInfo, Mathf.Infinity, floorLayer))
        {
            Vector3 newPos = raycastInfo.point;
            targetPos = newPos;
            targetPos.y += 0.1f;

            _player.AimAtPosition(targetPos);
        }

        _velocity = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical")).normalized * _moveSpeed;

        if (Input.GetMouseButtonDown(0))
        {
            _onMouseLeftClicked(targetPos);

            if(!_player.GetComponent<PlayerAgent>().isAiCoverShooting() ) 
            {
                foreach (var agent in listAgent)
                    agent.ShootToPosition(targetPos);
            }

        }
        if (Input.GetMouseButtonDown(1))
        {
            _onMouseRightClicked(targetPos);
            foreach (var agent in listAgent)
                agent.CoverShot(targetPos);
        }
    }
	void FixedUpdate()
    {
        _player.MoveToward(_velocity);
	}
}