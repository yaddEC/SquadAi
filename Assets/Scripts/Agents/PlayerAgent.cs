using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAgent : MonoBehaviour, IDamageable
{
    [SerializeField]
    public int MaxHP = 100;
    [SerializeField]
    float BulletPower = 1000f;
    [SerializeField]
    GameObject BulletPrefab;

    [SerializeField]
    GameObject TargetCursorPrefab = null;
    [SerializeField]
    GameObject NPCTargetCursorPrefab = null;

    [SerializeField]
    Slider HPSlider = null;

    Rigidbody rb;
    GameObject TargetCursor = null;
    GameObject NPCTargetCursor = null;
    Transform GunTransform;
    bool IsDead = false;
    public float CurrentHP;

    private GameObject GetTargetCursor()
    {
        if (TargetCursor == null)
            TargetCursor = Instantiate(TargetCursorPrefab);
        return TargetCursor;
    }
    public GameObject GetNPCTargetCursor()
    {
        if (NPCTargetCursor == null)
        {
            NPCTargetCursor = Instantiate(NPCTargetCursorPrefab);
        }
        else 
        {
            Destroy(NPCTargetCursor);
        }
        return NPCTargetCursor;
    }

    public bool isAiCoverShooting() => NPCTargetCursor != null;

    public void AimAtPosition(Vector3 pos)
    {
        GetTargetCursor().transform.position = pos;
        if (Vector3.Distance(transform.position, pos) > 2.5f)
            transform.LookAt(pos + Vector3.up * transform.position.y);
    }
    public void ShootToPosition(Vector3 pos)
    {
        RaycastHit hit;
        Vector3 shootDirection = transform.forward;
        if (Physics.Raycast(GunTransform.position, shootDirection, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Allies"))
            {
                return;
            }
        }
        // instantiate bullet
        if (BulletPrefab)
        {
            GameObject bullet = Instantiate<GameObject>(BulletPrefab, GunTransform.position + transform.forward * 0.5f, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * BulletPower);


            Collider[] allyColliders = Physics.OverlapSphere(bullet.transform.position, 50.0f, 1 << LayerMask.NameToLayer("Allies")); // Assuming a radius of 50 units for overlap check, adjust as needed.
            Collider bulletCollider = bullet.GetComponent<Collider>();
            foreach (var allyCollider in allyColliders)
            {
                Physics.IgnoreCollision(bulletCollider, allyCollider);
            }
        }
    }
    public void NPCShootToPosition(Vector3 pos)
    {

        GetNPCTargetCursor().transform.position = pos;
    }
    public void AddDamage(int amount)
    {
        CurrentHP -= amount;
        if (CurrentHP <= 0)
        {
            IsDead = true;
            CurrentHP = 0;
        }
        if (HPSlider != null)
        {
            HPSlider.value = CurrentHP;
        }
    }
    public void MoveToward(Vector3 velocity)
    {
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
    }

    #region MonoBehaviour Methods
    void Start()
    {
        CurrentHP = MaxHP;
        GunTransform = transform.Find("Gun");
        rb = GetComponent<Rigidbody>();

        if (HPSlider != null)
        {
            HPSlider.maxValue = MaxHP;
            HPSlider.value = CurrentHP;
        }
    }
    void Update()
    {
        
    }

    #endregion

}
