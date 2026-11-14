using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    public int damage;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            // só causa dano durante um golpe — antes, encostar a espada no inimigo já dava dano
            if (!Player.Instance.skills.IsAttacking) return;

            Enemy enemy = other.GetComponentInParent<Enemy>();
            if (enemy == null) return;

            // Calcula o dano total considerando o ataque atual do player
            int attackIndex = Player.Instance.currentAttack;
            int totalDamage = damage + Player.Instance.skills.CurrentSkillBonusDamage;
            enemy.OnHit(attackIndex, totalDamage);

            // Chama o número flutuante amarelo no inimigo
            DamageNumberSpawner.Instance.Spawn(totalDamage, other.transform.position, Color.yellow);
        }
    }
}
