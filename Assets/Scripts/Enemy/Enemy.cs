using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IPatrolEnemy
{
    public string enemyName = "Enemy";
    public int life;
    protected int maxLife;
    public int MaxLife => maxLife;
    public int damage;
    public GameObject[] hitFx;
    public float hitYOffset;
    public Animator animator;
    public Renderer enemyRenderer;
    public Collider coll;
    public Rigidbody rb;
    public AudioClip hitSfx;
    public event System.Action<int, int> OnHealthChanged;
    public event System.Action OnDeath;
    public enum PatrolMode { None, Waypoints, Random }
    public PatrolMode patrolMode;
    [HideInInspector] public EnemySpawner spawner { get; set; }

    [Header("Patrol")]
    [SerializeField] private Transform[] _waypoints;
    public Transform[] waypoints
    {
        get => _waypoints;
        set => _waypoints = value;
    }
    public float patrolRadius = 5f; // raio para gerar os pontos de patrulha aleatórios, usado apenas no modo Random
    public float patrolSpeed = 1f; // velocidade de patrulha, usada tanto para Waypoints quanto para Random
    public float waypointStopDistance = 0.3f; // distância para considerar que chegou no waypoint, para evitar ficar oscilando quando chegar perto demais
    protected Vector3 startPosition; // para o modo Random, para garantir que os pontos sejam gerados ao redor do local de spawn
    public float patrolWaitTime = 3f; // tempo de espera nos waypoints
    protected float patrolWaitTimer = 0f; // para controlar o tempo de espera nos waypoints
    protected bool isWaiting = false; // para controlar o tempo de espera nos waypoints
    protected Vector3 lastPosition; // para detectar se o inimigo está preso
    protected float stuckTimer = 0f; // tempo parado para detectar se está preso
    public float stuckThreshold = 3f; // segundos parado antes de resetar
    public float stuckDistanceThreshold = 0.005f; // distância mínima para considerar que está se movendo
    protected int currentWaypoint = 0; // índice do waypoint atual, usado apenas no modo Waypoints
    protected Vector3 randomPatrolTarget; // alvo atual para patrulha aleatória, usado apenas no modo Random
    protected bool isRandomTargetSet = false; // para controlar se o alvo aleatório já foi definido, usado apenas no modo Random
    protected float stuckCheckInterval = 0.5f; // intervalo para checar se está preso, para evitar checar a cada frame e acabar com o desempenho
    protected float stuckCheckTimer = 0f; // timer para controlar o intervalo de checagem de travamento

    protected virtual void Start()
    {
        maxLife = life;
        startPosition = transform.position;
        lastPosition = transform.position;
    }

    public virtual void Move()
    {
        
    }

    public virtual void Attack()
    {

    }

    public virtual void ResetLife()
    {
        life = maxLife;
        OnHealthChanged?.Invoke(life, maxLife);
    }

    public void OnHit(int attackIndex, int damage)
    {
        AudioManager.Instance.PlaySFX(hitSfx);

        life -= damage;
        OnHealthChanged?.Invoke(life, maxLife);

        if (life <= 0)
        {
            Death();
            return;
        }
        else
        {
            // pega o efeito correspondente ao ataque (attackIndex 1~4 → índice 0~3)
            int fxIndex = Mathf.Clamp(attackIndex - 1, 0, hitFx.Length - 1);

            animator.SetTrigger("Hit");

            Player.Instance.ShakeCamera();

            StartCoroutine(HitFlash());

            GameObject hitEffect = Instantiate(hitFx[fxIndex], transform.position + new Vector3(0, hitYOffset, 0), transform.rotation);
            Destroy(hitEffect, 2f);
        }
    }

    IEnumerator HitFlash()
    {
        enemyRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        enemyRenderer.material.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        enemyRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        enemyRenderer.material.color = Color.white;
    }

    public virtual void Patrol()
    {
        if (patrolMode == PatrolMode.Waypoints)
        {
            if (waypoints == null || waypoints.Length == 0) return;

            Transform target = waypoints[currentWaypoint];
            Vector3 direction = target.position - transform.position;
            direction.y = 0;

            if (direction.magnitude <= waypointStopDistance)
            {
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            }
            else
            {
                rb.MovePosition(rb.position + direction.normalized * patrolSpeed * Time.fixedDeltaTime);
                if (direction.magnitude > 0.5f)
                    RotateTowards(direction.normalized);
            }
        }
        else if (patrolMode == PatrolMode.Random)
        {
            if (!isRandomTargetSet)
                PickRandomTarget();

            Vector3 direction = randomPatrolTarget - transform.position;
            direction.y = 0;

            if (direction.magnitude <= waypointStopDistance)
            {
                isRandomTargetSet = false;
            }
            else
            {
                rb.MovePosition(rb.position + direction.normalized * patrolSpeed * Time.fixedDeltaTime);
                if (direction.magnitude > 0.5f)
                    RotateTowards(direction.normalized);
            }
        }
    }

    protected void PickRandomTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        randomPatrolTarget = startPosition + new Vector3(randomCircle.x, 0, randomCircle.y);
        isRandomTargetSet = true;
    }

    protected void CheckIfStuck()
    {
        stuckCheckTimer += Time.deltaTime;

        if (stuckCheckTimer < stuckCheckInterval) return; // só checa a cada 0.5s

        float moved = Vector3.Distance(transform.position, lastPosition);
        // Debug.Log($"Stuck timer: {stuckTimer:F1} | moved: {moved:F3}");

        if (moved < stuckDistanceThreshold)
        {
            stuckTimer += stuckCheckInterval;
            if (stuckTimer >= stuckThreshold)
            {
                // Debug.Log("RESETOU por travamento!");
                stuckTimer = 0f;
                isRandomTargetSet = false;
                isWaiting = false;
                currentWaypoint = (currentWaypoint + 1) % Mathf.Max(1, waypoints.Length);
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;
        stuckCheckTimer = 0f;
    }

    protected void RotateTowards(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    public virtual void Death()
    {
        OnDeath?.Invoke();
        if (spawner != null)
            spawner.NotifyDeath();
    }
}
