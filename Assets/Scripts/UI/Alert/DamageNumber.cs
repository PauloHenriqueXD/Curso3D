using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float moveSpeed = 2f;
    public float lifetime = 1f;
    private float timer;

    public void Setup(int damage, Color color)
    {
        text.text = damage.ToString();
        text.color = color;
        timer = lifetime;
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        transform.forward = Camera.main.transform.forward;

        timer -= Time.deltaTime;
        if (timer <= 0f)
            Destroy(gameObject);
    }
}