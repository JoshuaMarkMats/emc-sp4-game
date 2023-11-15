using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class RangedAttackRadius : AttackRadius
{
    public NavMeshAgent agent;
    public Bullet bulletPrefab;
    public Vector3 bulletSpawnOffset = new(0, 1, 0);
    public LayerMask mask;
    private ObjectPool bulletPool;
    public float maxRange;
    [SerializeField]
    private float SphereCastRadius = 0.1f;
    private RaycastHit hit;
    private IDamageable targetDamageable;

    protected override void Awake()
    {
        base.Awake();

        //if all bullets miss and go into oblivion we'll not run out of bullets
        //bulletPool = ObjectPool.CreateInstance(bulletPrefab, Mathf.CeilToInt((1 / attackCooldown) * bulletPrefab.AutoDestroyTime));
    }

    private void Start()
    {
        maxRange = GetComponent<SphereCollider>().radius;
    }

    public void CreateBulletPool()
    {
        if (bulletPool == null)
        {
            bulletPool = ObjectPool.CreateInstance(bulletPrefab, Mathf.CeilToInt((1 / attackCooldown) * bulletPrefab.AutoDestroyTime));
        }
    }

    protected override IEnumerator Attack()
    {
        WaitForSeconds Wait = new(attackCooldown);
        yield return Wait;
        while (Damageables.Count > 0)
        {
            for (int i = 0; i < Damageables.Count; i++)
            {
                if (HasLineOfSight(Damageables[i].GetTransform()))
                {
                    targetDamageable = Damageables[i];
                    onAttack?.Invoke(Damageables[i]);
                    agent.isStopped = true;
                    break;
                }
            }

            if (targetDamageable != null)
            {
                PoolableObject poolableObject = bulletPool.GetObject();
                if (poolableObject != null)
                {
                    Bullet bullet = poolableObject.GetComponent<Bullet>();
                    bullet.Damage = damage;
                    bullet.transform.position = transform.position + bulletSpawnOffset;
                    bullet.transform.rotation = agent.transform.rotation;
                    bullet.Rigidbody.AddForce((targetDamageable.GetTransform().position - transform.position).normalized * bulletPrefab.MoveSpeed, ForceMode.VelocityChange);
                    //bullet.Rigidbody.AddForce((agent.transform.forward + (targetDamageable.GetTransform().position - transform.position).y) * bulletPrefab.MoveSpeed, ForceMode.VelocityChange);
                }
            }
            else
            {
                agent.isStopped = false;
            }
            yield return Wait;

            if (targetDamageable == null || !HasLineOfSight(targetDamageable.GetTransform()))
            {
                agent.isStopped = false;
                attackCoroutine = null;
                targetDamageable = null; //remove current target to stop the shooting
            }

            Damageables.RemoveAll(DisableDamageables);
        }

        agent.isStopped = false;
        attackCoroutine = null;
    }

    private bool HasLineOfSight(Transform target)
    {
        if (Physics.SphereCast(transform.position + bulletSpawnOffset, SphereCastRadius, ((target.position + bulletSpawnOffset) - (transform.position + bulletSpawnOffset)).normalized, out hit, maxRange, mask))
        {
            IDamageable damageable;
            if (hit.collider.TryGetComponent<IDamageable>(out damageable))
            {
                return damageable.GetTransform() == target;
            }
        }

        return false;
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (attackCoroutine == null)
        {
            agent.isStopped = true;
        }
    }
}
