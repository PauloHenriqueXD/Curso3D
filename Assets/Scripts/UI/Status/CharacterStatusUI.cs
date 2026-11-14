using TMPro;
using UnityEngine;

public class CharacterStatusUI : MonoBehaviour
{
    public InventorySlot equippedRightSlot;
    public InventorySlot equippedLeftSlot;

    [Header("Stats")]
    public TextMeshProUGUI attackValue;
    public TextMeshProUGUI defenseValue;
    public TextMeshProUGUI healthValue;
    public TextMeshProUGUI manaValue;
    public TextMeshProUGUI speedValue;

    private void Start()
    {
        if (Player.Instance != null && Player.Instance.equipmentManager != null)
        {
            Player.Instance.equipmentManager.OnEquip += HandleEquip;
            Player.Instance.equipmentManager.OnUnequip += HandleUnequip;
        }
    }

    private void Update()
    {
        if (Player.Instance == null) return;
        healthValue.text = $"{Player.Instance.playerHealth.life} / {Player.Instance.playerHealth.maxLife}";
        manaValue.text = $"{Player.Instance.playerMana.mana} / {Player.Instance.playerMana.maxMana}";
        speedValue.text = Player.Instance.moveSpeed.ToString("F0");
    }

    private void OnDestroy()
    {
        if (Player.Instance != null && Player.Instance.equipmentManager != null)
        {
            Player.Instance.equipmentManager.OnEquip -= HandleEquip;
            Player.Instance.equipmentManager.OnUnequip -= HandleUnequip;
        }
    }

    private void OnEnable()
    {
        RefreshAllSlots();
        RefreshStats();
    }

    private void RefreshAllSlots()
    {
        if (Player.Instance == null || Player.Instance.equipmentManager == null) return;

        var em = Player.Instance.equipmentManager;

        // Mão direita (MainHand)
        var mainHand = em.GetEquipped(ItemData.EquipmentSlot.MainHand);
        equippedRightSlot.ResetSlot();
        if (mainHand != null) equippedRightSlot.AddItem(mainHand);

        // Mão esquerda (OffHand)
        var offHand = em.GetEquipped(ItemData.EquipmentSlot.OffHand);
        equippedLeftSlot.ResetSlot();
        if (offHand != null) equippedLeftSlot.AddItem(offHand);
    }

    private void RefreshStats()
    {
        if (Player.Instance == null) return;

        // Defesa vem direto do PlayerHealth
        defenseValue.text = Player.Instance.playerHealth.defense.ToString();

        // Ataque vem do item equipado na MainHand
        var mainHand = Player.Instance.equipmentManager.GetEquipped(ItemData.EquipmentSlot.MainHand);
        attackValue.text = mainHand != null ? mainHand.damage.ToString() : "0";

        // Vida e Mana vêm do PlayerHealth e PlayerMana
        healthValue.text = $"{Player.Instance.playerHealth.life} / {Player.Instance.playerHealth.maxLife}";
        manaValue.text = $"{Player.Instance.playerMana.mana} / {Player.Instance.playerMana.maxMana}";

        // Velocidade vem do Player
        speedValue.text = Player.Instance.moveSpeed.ToString();
    }

    private void HandleEquip(ItemData.EquipmentSlot slot, ItemData item)
    {
        var targetSlot = slot == ItemData.EquipmentSlot.OffHand
            ? equippedLeftSlot
            : equippedRightSlot;

        if (targetSlot != null)
        {
            targetSlot.ResetSlot();
            targetSlot.AddItem(item);
        }

        RefreshStats();
    }

    private void HandleUnequip(ItemData.EquipmentSlot slot, ItemData item)
    {
        var targetSlot = slot == ItemData.EquipmentSlot.OffHand
            ? equippedLeftSlot
            : equippedRightSlot;

        if (targetSlot != null)
            targetSlot.ResetSlot();

        RefreshStats();
    }
}