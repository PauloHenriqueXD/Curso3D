using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    public static TargetingSystem Instance;

    public Enemy CurrentTarget { get; private set; }
    public float detectionRadius = 15f;
    public LayerMask enemyLayer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (InputManager.Instance == null) return;

        if (Input.GetKeyDown(InputManager.Instance.selectTarget))
        {
            SelectClosestEnemy();
        }

        if (Input.GetKeyDown(InputManager.Instance.closeMenu) && CurrentTarget != null)
        {
            ClearTarget();
        }

        // se o alvo morreu ou foi destruído, limpa
        if (CurrentTarget == null) return;

        if (!CurrentTarget.gameObject.activeInHierarchy)
        {
            ClearTarget();
        }
    }

    private void SelectClosestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        Enemy closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = enemy;
            }
        }

        if (closest != null)
        {
            SetTarget(closest);
        }
    }

    public void SetTarget(Enemy enemy)
    {
        // remove listeners do alvo anterior
        if (CurrentTarget != null)
            UnsubscribeFromTarget(CurrentTarget);

        CurrentTarget = enemy;

        SubscribeToTarget(enemy);

        Player.Instance.playerHUD.ShowEnemyInfo(enemy.enemyName);
        Player.Instance.playerHUD.UpdateEnemyHP(enemy.life, enemy.MaxLife);
    }

    public void ClearTarget()
    {
        if (CurrentTarget != null)
            UnsubscribeFromTarget(CurrentTarget);

        CurrentTarget = null;
        Player.Instance.playerHUD.HideEnemyInfo();
    }

    private void SubscribeToTarget(Enemy enemy)
    {
        enemy.OnHealthChanged += HandleTargetHealthChanged;
        enemy.OnDeath += HandleTargetDeath;
    }

    private void UnsubscribeFromTarget(Enemy enemy)
    {
        enemy.OnHealthChanged -= HandleTargetHealthChanged;
        enemy.OnDeath -= HandleTargetDeath;
    }

    private void HandleTargetHealthChanged(int current, int max)
    {
        Player.Instance.playerHUD.UpdateEnemyHP(current, max);
    }

    private void HandleTargetDeath()
    {
        ClearTarget();
    }
}