using UnityEngine;

[System.Serializable]
public class SkillSlot
{
    public SkillData data;
    [Tooltip("Opcional. Deixe vazio se a skill não tiver efeito de partícula (caso do ataque básico).")]
    public ParticleSystem effect;

    [System.NonSerialized] public float cooldownTimer;
    public bool IsOnCooldown => cooldownTimer > 0f;
    public float CooldownPercent => data != null && data.CooldownTime > 0f
        ? cooldownTimer / data.CooldownTime
        : 0f;
}
