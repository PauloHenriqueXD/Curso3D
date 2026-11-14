using UnityEngine;

public class WaterDamage : MonoBehaviour
{
    [Tooltip("Dano por segundo enquanto o player está na água.")]
    public int damagePerSecond = 5;

    private float damageTimer;
    private bool playerInWater;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            playerInWater = true;
            damageTimer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            playerInWater = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!playerInWater) return;

        var player = other.GetComponent<Player>();
        if (player == null) return;

        damageTimer += Time.deltaTime;
        if (damageTimer >= 1f)
        {
            damageTimer = 0f;
            player.playerHealth.TakeDamage(damagePerSecond);
        }
    }
}