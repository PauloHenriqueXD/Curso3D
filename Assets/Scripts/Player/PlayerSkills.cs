using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    [Tooltip("Índice 0 = ataque básico. A ordem deve casar com a lista skillKeys do InputManager.")]
    public List<SkillSlot> skills = new List<SkillSlot>();
    Player player;
    bool isAttacking;
    bool currentAttackIsAuto;
    int currentAttackIndex = -1;
    float attackCooldown;

    public bool IsAttacking => isAttacking;
    public int CurrentAttack { get; private set; }
    public bool AutoAttackEnabled { get; private set; } = true;
    public int CurrentSkillBonusDamage { get; private set; }

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (InputManager.Instance == null || player.isPaused) return;

        if (Input.GetKeyDown(InputManager.Instance.toggleAutoAttack))
        {
            ToggleAutoAttack();
        }

        var keys = InputManager.Instance.skillKeys;
        int count = Mathf.Min(skills.Count, keys.Count);

        bool manualAttackThisFrame = false;
        for (int i = 0; i < count; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                TryUseSkill(i);
                manualAttackThisFrame = true;
                break; // evita disparar mais de uma skill no mesmo frame
            }
        }

        if (!manualAttackThisFrame && AutoAttackEnabled)
        {
            HandleAutoAttack();
        }

        UpdateAttackState();
        UpdateCooldowns();
    }

    void UpdateCooldowns()
    {
        foreach (var slot in skills)
        {
            if (slot.cooldownTimer > 0f)
                slot.cooldownTimer -= Time.deltaTime;
        }
    }

    public void ToggleAutoAttack()
    {
        AutoAttackEnabled = !AutoAttackEnabled;
    }

    public void SetAutoAttack(bool enabled)
    {
        AutoAttackEnabled = enabled;
    }

    private void HandleAutoAttack()
    {
        if (player.IsMovingToTarget) return; // já indo até o alvo, espera chegar
        if (isAttacking) return;              // já no meio de um golpe (auto), espera acabar

        var target = TargetingSystem.Instance != null ? TargetingSystem.Instance.CurrentTarget : null;
        if (target == null) return; // sem alvo travado, não faz nada

        TryUseSkillInternal(0, manual: false); // auto-attack sempre usa o ataque básico
    }

    // chamado pelo Update() acima (input manual) e também pode ser chamado por botões de UI
    public void TryUseSkill(int index)
    {
        TryUseSkillInternal(index, manual: true);
    }

    private void TryUseSkillInternal(int index, bool manual)
    {
        if (index < 0 || index >= skills.Count) return;
        if (player.isPaused) return;

        if (isAttacking)
        {
            // só permite interromper se for manual, o golpe atual for do auto-attack,
            // E a skill escolhida for DIFERENTE da que já está em andamento
            bool canInterrupt = manual && currentAttackIsAuto && index != currentAttackIndex;
            if (!canInterrupt) return;
        }

        var slot = skills[index];
        if (slot.data == null) return;
        if (slot.IsOnCooldown) return;
        if (player.playerMana.mana < slot.data.manaCost) return;

        bool isAuto = !manual;

        if (player.TryAttackWithTarget(() => DoSkill(index, isAuto))) return;

        DoSkill(index, isAuto);
    }

    void DoSkill(int index, bool isAuto)
    {
        var slot = skills[index];

        if (slot.data.manaCost > 0 && !player.playerMana.UseMana(slot.data.manaCost)) return;

        if (index > 0) CurrentAttack = index;

        CurrentSkillBonusDamage = slot.data.additionalDamage;

        player.Animator.SetTrigger(slot.data.animatorTrigger);
        isAttacking = true;
        currentAttackIsAuto = isAuto;
        currentAttackIndex = index;
        attackCooldown = 0.53f;

        slot.cooldownTimer = slot.data.CooldownTime;

        if (slot.effect != null) slot.effect.Play();

        PlayerSoundManager.Instance.PlaySkillSfx(slot.data.sfxIndex);
    }

    void UpdateAttackState()
    {
        if (!isAttacking) return;

        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f) isAttacking = false;
    }
}