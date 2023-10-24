using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField]
    private AttackRadius attackRadius;
    [SerializeField]
    private Animator animator;
    private Coroutine lookCoroutine, deathCoroutine;

    [SerializeField]
    private HealthBar healthBar;   
    private ObjectPool damageNumbers;
    [SerializeField]
    private DamageNumber damageNumberObject;

    [SerializeField]
    private int Health = 300;

    [SerializeField]
    private float deathDuration = 3;
    [SerializeField]
    private bool isAlive = true;

    private const string ATTACK_TRIGGER = "Attack", DEATH_TRIGGER = "ToggleDeath";

    private void Awake()
    {
        attackRadius.onAttack += OnAttack;
    }

    private void Start()
    {
        healthBar.SetMaxHealth(Health);
        damageNumbers = ObjectPool.CreateInstance(damageNumberObject, 20);
    }

    private void Update()
    {
        if (isAlive && Health <= 0)
        {
            deathCoroutine = StartCoroutine(Death(deathDuration));
        }
    }

    public bool IsAlive { 
        get { return isAlive; }
    }

    private void OnAttack(IDamageable Target)
    {
        if (isAlive)
        {
            animator.SetTrigger(ATTACK_TRIGGER);

            if (lookCoroutine != null)
            {
                StopCoroutine(lookCoroutine);
            }

            lookCoroutine = StartCoroutine(LookAt(Target.GetTransform()));
        }   
    }

    private IEnumerator LookAt(Transform Target)
    {
            Quaternion lookRotation = Quaternion.LookRotation(Target.position - transform.position);
            float time = 0;

            while (time < 1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);

                time += Time.deltaTime * 2;
                yield return null;
            }

            transform.rotation = lookRotation;

    }

    public void TakeDamage(int Damage)
    {
        if (isAlive)
        {
            DamageNumber damageNumber = (DamageNumber)damageNumbers.GetObject();
            if (damageNumber != null)
            {
                damageNumber.SetValue(Damage);
                damageNumber.SetStartPosition(transform.position);
                

            } 
                
                
        }

        if (Health > Damage)
        {
            Health -= Damage;
            healthBar.SetHealth(Health);
        }
        else
        {
            Health = 0;
            healthBar.SetHealth(0);
        }
            
    }

    public Transform GetTransform()
    {
        return transform;
    }

    //allow time for death before thanos snap
    private IEnumerator Death(float duration)
    {
        isAlive = false;
        attackRadius.enabled = false;
        gameObject.GetComponent<NavMeshAgent>().isStopped = true;
        animator.SetTrigger(DEATH_TRIGGER);
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
}
