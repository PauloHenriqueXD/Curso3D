using UnityEngine;

public class EnemyDamageHitbox : MonoBehaviour
{
    public int damage;
    private bool isActive = false;
    private bool hasHitThisAttack = false;

    public void EnableHitbox()
    {
        isActive = true;
        hasHitThisAttack = false;
    }

    public void DisableHitbox()
    {
        isActive = false;
    }

    // OnTriggerStay em vez de OnTriggerEnter: se o player já estava encostado na arma
    // quando o golpe começou, o Enter não disparava de novo e o ataque não dava dano.
    public void OnTriggerStay(Collider other)
    {
        if (!isActive || hasHitThisAttack) return;

        if (other.CompareTag("Player") && other.TryGetComponent<PlayerHealth>(out var health))
        {
            health.TakeDamage(damage);
            hasHitThisAttack = true; // um acerto por golpe
        }
    }
}