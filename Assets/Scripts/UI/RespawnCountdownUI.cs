using System.Collections;
using TMPro;
using UnityEngine;

public class RespawnCountdownUI : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float respawnTime = 5f;

    private Coroutine countdownRoutine;

    public void StartCountdown(System.Action onRespawn)
    {
        if (countdownRoutine != null)
            StopCoroutine(countdownRoutine);

        countdownRoutine = StartCoroutine(CountdownRoutine(onRespawn));
    }

    private IEnumerator CountdownRoutine(System.Action onRespawn)
    {
        float timer = respawnTime;

        while (timer > 0f)
        {
            countdownText.text = Mathf.CeilToInt(timer).ToString();
            yield return null;
            timer -= Time.deltaTime;
        }

        countdownText.text = "0";
        onRespawn?.Invoke();
    }
}