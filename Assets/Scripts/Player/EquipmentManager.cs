using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    private Dictionary<ItemData.EquipmentSlot, ItemData> _equipped = new();

    public event System.Action<ItemData.EquipmentSlot, ItemData> OnEquip;
    public event System.Action<ItemData.EquipmentSlot, ItemData> OnUnequip;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        _equipped.Clear();
        foreach (ItemData.EquipmentSlot slot in System.Enum.GetValues(typeof(ItemData.EquipmentSlot)))
            _equipped[slot] = null;
    }

    public bool TryEquip(ItemData item, out string failReason)
    {
        failReason = "";
        // Debug.Log($"Tentando equipar {item.itemName} no slot {item.allowedSlot}");

        if (item.allowedSlot == ItemData.EquipmentSlot.None)
        {
            failReason = "Este item não pode ser equipado.";
            Debug.Log(failReason);
            return false;
        }

        if (item.isTwoHanded && !IsSlotEmpty(ItemData.EquipmentSlot.OffHand))
        {
            failReason = "Desequipe o escudo antes de usar uma arma de duas mãos.";
            Debug.Log(failReason);
            return false;
        }

        if (item.allowedSlot == ItemData.EquipmentSlot.OffHand)
        {
            var mainHand = GetEquipped(ItemData.EquipmentSlot.MainHand);
            if (mainHand != null && mainHand.isTwoHanded)
            {
                failReason = "Não é possível usar escudo com uma arma de duas mãos.";
                Debug.Log(failReason);
                return false;
            }
        }

        if (!IsSlotEmpty(item.allowedSlot))
            ForceUnequip(item.allowedSlot);

        _equipped[item.allowedSlot] = item;

        if (item.defense > 0)
            Player.Instance.playerHealth.defense += item.defense;

        OnEquip?.Invoke(item.allowedSlot, item);
        // Debug.Log($"Item equipado: {item.itemName}");
        return true;
    }

    public void Unequip(ItemData.EquipmentSlot slot)
    {
        var item = _equipped[slot];
        if (item == null) return;

        // remove defesa do item
        if (item.defense > 0)
            Player.Instance.playerHealth.defense -= item.defense;

        // mesmo que seja possível equipar um item no slot de suit, não é possível ficar sem suit
        if (slot == ItemData.EquipmentSlot.Suit)
        {
            // não pode ficar sem suit — volta para o corpo base da customização
            _equipped[slot] = null;
            OnUnequip?.Invoke(slot, item);
            BodyManager.Instance.EquipSuit(Player.Instance.baseBodyIndex);
            Inventory.Instance.AddItem(item);
            return;
        }

        _equipped[slot] = null;

        OnUnequip?.Invoke(slot, item);

        // TODO (multiplayer): substituir por referência local ao player
        if (slot == ItemData.EquipmentSlot.MainHand)
            WeaponManager.Instance.UnequipAll();

        // TODO (multiplayer): substituir por referência local ao player
        if (slot == ItemData.EquipmentSlot.OffHand)
            ShieldManager.Instance.UnequipAll();

        // TODO (multiplayer): substituir por referência local ao inventário do player
        Inventory.Instance.AddItem(item);
    }

    private void ForceUnequip(ItemData.EquipmentSlot slot)
    {
        var item = _equipped[slot];
        if (item == null) return;

        _equipped[slot] = null;

        // remove defesa do item anterior
        if (item.defense > 0)
            Player.Instance.playerHealth.defense -= item.defense;

        OnUnequip?.Invoke(slot, item);

        if (slot == ItemData.EquipmentSlot.MainHand)
            WeaponManager.Instance.UnequipAll();

        if (slot == ItemData.EquipmentSlot.OffHand)
            ShieldManager.Instance.UnequipAll();

        Inventory.Instance.AddItem(item);
    }

    public ItemData GetEquipped(ItemData.EquipmentSlot slot) =>
        _equipped.TryGetValue(slot, out var item) ? item : null;

    public bool IsSlotEmpty(ItemData.EquipmentSlot slot) =>
        GetEquipped(slot) == null;

}