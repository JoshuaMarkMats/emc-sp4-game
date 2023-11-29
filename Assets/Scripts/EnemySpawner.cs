using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    public Transform player;
    public int numberOfEnemiesToSpawn = 5;
    public float SpawnDelay = 1f;
    public List<EnemyScriptableObject> Enemies= new List<EnemyScriptableObject>();
    public SpawnMethod enemySpawnMethod = SpawnMethod.RoundRobin;

    [SerializeField]
    public UnityEvent enemyDiedEvent= new UnityEvent();

    private NavMeshTriangulation triangulation;
    private Dictionary<int, ObjectPool> enemyObjectPool = new Dictionary<int, ObjectPool>();

    private ObjectPool damageNumbers;
    [SerializeField]
    private DamageNumber damageNumberObject;

    public void Awake()
    {
        damageNumbers = ObjectPool.CreateInstance(damageNumberObject, 10);
        for (int i = 0; i<Enemies.Count; i++)
        {
            enemyObjectPool.Add(i, ObjectPool.CreateInstance(Enemies[i].Prefab, numberOfEnemiesToSpawn));
        }
    }
    void Start()
    {
        triangulation = NavMesh.CalculateTriangulation();

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds Wait = new WaitForSeconds(SpawnDelay);

        int spawnedEnemies = 0;

        while (spawnedEnemies < numberOfEnemiesToSpawn)
        {
            if (enemySpawnMethod == SpawnMethod.RoundRobin)
            {
                SpawnRoundRobinEnemy(spawnedEnemies);
            }
            else if (enemySpawnMethod == SpawnMethod.Random)
            {
                SpawnRandomEnemy();
            }

            spawnedEnemies++;

            yield return Wait;
        }
    }

    private void SpawnRandomEnemy()
    {
        DoSpawnEnemy(Random.Range(0, Enemies.Count));
    }

    private void SpawnRoundRobinEnemy(int spawnedEnemies)
    {
        int spawnIndex = spawnedEnemies % Enemies.Count;

        DoSpawnEnemy(spawnIndex);
    }

    private void DoSpawnEnemy(int spawnIndex)
    {
        PoolableObject poolableObject = enemyObjectPool[spawnIndex].GetObject();

        if(poolableObject != null)
        {
            Enemy enemy = poolableObject.GetComponent<Enemy>();
            Enemies[spawnIndex].SetupEnemy(enemy);

            int vertexIndex = Random.Range(0, triangulation.vertices.Length);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(triangulation.vertices[vertexIndex], out hit, 2f, 1))
            {
                enemy.agent.Warp(hit.position);
                enemy.movement.target = player;
                enemy.agent.enabled = true;
                enemy.agent.isStopped = false;
                enemy.SetMaxHealth();
                enemy.damageNumbers = damageNumbers;
                enemy.movement.Spawn();              
            }
            else
            {
                Debug.LogError("Unable to place navmesh");
            }
        }
    }

    public enum SpawnMethod
    {
        RoundRobin,
        Random
    }    
}
