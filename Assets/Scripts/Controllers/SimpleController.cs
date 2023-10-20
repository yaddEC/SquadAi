using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using FSMMono;
using System.Collections.Generic;

public class SimpleController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 6f;



    PlayerAgent Player;
    public List<AIAgent> listAgent = new List<AIAgent>();

	Camera viewCamera;
	Vector3 velocity;

    private Action<Vector3> OnMouseLeftClicked;
    private Action<Vector3> OnMouseRightClicked;

    void Start ()
    {
        Player = GetComponent<PlayerAgent>();
        listAgent.AddRange(FindObjectsOfType<AIAgent>());
        viewCamera = Camera.main;

        OnMouseLeftClicked += Player.ShootToPosition;

        OnMouseRightClicked += Player.NPCShootToPosition;
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

            Player.AimAtPosition(targetPos);
        }

        velocity = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical")).normalized * moveSpeed;

        if (Input.GetMouseButtonDown(0))
        {
            OnMouseLeftClicked(targetPos);

            if(!Player.GetComponent<PlayerAgent>().isAiCoverShooting() ) 
            {
                foreach (var agent in listAgent)
                    agent.ShootToPosition(targetPos);
            }

        }
        if (Input.GetMouseButtonDown(1))
        {
            OnMouseRightClicked(targetPos);
            foreach (var agent in listAgent)
                agent.CoverShot(targetPos);
        }
    }
	void FixedUpdate()
    {
        Player.MoveToward(velocity);
	}
}