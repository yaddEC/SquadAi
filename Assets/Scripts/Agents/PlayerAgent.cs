using UnityEngine;
using UnityEngine.UI;

public class PlayerAgent : MonoBehaviour, IDamageable
{
    [SerializeField] private float      _bulletPower = 1000f;
    [SerializeField] private GameObject _bulletPrefab;

    [SerializeField] private GameObject _targetCursorPrefab = null;
    [SerializeField] private GameObject _npcTargetCursorPrefab = null;
    [SerializeField] private Slider     _hpSlider = null;

    private Rigidbody  _rb;
    private GameObject _targetCursor = null;
    private GameObject _npcTargetCursor = null;
    private Transform  _gunTransform;

    public int   maxHP = 100;
    public float currentHP;

    private GameObject GetTargetCursor()
    {
        if (_targetCursor == null)
            _targetCursor = Instantiate(_targetCursorPrefab);
        return _targetCursor;
    }

    public GameObject GetNPCTargetCursor()
    {
        if (_npcTargetCursor == null)
        {
            _npcTargetCursor = Instantiate(_npcTargetCursorPrefab);
        }
        else
        {
            Destroy(_npcTargetCursor);
        }
        return _npcTargetCursor;
    }

    public bool isAiCoverShooting() => _npcTargetCursor != null;

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
        if (Physics.Raycast(_gunTransform.position, shootDirection, out hit, Mathf.Infinity))
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Allies"))
                return;

        if (_bulletPrefab)
        {
            GameObject bullet = Instantiate<GameObject>(_bulletPrefab, _gunTransform.position + transform.forward * 0.5f, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * _bulletPower);

            Collider[] allyColliders = Physics.OverlapSphere(bullet.transform.position, 50.0f, 1 << LayerMask.NameToLayer("Allies"));
            Collider bulletCollider = bullet.GetComponent<Collider>();
            foreach (var allyCollider in allyColliders)
                Physics.IgnoreCollision(bulletCollider, allyCollider);
        }
    }

    public void NPCShootToPosition(Vector3 pos)
    {
        GetNPCTargetCursor().transform.position = pos;
    }

    public void AddDamage(int amount)
    {
        currentHP -= amount;

        if (currentHP <= 0)
            currentHP = 0;

        if (_hpSlider != null)
            _hpSlider.value = currentHP;
    }

    public void MoveToward(Vector3 velocity)
    {
        _rb.MovePosition(_rb.position + velocity * Time.deltaTime);
    }

    #region MonoBehaviour Methods

    void Start()
    {
        currentHP = maxHP;
        _gunTransform = transform.Find("Gun");
        _rb = GetComponent<Rigidbody>();

        if (_hpSlider != null)
        {
            _hpSlider.maxValue = maxHP;
            _hpSlider.value = currentHP;
        }
    }

    private void Update()
    {
        _hpSlider.value = currentHP;
    }

    #endregion
}
