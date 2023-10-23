using UnityEngine;

public class TurretAgent : MonoBehaviour, IDamageable
{
    [SerializeField] private int        _maxHP = 100;
    [SerializeField] private float      _bulletPower = 1000f;
    [SerializeField] private float      _shootFrequency = 1f;
    [SerializeField] private GameObject _bulletPrefab;

    private float     _nextShootDate = 0f;
    private Transform _gunTransform;

    private int        _currentHP;
    private GameObject _target = null;

    public bool isShooting = false;

    public void AddDamage(int amount)
    {
        _currentHP -= amount;
        if (_currentHP <= 0)
        {
            _currentHP = 0;

            gameObject.SetActive(false);
        }
    }

    private void ShootToPosition(Vector3 pos)
    {
        transform.LookAt(pos + Vector3.up * transform.position.y);

        if (_bulletPrefab)
        {
            GameObject bullet = Instantiate<GameObject>(_bulletPrefab, _gunTransform.position + transform.forward * 0.5f, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * _bulletPower);
        }
    }

    void Start()
    {
        _gunTransform = transform.Find("Body/Gun");

        if (_gunTransform == null)
            Debug.Log("could not find gun transform");

        _currentHP = _maxHP;
    }

    void Update()
    {
        if (_target && Time.time >= _nextShootDate)
        {
            _nextShootDate = Time.time + _shootFrequency;
            ShootToPosition(_target.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_target == null && other.gameObject.layer == LayerMask.NameToLayer("Allies"))
        {
            _target = other.gameObject;
            isShooting = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_target != null && other.gameObject == _target)
        {
            _target = null;
            isShooting = false;
        }
    }
}
