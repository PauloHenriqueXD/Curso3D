using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillData", menuName = "RPG/Skill Data")]
public class SkillData : ScriptableObject
{
    public string skillName = "Nova Skill";
    public string desc;
    public Sprite icon;

    [Tooltip("Custo de mana. Use 0 para o ataque básico.")]
    public int manaCost = 0;

    [Tooltip("Nome do trigger configurado no Animator (ex: Attack1)")]
    public string animatorTrigger = "Attack";

    [Tooltip("Índice usado em PlayerSoundManager.PlaySkillSfx")]
    public int sfxIndex = 0;

    [Tooltip("Dano do ataque adicional do ataque do player. Use 0 para ataques básicos.")]
    public int additionalDamage = 0;

    [SerializeField] private float cooldownTime;
    public float CooldownTime => cooldownTime;
}