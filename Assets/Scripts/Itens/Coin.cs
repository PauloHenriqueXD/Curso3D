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

    public void Collect()
    {
        GameObject effect = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
        Destroy(effect, 3f);
        GameManagement.Instance.AddCoin();
        AudioManager.Instance.PlaySFX(collectSfx);
        Destroy(gameObject);
    }
}