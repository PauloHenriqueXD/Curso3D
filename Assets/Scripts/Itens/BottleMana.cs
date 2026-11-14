using UnityEngine;

public class BottleMana : MonoBehaviour, IInventoryPickup
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

    public void Collect()
    {
        GameObject effect = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
        Destroy(effect, 3f);
        Inventory.Instance.AddItem(itemData, count);
        AudioManager.Instance.PlaySFX(collectSfx);
        Destroy(gameObject);
    }
}