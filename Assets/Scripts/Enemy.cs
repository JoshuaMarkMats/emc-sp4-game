using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : PoolableObject
{
    //public EnemyController movement;
    public NavMeshAgent agent;
    public int health = 100;
}
