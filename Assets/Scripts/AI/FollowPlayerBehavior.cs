using UnityEngine;

public class FollowPlayerBehavior : UtilityBehavior
{
    private GameObject player;
    private DistanceToPlayer distanceConsideration;

    [SerializeField]
    private float desiredDistance = 5.0f;

    private Vector3 offset;

    public override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        // init considerations
        distanceConsideration = gameObject.AddComponent<DistanceToPlayer>();
        distanceConsideration.desiredDistance = desiredDistance;
        // add to the list
        Considerations.Add(distanceConsideration);
        // will be replaced by umut's logic
        offset = (gameObject.transform.position - player.transform.position).normalized * desiredDistance;
    }

    public override void UpdateBehavior()
    {
        // will be replaced by umut's logic
        Vector3 desiredPosition = player.transform.position + offset;
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, desiredPosition, 0.05f); 
    }

    public override void Reset()
    {
       
    }
}

