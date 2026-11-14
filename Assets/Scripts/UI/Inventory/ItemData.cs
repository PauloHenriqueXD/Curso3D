using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [System.Serializable]
    public enum ItemEffects
    {
        health,
        speed,
        mana,
        weapon,
        shield,
        armor
    }

    [System.Serializable]
    public enum EquipmentSlot
    {
        None,
        MainHand,
        OffHand,
        Suit,
        Accessory
    }

    public string itemName;
    public string desc;
    public Sprite icon;
    public GameObject prefab;
    public ItemEffects effect;
    public int damage;
    public int defense;
    public int amount;
    public int price;
    public int sellPrice;
    public int weaponIndex;
    public int shieldIndex;
    public int bodyIndex;

    [Header("World Drop")]
    public GameObject worldPickupPrefab;

    [Header("Equipment")]
    public EquipmentSlot allowedSlot = EquipmentSlot.None;
    public bool isTwoHanded = false; // armas que bloqueiam OffHand

    public void Apply()
    {
        switch (effect)
        {
            case ItemEffects.health:
                Player.Instance.playerHealth.RecoveryHP(Mathf.RoundToInt(amount));
                GameObject healthEffect = Instantiate(prefab, Player.Instance.transform.position, Quaternion.identity);
                Destroy(healthEffect, 2f);
                break;
            case ItemEffects.speed:
                Player.Instance.SpeedBoost(amount);
                GameObject speedEffect = Instantiate(prefab, Player.Instance.transform.position, Quaternion.identity);
                Destroy(speedEffect, 2f);
                break;
            case ItemEffects.mana:
                Player.Instance.playerMana.RecoveryMana(Mathf.RoundToInt(amount));
                GameObject manaEffect = Instantiate(prefab, Player.Instance.transform.position, Quaternion.identity);
                Destroy(manaEffect, 2f);
                break;
            case ItemEffects.weapon:
                WeaponManager.Instance.EquipWeapon(weaponIndex, damage);
                break;

            case ItemEffects.shield:
                ShieldManager.Instance.EquipShield(shieldIndex);
                break;

            case ItemEffects.armor:
                BodyManager.Instance.EquipSuit(bodyIndex);
                break;
        }
    }
}