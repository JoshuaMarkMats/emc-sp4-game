using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyLineOfSightChecker : MonoBehaviour
{
    public SphereCollider sphereCollider;
    public float fieldOfView = 90f;
    public LayerMask lineOfSightLayers;
    
    public delegate void GainSightEvent(Player player);
    public GainSightEvent onGainSight;
    public delegate void LoseSightEvent(Player player);
    public LoseSightEvent onLoseSight;

    private Coroutine checkForLineOfSightCoroutine;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("player entered");
        if (other.TryGetComponent<Player>(out Player player))
        {

            if (!CheckLineOfSight(player))
            {
                checkForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(player));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player;
        if (other.TryGetComponent<Player>(out player))
        {
            onLoseSight?.Invoke(player);
            if (checkForLineOfSightCoroutine != null)
                StopCoroutine(checkForLineOfSightCoroutine);
        }
    }

    private bool CheckLineOfSight(Player player)
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        if (Vector3.Dot(transform.forward,direction) >= Mathf.Cos(fieldOfView))
        {
            onGainSight?.Invoke(player);
        }
        return false;
    }

    private IEnumerator CheckForLineOfSight(Player player)
    {
        WaitForSeconds wait = new(0.1f);

        while (!CheckLineOfSight(player))
        {
            yield return wait;
        }
    }
}
