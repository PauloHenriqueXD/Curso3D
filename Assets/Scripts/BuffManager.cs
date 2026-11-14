using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance;
    public Transform buffContainer; // onde os ícones vão aparecer na HUD
    public GameObject buffIconPrefab; // prefab do ícone de buff

    private Dictionary<string, BuffIcon> activeBuffs = new Dictionary<string, BuffIcon>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddBuff(string buffId, Sprite icon, float duration)
    {
        //Debug.Log($"[BuffManager] AddBuff chamado: {buffId}, container={buffContainer}, prefab={buffIconPrefab}");

        if (activeBuffs.ContainsKey(buffId))
        {
            activeBuffs[buffId].Refresh(duration);
            return;
        }

        GameObject obj = Instantiate(buffIconPrefab, buffContainer);
        BuffIcon buffIcon = obj.GetComponent<BuffIcon>();
        buffIcon.Setup(buffId, icon, duration, OnBuffExpired);
        activeBuffs.Add(buffId, buffIcon);
    }

    private void OnBuffExpired(string buffId)
    {
        if (activeBuffs.ContainsKey(buffId))
        {
            activeBuffs.Remove(buffId);
        }
    }
}