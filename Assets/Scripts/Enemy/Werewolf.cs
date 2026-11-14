using UnityEngine;
using UnityEngine.AI;

public class Werewolf : Enemy
{
    bool isPlayerVisible;
    bool isAlerting;
    private bool isDead = false;
    public float moveSpeed;
    private NavMeshAgent agent;
    public float detectionRadius;
    public float persecutionRadius;
    public float rotationSpeed;
    public GameObject canvaAlert;
    public float attackRange;
    public float attackCooldown = 2f;
    private float lastAttackTime;
    float distance;
    public EnemyDamageHitbox weaponHitbox;
    public Transform lootSpawnPoint;
    public AudioClip spawnItemSfx;
    public AudioClip werewolfHitSfx;
    public AudioClip werewolfDieSfx;
    [Range(0f, 100f)]
    public float dropChance = 60f;

    [Header("Combat")]
    public float stopBuffer = 0f;

    [System.Serializable]
    public class LootEntry
    {
        public GameObject prefab;
        [Range(0f, 100f)]
        public float weight;
    }
    public LootEntry[] loots;

    protected override void Start()
    {
        base.Start();
        weaponHitbox.damage = damage;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
    }

    private void Update()
    {
        // Se o inimigo estiver morto, não faz nada
        if (isDead) return;

        // Rotaciona para o player se ele estiver visível
        distance = Vector3.Distance(transform.position, Player.Instance.transform.position);

        // Perde o player se sair do raio de detecção
        if (distance > detectionRadius)
        {
            isPlayerVisible = false;
            isAlerting = false;
            animator.SetBool("Alert", false);
            agent.speed = patrolSpeed; // volta para a velocidade de patrulha depois da perseguição
            animator.SetFloat("Speed", patrolMode != PatrolMode.None ? 0.5f : 0f);
            ResetLife();
            if (!isWaiting) CheckIfStuck();
            Patrol();
            return;
        }

        // Acabou de detectar o player
        if (isPlayerVisible == false)
        {
            isPlayerVisible = true;
            isAlerting = true;
            animator.SetBool("Alert", true);
            GameObject alert = Instantiate(canvaAlert);
            alert.GetComponent<EnemyAlert>().target = transform;
            Destroy(alert, 2f);
        }
        else
        {
            // Pausa a animação de alerta
            if (distance < persecutionRadius && isAlerting)
            {
                isAlerting = false;
                animator.SetBool("Alert", false);
            }

            // Persegue se entrou no raio de perseguição
            if (distance < persecutionRadius && distance > (attackRange + stopBuffer))
            {
                animator.SetFloat("Speed", 1f);
                agent.speed = moveSpeed;
                agent.SetDestination(Player.Instance.transform.position);
            }
            else
            {
                animator.SetFloat("Speed", 0f);
                agent.ResetPath();
                agent.velocity = Vector3.zero;
                RotateTowardsPlayer();
            }

            // Ataca se chegou perto
            if (distance <= (attackRange + stopBuffer) && Time.time - lastAttackTime >= attackCooldown && Player.Instance.playerHealth.isDead == false)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    public override void Attack()
    {
        if (isDead) return;
        animator.SetTrigger("Attack");
        if (werewolfHitSfx != null)
            AudioManager.Instance.PlaySFX(werewolfHitSfx);

        // ativa o hitbox no momento do ataque e desativa após o tempo do golpe
        weaponHitbox.EnableHitbox();
        Invoke(nameof(DisableWeaponHitbox), 0.5f);
    }

    private void DisableWeaponHitbox()
    {
        weaponHitbox.DisableHitbox();
    }

    public override void Patrol()
    {
        if (patrolMode == PatrolMode.Waypoints)
        {
            if (isWaiting)
            {
                patrolWaitTimer -= Time.deltaTime;
                agent.ResetPath();
                animator.SetFloat("Speed", 0f);
                if (patrolWaitTimer <= 0f)
                    isWaiting = false;
                return;
            }

            if (waypoints == null || waypoints.Length == 0) return;

            Transform target = waypoints[currentWaypoint];
            Vector3 direction = target.position - transform.position;
            direction.y = 0;

            if (direction.magnitude <= waypointStopDistance)
            {
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
                isWaiting = true;
                patrolWaitTimer = patrolWaitTime;
                agent.ResetPath();
                animator.SetFloat("Speed", 0f);
            }
            else
            {
                agent.SetDestination(target.position);
            }
        }
        else if (patrolMode == PatrolMode.Random)
        {
            if (isWaiting)
            {
                patrolWaitTimer -= Time.deltaTime;
                agent.ResetPath();
                animator.SetFloat("Speed", 0f);
                if (patrolWaitTimer <= 0f)
                    isWaiting = false;
                return;
            }

            if (!isRandomTargetSet)
                PickRandomTarget();

            Vector3 direction = randomPatrolTarget - transform.position;
            direction.y = 0;

            if (direction.magnitude <= waypointStopDistance)
            {
                isRandomTargetSet = false;
                isWaiting = true;
                patrolWaitTimer = patrolWaitTime;
                agent.ResetPath();
                animator.SetFloat("Speed", 0f);
            }
            else
            {
                agent.SetDestination(randomPatrolTarget);
            }
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = (Player.Instance.transform.position - transform.position).normalized;
        direction.y = 0; // Manter a rotação apenas no plano horizontal

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public override void Death()
    {
        if (isDead) return;
        isDead = true;

        base.Death(); // notifica o spawner

        weaponHitbox.GetComponent<Collider>().enabled = false; // desativa a garra imediatamente

        animator.SetTrigger("Die");
        if (werewolfDieSfx != null)
        {
            AudioManager.Instance.PlaySFX(werewolfDieSfx);
        }
        rb.useGravity = false; // Disable gravity to prevent the crab from falling through the ground
        coll.enabled = false; // Disable the collider to prevent further interactions
        agent.enabled = false; // para o NavMeshAgent para o corpo não continuar andando durante a animação de morte
        float dropRoll = Random.Range(0f, 100f);
        if (dropRoll <= dropChance)
        {
            GameObject loot = GetWeightedLoot();
            if (loot != null)
            {
                Instantiate(loot, lootSpawnPoint.position, Quaternion.identity);
                if (spawnItemSfx != null)
                    AudioManager.Instance.PlaySFX(spawnItemSfx);
            }
        }
        Destroy(gameObject, 2f); // Destroy the crab after the death animation plays
        enabled = false; // Disable the script to stop further updates
        Debug.Log(enemyName + " died");
    }

    private GameObject GetWeightedLoot()
    {
        float totalWeight = 0f;
        foreach (var entry in loots)
            totalWeight += entry.weight;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in loots)
        {
            cumulative += entry.weight;
            if (roll <= cumulative)
                return entry.prefab;
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        // Desenha a esfera de detecção no editor para visualização
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        // Desenha a esfera de perseguição no editor para visualização
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, persecutionRadius);
        // Desenha a esfera de ataque no editor para visualização
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
