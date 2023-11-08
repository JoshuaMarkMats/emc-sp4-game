using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(SphereCollider))]
public class AttackRadius : MonoBehaviour
{
    protected List<IDamageable> Damageables = new List<IDamageable>();
    public int damage = 10;
    public float attackCooldown = 0.5f;
    public delegate void AttackEvent(IDamageable Target);
    public AttackEvent onAttack;
    protected Coroutine attackCoroutine;

    protected virtual void Awake() { }

    //damageables that enter range added to list; start attacking
    protected virtual void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (damageable.IsAlive)
            {
                Damageables.Add(damageable);
                attackCoroutine ??= StartCoroutine(Attack());
            }           
        }
    }

    //damageables that exit range added to list, stop attacking
    protected virtual void OnTriggerExit(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Damageables.Remove(damageable);
            if (Damageables.Count == 0)
            {
                attackCoroutine = null;
            }
        }

        
    }

    //attack closest damageable
    protected virtual IEnumerator Attack()
    {
        WaitForSeconds wait = new(attackCooldown);

        yield return wait;

        IDamageable closestDamageable = null;
        float closestDistance = float.MaxValue; //initialize to max possible range

        //get distance of each damageable, save closest
        while (Damageables.Count > 0)
        {
            for (int i = 0; i < Damageables.Count; i++)
            {
                Transform damageableTransform = Damageables[i].GetTransform();
                float distance = Vector3.Distance(transform.position, damageableTransform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestDamageable = Damageables[i];
                }
            }

            if (closestDamageable != null)
            {
                onAttack?.Invoke(closestDamageable);
                closestDamageable.TakeDamage(damage);
            }

            //reset damageables
            closestDamageable = null;
            closestDistance = float.MaxValue;

            yield return wait;

            Damageables.RemoveAll(DisableDamageables);
        }

        attackCoroutine = null;      
    }

    protected bool DisableDamageables(IDamageable Damageable)
    {
        return Damageable != null && !Damageable.GetTransform().gameObject.activeSelf;
    }

    public void SetAttackRadius(float attackRadius) { 
        gameObject.GetComponent<SphereCollider>().radius = attackRadius;
    }
}
