using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float respawnTime = 5f;
    [Tooltip("0 = infinito")]
    public int maxSpawns = 0;
    public Transform[] waypoints;

    private int spawnCount = 0;

    private void Start()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (maxSpawns > 0 && spawnCount >= maxSpawns) return;

        GameObject current = Instantiate(enemyPrefab, transform.position, transform.rotation);
        spawnCount++;

        if (current.TryGetComponent<IPatrolEnemy>(out var patrolEnemy))
        {
            patrolEnemy.spawner = this;
            patrolEnemy.waypoints = waypoints;
        }
    }

    public void NotifyDeath()
    {
        if (maxSpawns > 0 && spawnCount >= maxSpawns) return;
        Invoke(nameof(SpawnEnemy), respawnTime);
    }
}