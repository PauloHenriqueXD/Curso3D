using UnityEngine;

public interface IPatrolEnemy
{
    EnemySpawner spawner { get; set; }
    Transform[] waypoints { get; set; }
}