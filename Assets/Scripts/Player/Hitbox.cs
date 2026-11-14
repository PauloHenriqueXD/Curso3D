using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            int attackIndex = Player.Instance.currentAttack;

            // Busca o damage no SwordHitbox pai
            SwordHitbox swordHitbox = GetComponentInParent<SwordHitbox>();
            int damage = swordHitbox != null ? swordHitbox.damage : 0;

            other.GetComponent<Enemy>().OnHit(attackIndex, damage);
        }
    }
}