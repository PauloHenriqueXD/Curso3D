using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    [Header("Weapons - mesma ordem do weaponIndex no ItemData")]
    public GameObject[] weapons;
    // 0  = OHS01_Stick
    // 1  = OHS02_Niddle
    // 2  = OHS03_Sword
    // 3  = OHS04_Sword
    // 4  = OHS05_Sword
    // 5  = OHS06_Sword
    // 6  = OHS07_Sword
    // 7  = OHS08_Sword
    // 8  = OHS09_Sword
    // 9  = OHS10_Axe
    // 10 = OHS11_Hammer
    // 11 = OHS12_Sword
    // 12 = OHS13_Axe
    // 13 = OHS14_Hammer
    // 14 = OHS15_Sword
    // 15 = OHS16_Sword
    // 16 = THS01_Sword
    // 17 = THS02_Sword
    // 18 = THS03_Sword
    // 19 = THS04_Sword
    // 20 = THS05_Sword
    // 21 = THS06_Sword
    // 22 = THS07_Sword

    private int equippedIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Garante que todas as armas comecem desativadas
        //DeactivateAll();
    }

    public void EquipWeapon(int index, int damage = 0)
    {
        DeactivateAll();
        if (index >= 0 && index < weapons.Length)
        {
            weapons[index].SetActive(true);
            equippedIndex = index;
            Debug.Log("Arma equipada: " + weapons[index].name);

            // injeta o dano do ItemData no SwordHitbox da arma
            SwordHitbox hitbox = weapons[index].GetComponentInChildren<SwordHitbox>();
            if (hitbox != null)
                hitbox.damage = damage;
        }
    }

    public void UnequipAll()
    {
        DeactivateAll();
        equippedIndex = -1;
    }

    public int GetEquippedIndex() => equippedIndex;

    private void DeactivateAll()
    {
        foreach (var w in weapons)
            if (w != null) w.SetActive(false);
    }
}