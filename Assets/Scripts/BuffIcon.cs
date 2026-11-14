using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour
{
    public Image icon;
    public Image timerBar;

    private string buffId;
    private float duration;
    private Action<string> onExpired;
    private Coroutine timerCoroutine;

    public void Setup(string id, Sprite sprite, float dur, Action<string> callback)
    {
        if (icon == null || timerBar == null)
        {
            Debug.LogError($"[BuffIcon] Referência faltando! icon={icon}, timerBar={timerBar}");
            return;
        }

        buffId = id;
        icon.sprite = sprite;
        icon.color = Color.white; // garante visibilidade mesmo se alguém deixou alpha 0 no prefab
        duration = dur;
        onExpired = callback;

        //Debug.Log($"[BuffIcon] Setup chamado: id={id}, sprite={(sprite != null ? sprite.name : "NULL")}, duration={dur}");

        timerCoroutine = StartCoroutine(TimerCoroutine(duration));
    }

    public void Refresh(float newDuration)
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        duration = newDuration;
        timerCoroutine = StartCoroutine(TimerCoroutine(duration));
        //Debug.Log($"[BuffIcon] Refresh chamado: id={buffId}, newDuration={newDuration}");
    }

    IEnumerator TimerCoroutine(float dur)
    {
        float elapsed = 0f;

        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            timerBar.fillAmount = 1f - (elapsed / dur);
            yield return null;
        }

        onExpired?.Invoke(buffId);
        Destroy(gameObject);
    }
}