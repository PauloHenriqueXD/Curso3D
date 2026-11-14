using UnityEngine;

public class Coin : MonoBehaviour
{
    public GameObject collectEffectPrefab;
    public AudioClip collectSfx;

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
        GameManagement.Instance.AddCoin();
        AudioManager.Instance.PlaySFX(collectSfx);
        Destroy(gameObject);
    }
}