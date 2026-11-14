using UnityEngine;

public class DamageNumberSpawner : MonoBehaviour
{
    public static DamageNumberSpawner Instance;
    public GameObject damageNumberPrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Spawn(int damage, Vector3 position, Color color)
    {
        GameObject go = Instantiate(damageNumberPrefab, position + Vector3.up * 1.5f, Quaternion.identity);
        go.GetComponent<DamageNumber>().Setup(damage, color);
    }
}