using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace FSMMono
{
    public class AIAgent : MonoBehaviour, IDamageable
    {

        [SerializeField]
        int MaxHP = 100;
        [SerializeField]
        float BulletPower = 1000f;
        [SerializeField]
        GameObject BulletPrefab;

        [SerializeField]
        PlayerAgent player;

        List<TurretAgent> listEnemies = new List<TurretAgent>();

        [SerializeField]
        Slider HPSlider = null;

        Transform GunTransform;
        NavMeshAgent NavMeshAgentInst;
        Material MaterialInst;

        [SerializeField]
        float ShootFrequency = 1f;

        float NextShootDate = 0f;

        bool IsDead = false;
        int CurrentHP;
        public int distBetweenPlayerAllie;

        private void SetMaterial(Color col)
        {
            MaterialInst.color = col;
        }
        public void SetWhiteMaterial() { SetMaterial(Color.white); }
        public void SetRedMaterial() { SetMaterial(Color.red); }
        public void SetBlueMaterial() { SetMaterial(Color.blue); }
        public void SetYellowMaterial() { SetMaterial(Color.yellow); }

        #region MonoBehaviour

        private void Awake()
        {
            CurrentHP = MaxHP;

            NavMeshAgentInst = GetComponent<NavMeshAgent>();

            Renderer rend = transform.Find("Body").GetComponent<Renderer>();
            MaterialInst = rend.material;

            GunTransform = transform.Find("Body/Gun");
            if (GunTransform == null)
                Debug.Log("could not fin gun transform");

            if (HPSlider != null)
            {
                HPSlider.maxValue = MaxHP;
                HPSlider.value = CurrentHP;
            }
        }
        private void Start()
        {
            listEnemies.AddRange(FindObjectsOfType<TurretAgent>());
        }
        private void Update()
        {
            foreach(TurretAgent turretagent in listEnemies)
            {
                if(turretagent.IsShooting && Time.time >= NextShootDate)
                {
                    float dist = Vector3.Distance(player.gameObject.transform.position, turretagent.transform.position);

                    // Direction between player and turrets
                    Vector3 direction = (player.gameObject.transform.position - turretagent.transform.position).normalized;

                    // New Position of agents
                    Vector3 newPosition = player.gameObject.transform.position - direction * 2;

                    MoveTo(newPosition);
                    NextShootDate = Time.time + ShootFrequency;
                    ShootToPosition(turretagent.gameObject.transform.position);
                }
                else if (!turretagent.IsShooting)
                {
                    FollowPlayer();
                }
            }

            if(player.CurrentHP < 10)
            {
                MoveTo(player.gameObject.transform.position);

                float dist = Vector3.Distance(player.gameObject.transform.position, transform.position);

                Debug.Log(dist);

                if(dist < 1.4)
                {
                    player.CurrentHP += 1;
                }

                Debug.Log(player.CurrentHP);

            }

        }
        private void OnTriggerEnter(Collider other)
        {
        }
        private void OnTriggerExit(Collider other)
        {
        }
        private void OnDrawGizmos()
        {
        }

        #endregion

        #region Perception methods

        #endregion

        #region MoveMethods

        public void FollowPlayer()
        {
            MoveTo(player.gameObject.transform.position);

            float dist = Vector3.Distance(transform.position, player.gameObject.transform.position);

            if (dist < distBetweenPlayerAllie)
                StopMove();
        }
        public void StopMove()
        {
            NavMeshAgentInst.isStopped = true;
        }
        public void MoveTo(Vector3 dest)
        {
            NavMeshAgentInst.isStopped = false;
            NavMeshAgentInst.SetDestination(dest);
        }
        public bool HasReachedPos()
        {
            return NavMeshAgentInst.remainingDistance - NavMeshAgentInst.stoppingDistance <= 0f;
        }

        #endregion

        #region ActionMethods

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
        public void ShootToPosition(Vector3 pos)
        {
            // look at target position
            transform.LookAt(pos + Vector3.up * transform.position.y);

            // instantiate bullet
            if (BulletPrefab)
            {
                GameObject bullet = Instantiate<GameObject>(BulletPrefab, GunTransform.position + transform.forward * 0.5f, Quaternion.identity);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * BulletPower);
            }
        }
        #endregion
    }
}