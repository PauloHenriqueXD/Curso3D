using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SkillCooldownUI : MonoBehaviour
{
    public PlayerSkills playerSkills;
    public List<Image> cooldownOverlays;
    public List<TextMeshProUGUI> cooldownTexts; // opcional, pode deixar vazio na lista

    private void Update()
    {
        if (playerSkills == null) return;

        int count = Mathf.Min(cooldownOverlays.Count, playerSkills.skills.Count);

        for (int i = 0; i < count; i++)
        {
            var slot = playerSkills.skills[i];
            var overlay = cooldownOverlays[i];
            if (overlay == null) continue;

            if (slot.IsOnCooldown)
            {
                overlay.gameObject.SetActive(true);
                overlay.fillAmount = slot.CooldownPercent;

                if (i < cooldownTexts.Count && cooldownTexts[i] != null)
                {
                    cooldownTexts[i].gameObject.SetActive(true);
                    cooldownTexts[i].text = slot.cooldownTimer.ToString("F1");
                }
            }
            else
            {
                overlay.gameObject.SetActive(false);

                if (i < cooldownTexts.Count && cooldownTexts[i] != null)
                    cooldownTexts[i].gameObject.SetActive(false);
            }
        }
    }
}