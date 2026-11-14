using UnityEngine;

public class BottleHealth : MonoBehaviour, IInventoryPickup
{
    public GameObject collectEffectPrefab;
    [field: SerializeField] public ItemData itemData { get; set; }
    public AudioClip collectSfx;
    [field: SerializeField] public int count { get; set; } = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private bool collected; // evita coletar duas vezes no mesmo frame (trigger + pulo)

    public void Collect()
    {
        if (collected) return;
        collected = true;
        GameObject effect = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
        Destroy(effect, 3f);
        Inventory.Instance.AddItem(itemData, count);
        AudioManager.Instance.PlaySFX(collectSfx);
        Destroy(gameObject);
    }
}