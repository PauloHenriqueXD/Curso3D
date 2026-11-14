using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public bool isDead;
    public int life;
    public int maxLife;

    [Header("HUD - Barra de Vida")]
    public Image hpBar;              // arraste o objeto "HpBar" aqui
    public TextMeshProUGUI hpText;   // arraste o objeto "HpQtd" aqui (opcional)

    public int defense = 0;
    private Player player;
    public GameObject gameOverPanel;
    public GameObject playerHitFx;

    [Header("Respawn")]
    public float respawnDelay = 2f;
    public TextMeshProUGUI respawnCountdownText;

    [Header("Regeneração")]
    public int regenAmount; // quanto regenera por tick
    public float regenInterval; // a cada quantos segundos

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        UpdateHealthBar();
        StartCoroutine(HealthRegen());
    }

    public void RecoveryHP(int amount)
    {
        life += amount;

        if (life >= maxLife)
        {
            life = maxLife;
            // Debug.Log("Player HP increased: " + life);
        }

        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // evita levar dano durante a animação/delay de morte

        int finalDamage = Mathf.Max(1, damage - defense);
        Debug.Log("Player takes damage: " + finalDamage);

        PlayerSoundManager.Instance.PlayGetHitSfx();
        Player.Instance.ShakeCamera();
        GameObject hitEffect = Instantiate(playerHitFx, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        Destroy(hitEffect, 2f);
        life -= finalDamage;

        DamageNumberSpawner.Instance.Spawn(finalDamage, transform.position, Color.red);

        if (life <= 0)
        {
            Death();
        }
        else
        {
            player.Animator.SetTrigger("Hit");
            StartCoroutine(DelayedHit());
        }
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (hpBar != null)
            hpBar.fillAmount = maxLife > 0 ? (float)life / maxLife : 0f;

        if (hpText != null)
            hpText.text = $"{life}/{maxLife}";
    }

    public void RestarGame()
    {
        SceneManager.LoadScene("main_menu");
        Destroy(gameObject);
    }

    private void Death()
    {
        Debug.Log("Player died");
        isDead = true;
        player.Animator.SetTrigger("Die");
        player.isPaused = true;

        Inventory.Instance.DropAllItems(transform.position);

        // limpa o alvo travado para não correr atrás dele ao renascer
        if (TargetingSystem.Instance != null)
            TargetingSystem.Instance.ClearTarget();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        StartCoroutine(RespawnRoutine());
    }
    IEnumerator RespawnRoutine()
    {
        float timer = respawnDelay;

        while (timer > 0f)
        {
            if (respawnCountdownText != null)
                respawnCountdownText.text = Mathf.CeilToInt(timer).ToString();

            yield return null;
            timer -= Time.deltaTime;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Respawn();
    }

    public void Respawn()
    {
        Transform point = GameManagement.Instance != null ? GameManagement.Instance.respawnPoint : null;

        if (point != null)
            transform.position = point.position;
        else
            Debug.LogWarning("Nenhum respawn point encontrado no GameManager da cena!");

        life = maxLife;
        isDead = false;
        UpdateHealthBar();

        player.Animator.ResetTrigger("Die");
        player.Animator.SetTrigger("Respawn");

        player.isPaused = false;
    }

    IEnumerator DelayedHit()
    {
        player.isPaused = true;
        yield return new WaitForSeconds(0.5f);
        // se morreu durante o stun, não despausa (senão o player anda morto)
        if (!isDead)
            player.isPaused = false;
    }

    IEnumerator HealthRegen()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenInterval);

            if (!isDead && life < maxLife)
            {
                RecoveryHP(regenAmount);
            }
        }
    }
}