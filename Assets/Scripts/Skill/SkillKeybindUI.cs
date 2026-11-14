using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class SkillKeybindUI : MonoBehaviour
{
    public List<TextMeshProUGUI> skillKeyTexts;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        RefreshKeybinds();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshKeybinds();
    }

    public void RefreshKeybinds()
    {
        if (InputManager.Instance == null) return;

        for (int i = 0; i < skillKeyTexts.Count; i++)
        {
            if (i < InputManager.Instance.skillKeys.Count && skillKeyTexts[i] != null)
                skillKeyTexts[i].text = GetKeyName(InputManager.Instance.skillKeys[i]);
        }
    }

    private string GetKeyName(KeyCode key)
    {
        return key switch
        {
            KeyCode.Alpha1 => "1",
            KeyCode.Alpha2 => "2",
            KeyCode.Alpha3 => "3",
            KeyCode.Alpha4 => "4",
            KeyCode.Alpha5 => "5",
            KeyCode.Alpha6 => "6",
            KeyCode.Alpha7 => "7",
            KeyCode.Alpha8 => "8",
            KeyCode.Alpha9 => "9",
            KeyCode.Alpha0 => "0",
            _ => key.ToString()
        };
    }
}