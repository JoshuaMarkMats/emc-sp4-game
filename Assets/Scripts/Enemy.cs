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

    private Coroutine lookCoroutine, stayStillCoroutine;
    private const string ATTACK_TRIGGER = "Attack";

    private void Awake()
    {
        attackRadius.onAttack += OnAttack;
        
    }

    private void OnAttack(IDamageable Target)
    {
        animator.SetTrigger(ATTACK_TRIGGER);

        stayStillCoroutine = StartCoroutine(StayStill(attackRadius.attackCooldown));

        if (lookCoroutine!= null )
        {
            StopCoroutine(lookCoroutine);
        }

        lookCoroutine = StartCoroutine(LookAt(Target.GetTransform()));
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
    
    //stay still while attacking
    private IEnumerator StayStill(float duration)
    {
        agent.isStopped= true;
        yield return new WaitForSeconds(duration);
        agent.isStopped = false;

    }

    public void setupHealth()
    {
        healthBar.SetMaxHealth(health);
    }

    public virtual void OnEnable()
    {
        SetupAgentFromConfiguration();  
    }

    public override void OnDisable()
    {
        base.OnDisable();
        agent.enabled = false;
    }

    public virtual void SetupAgentFromConfiguration()
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
    }

    public void TakeDamage(int Damage)
    {
        health -= Damage;
        healthBar.SetHealth(health);
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
