using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIAgent : MonoBehaviour, IDamageable
{

    [SerializeField] private int         _maxHP = 100;
    [SerializeField] private float       _bulletPower = 1000f;
    [SerializeField] private GameObject  _bulletPrefab;
    [SerializeField] private PlayerAgent _player;
    [SerializeField] private Slider      _hpSlider = null;

    private List<TurretAgent> _listEnemies = new List<TurretAgent>();
    private Transform         _gunTransform;
    private NavMeshAgent      _navMeshAgentInst;
    private Material          _materialInst;

    public UtilityBehavior currentBehavior { get; private set; }
    public Vector3         targetPos;

    public bool isCovering { get; set; } = false;
    public bool isGunLoaded { get; set; } = true;
    public int  distBetweenPlayerAllie;

    private int  _currentHP;

    #region Material Methods

    private void SetMaterial(Color col) { _materialInst.color = col; }
    public void SetWhiteMaterial() { SetMaterial(Color.white); }
    public void SetRedMaterial() { SetMaterial(Color.red); }
    public void SetBlueMaterial() { SetMaterial(Color.blue); }
    public void SetYellowMaterial() { SetMaterial(Color.yellow); }

    #endregion

    public void SetBehavior(UtilityBehavior newBehavior)
    {
        if (currentBehavior != null && currentBehavior != newBehavior)
            currentBehavior.Reset();

        currentBehavior = newBehavior;
    }

    #region MonoBehaviour

    private void Awake()
    {
        _currentHP = _maxHP;

        _navMeshAgentInst = GetComponent<NavMeshAgent>();

        Renderer rend = transform.Find("Body").GetComponent<Renderer>();
        _materialInst = rend.material;

        _gunTransform = transform.Find("Body/Gun");

        if (_gunTransform == null)
            Debug.Log("could not find gun transform");

        if (_hpSlider != null)
        {
            _hpSlider.maxValue = _maxHP;
            _hpSlider.value = _currentHP;
        }
    }
    private void Start()
    {
        _listEnemies.AddRange(FindObjectsOfType<TurretAgent>());
    }

    private void Update()
    {
        if (currentBehavior == null) return;

        if (isCovering) ShootToPosition(targetPos);

        currentBehavior.UpdateBehavior();
    }

    #endregion

    #region MoveMethods

    public void FollowPlayer()
    {
        MoveTo(_player.gameObject.transform.position + ((gameObject.transform.position - _player.transform.position).normalized * distBetweenPlayerAllie));
    }
    public void StopMove()
    {
        _navMeshAgentInst.isStopped = true;
    }
    public void MoveTo(Vector3 dest)
    {
        _navMeshAgentInst.isStopped = false;
        _navMeshAgentInst.SetDestination(dest);
    }
    public bool HasReachedPos()
    {
        return _navMeshAgentInst.remainingDistance - _navMeshAgentInst.stoppingDistance <= 0f;
    }

    #endregion

    #region ActionMethods

    public void AddDamage(int amount)
    {
        _currentHP -= amount;
        if (_currentHP <= 0)
        {
            _currentHP = 0;
        }

        if (_hpSlider != null)
            _hpSlider.value = _currentHP;
    }

    private IEnumerator ReloadGun()
    {
        isGunLoaded = false;
        yield return new WaitForSeconds(2.0f);
        isGunLoaded = true;
    }

    public void CoverShot(Vector3 pos)
    {
        targetPos  = pos;
        isCovering = !isCovering;
    }

    public void ShootToPosition(Vector3 pos)
    {
        if (!isGunLoaded) return;

        transform.LookAt(pos + Vector3.up * transform.position.y);

        RaycastHit hit;
        Vector3 shootDirection = transform.forward;
        if (Physics.Raycast(_gunTransform.position, shootDirection, out hit, Mathf.Infinity))
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Allies"))
                return;

        if (_bulletPrefab)
        {
            // Instantiates a bullet and fires it to the specified position
            GameObject bullet = Instantiate<GameObject>(_bulletPrefab, _gunTransform.position + shootDirection * 0.5f, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(shootDirection * _bulletPower);

            Collider[] allyColliders = Physics.OverlapSphere(bullet.transform.position, 50.0f, 1 << LayerMask.NameToLayer("Allies")); // Assuming a radius of 50 units for overlap check, adjust as needed.
            Collider bulletCollider = bullet.GetComponent<Collider>();
            foreach (var allyCollider in allyColliders)
                Physics.IgnoreCollision(bulletCollider, allyCollider);
        }

        StartCoroutine(ReloadGun());
    }

    #endregion
}

