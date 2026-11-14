using UnityEngine;

public class BodyManager : MonoBehaviour
{
    public static BodyManager Instance;

    [Header("Suits - mesma ordem do bodyIndex no ItemData")]
    public GameObject[] suits; // Body01 a Body20

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void EquipSuit(int index)
    {
        DeactivateAll();
        if (index >= 0 && index < suits.Length)
        {
            suits[index].SetActive(true);
            Debug.Log("Traje equipado: " + suits[index].name);
        }
    }

    public void UnequipAll() => DeactivateAll();

    private void DeactivateAll()
    {
        foreach (var s in suits)
            if (s != null) s.SetActive(false);
    }
}