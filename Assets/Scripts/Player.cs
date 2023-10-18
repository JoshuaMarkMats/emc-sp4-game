using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField]
    private AttackRadius attackRadius;
    [SerializeField]
    private Animator animator;
    private Coroutine lookCoroutine;

    [SerializeField]
    private HealthBar healthBar;

    [SerializeField]
    private int Health = 300;

    private const string ATTACK_TRIGGER = "Attack";

    private void Awake()
    {
        attackRadius.onAttack += OnAttack;
        healthBar.SetMaxHealth(Health);
    }

    private void OnAttack(IDamageable Target)
    {
        animator.SetTrigger(ATTACK_TRIGGER);

        if (lookCoroutine != null)
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

    public void TakeDamage(int Damage)
    {
        Health -= Damage;
        healthBar.SetHealth(Health);
        if (Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
