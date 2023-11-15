using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : PoolableObject, IDamageable
{
    public AttackRadius attackRadius;
    public Animator animator;
    public EnemyController movement;
    public NavMeshAgent agent;
    public EnemyScriptableObject enemyScriptableObject;
    public int health = 100;

    [SerializeField]
    private HealthBar healthBar;
    public ObjectPool damageNumbers;

    [SerializeField]
    private float deathDuration = 3;
    public bool isAlive = true;

    private Coroutine lookCoroutine, stayStillCoroutine, deathCoroutine;
    private const string ATTACK_TRIGGER = "Attack", DEATH_TRIGGER = "ToggleDeath";

    private void Awake()
    {
        attackRadius.onAttack += OnAttack;
    }

    private void Update()
    {
        if (isAlive && health <= 0)
        {
            deathCoroutine = StartCoroutine(Death(deathDuration));
        }
    }

    public bool IsAlive { get { return isAlive; } }

    private void OnAttack(IDamageable Target)
    {
        if (isAlive)
        {
            animator.SetTrigger(ATTACK_TRIGGER);

            stayStillCoroutine = StartCoroutine(StayStill(attackRadius.attackCooldown));

            if (lookCoroutine != null)
            {
                StopCoroutine(lookCoroutine);
            }

            lookCoroutine = StartCoroutine(LookAt(Target.GetTransform()));
        }
    }

    private IEnumerator LookAt(Transform Target)
    {
        if (isAlive)
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
    }
    
    //stay still while attacking
    private IEnumerator StayStill(float duration)
    {
        agent.isStopped= true;
        yield return new WaitForSeconds(duration);
        if (agent.enabled)
            agent.isStopped = false;

    }

    public void SetMaxHealth()
    {
        healthBar.SetMaxHealth(health);
    }

    public virtual void OnEnable()
    {
        //SetupAgentFromConfiguration();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        agent.enabled = false;
    }

    /*public virtual void SetupAgentFromConfiguration()
    {
        agent.acceleration = enemyScriptableObject.Acceleration;
        agent.angularSpeed= enemyScriptableObject.AngularSpeed;
        agent.areaMask= enemyScriptableObject.AreaMask;
        agent.avoidancePriority= enemyScriptableObject.AvoidancePriority;
        agent.baseOffset= enemyScriptableObject.BaseOffset;
        agent.height= enemyScriptableObject.Height;
        agent.obstacleAvoidanceType=enemyScriptableObject.ObstacleAvoidanceType;
        agent.radius = enemyScriptableObject.Radius;
        agent.speed = enemyScriptableObject.Speed;
        agent.stoppingDistance = enemyScriptableObject.StoppingDistance;

        movement.updateSpeed = enemyScriptableObject.AIUpdateInterval;
        health = enemyScriptableObject.health;
        attackRadius.attackCooldown = enemyScriptableObject.attackCooldown;
        attackRadius.damage = enemyScriptableObject.damage;
        attackRadius.SetAttackRadius(enemyScriptableObject.attackRadius);
    }*/

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

        if (health > Damage)
        {
            health -= Damage;
            healthBar.SetHealth(health);
        }
        else
        {
            health = 0;
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
        if (stayStillCoroutine != null)
        {
            StopCoroutine(stayStillCoroutine);
        }
        agent.enabled = false;
        animator.SetTrigger(DEATH_TRIGGER);
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
}
