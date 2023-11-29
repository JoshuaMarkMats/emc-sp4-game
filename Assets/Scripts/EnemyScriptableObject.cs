using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Enemy Configuration", menuName = "ScriptableObject/Enemy Configuration")]
public class EnemyScriptableObject : ScriptableObject
{
    //Stats
    public int health = 100;
    public Enemy Prefab;
    public AttackScriptableObject AttackConfigurations;
    public float detectionRange = 5f;
    public float fieldOfView = 90f;
    public EnemyState DefaultState;
    public float idleLocationRadius = 4f;
    public float idleMovespeedMultiplier = 0.5f;

    //NavMesh Configs
    public float AIUpdateInterval = 0.1f;
    public float Acceleration = 8f;
    public float AngularSpeed = 120;
    public int AreaMask = -1;
    public int AvoidancePriority = 50;
    public float BaseOffset = 0;
    public float Height = 2f;
    public ObstacleAvoidanceType ObstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    public float Radius = 0.5f;
    public float Speed = 3f;
    public float StoppingDistance = 0.5f;

    public void SetupEnemy(Enemy enemy)
    {
        enemy.agent.acceleration = Acceleration;
        enemy.agent.angularSpeed = AngularSpeed;
        enemy.agent.areaMask = AreaMask;
        enemy.agent.avoidancePriority = AvoidancePriority;
        enemy.agent.baseOffset = BaseOffset;
        enemy.agent.height = Height;
        enemy.agent.radius = Radius;
        enemy.agent.speed = Speed;
        enemy.agent.stoppingDistance = StoppingDistance;       

        enemy.movement.DefaultState = DefaultState;
        enemy.movement.IdleLocationRadius = idleLocationRadius;
        enemy.movement.IdleMovespeedMultiplier = idleMovespeedMultiplier;
        enemy.movement.updateSpeed = AIUpdateInterval;

        enemy.movement.lineOfSightChecker.fieldOfView = fieldOfView;
        enemy.movement.lineOfSightChecker.sphereCollider.radius = detectionRange;
        
        enemy.health = health;

        AttackConfigurations.SetupEnemy(enemy);
    }
}
