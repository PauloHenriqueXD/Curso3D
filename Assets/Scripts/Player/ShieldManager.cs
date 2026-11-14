using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    public static ShieldManager Instance;

    [Header("Shields - mesma ordem do shieldIndex no ItemData")]
    public GameObject[] shields;
    // 0  = Shield01
    // 1  = Shield02
    // 2  = Shield03
    // 3  = Shield04
    // 4  = Shield05
    // 5  = Shield06
    // 6  = Shield07
    // 7  = Shield08
    // 8  = Shield09
    // 9  = Shield10
    // 10 = Shield11
    // 11 = Shield12
    // 12 = Shield13
    // 13 = Shield14
    // 14 = Shield15
    // 15 = Shield16
    // 16 = Shield17
    // 17 = Shield18
    // 18 = Shield19
    // 19 = Shield20

    private int equippedIndex = -1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void EquipShield(int index)
    {
        DeactivateAll();
        if (index >= 0 && index < shields.Length)
        {
            shields[index].SetActive(true);
            equippedIndex = index;
            Debug.Log("Escudo equipado: " + shields[index].name);
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
        foreach (var s in shields)
            if (s != null) s.SetActive(false);
    }
}