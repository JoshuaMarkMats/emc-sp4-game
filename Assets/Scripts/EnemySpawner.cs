using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    // public Transform player;
    // public int numberOfEnemiesToSpawn = 5;
    // public float SpawnDelay = 1f;
    // public List<Enemy> enemyPrefab = new List<Enemy>();

    // private NavMeshTriangulation triangulation;
    // private Dictionary<int, ObjectPool> enemyObjectPool = newDisctionary<int, ObjectPool>();

    // // Start is called before the first frame update
    // void Start()
    // {
    //     triangulation = NavMesh.CalculateTriangulation();

    //     StartCoroutine(SpawnEnemies);
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    // private void DoSpawnEnemy(int spawnIndex)
    // {
    //     PoolableObjext poolableObject = enemyObjectPool[spawnIndex].GetObject();

    //     if(poolableObject != null)
    //     {
    //         Enemy enemy = poolableObject.GetComponent<Enemy>();

    //         int vertexIndex = Random.Range(0, triangulation.vertices.Length);

    //         NavMeshHit hit;
    //         //if (NavMesh.SamplePosition(triangulation.vertices)
    //     }
    // }

    // private IEnumerator SpawnEnemies()
    // {
    //     //WaitForSeconds
    // }

    // private void SpawnRoundRobinEnemy(int spawnedEnemies);


    // // public enum SpawnMethod
    // // {
    // //     RoundRobin,
    // //     Random
    // // }    
}
