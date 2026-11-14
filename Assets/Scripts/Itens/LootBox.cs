using UnityEngine;

public class LootBox : MonoBehaviour
{
    public GameObject idleBox;
    public GameObject destroyBox;
    public Animator anim;
    public Collider coll;
    public GameObject[] loots;
    public Transform spawnPoint;
    public AudioClip destroyBoxSfx;
    public AudioClip spawnItemSfx;

    public void DestroyBox()
    {
        AudioManager.Instance.PlaySFX(destroyBoxSfx);
        AudioManager.Instance.PlaySFX(spawnItemSfx);
        coll.enabled = false;
        Instantiate(loots[Random.Range(0, loots.Length)], spawnPoint.position, Quaternion.identity); // Instantiate a random loot from the array at the position of the loot box
        idleBox.SetActive(false);
        destroyBox.SetActive(true);
        anim.Play("Bang");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<SwordHitbox>())
        {
            DestroyBox();
        }
    }
}
