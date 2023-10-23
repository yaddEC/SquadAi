using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 _offset = new Vector3(0f, 13f, -7f);

	public Transform target    = null;
	public float     smoothing = 5f;

	void Start ()
	{
        if (target == null)
            return;

        transform.position = target.position + _offset;
        transform.LookAt(target);
    }

	void FixedUpdate ()
	{
        if (target == null)
            return;

		Vector3 targetCamPos = target.position + _offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}
