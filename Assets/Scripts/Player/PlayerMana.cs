using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMana : MonoBehaviour
{
    public int mana;
    public int maxMana;

    [Header("HUD - Barra de Mana")]
    public Image mpBar;              // arraste o objeto "MpBar" aqui
    public TextMeshProUGUI mpText;   // arraste o objeto "MpQtd" aqui (opcional)

    [Header("Skill Buttons")]
    public Button skillButton1;
    public Button skillButton2;
    public Button skillButton3;
    public Button skillButton4;

    [Header("Skill Costs")]
    public int costSkill1;
    public int costSkill2;
    public int costSkill3;
    public int costSkill4;

    [Header("Regeneração")]
    public int regenAmount; // quanto regenera por tick
    public float regenInterval; // a cada quantos segundos

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        UpdateManaBar();
        UpdateSkillButtons();
        StartCoroutine(ManaRegen());
    }

    // chamado ao usar uma skill
    public bool UseMana(int cost)
    {
        if (mana < cost)
        {
            Debug.Log("Mana insuficiente!");
            return false; // skill não executa
        }

        mana -= cost;
        UpdateManaBar();
        UpdateSkillButtons();
        return true; // skill executa
    }

    // recuperar mana (poção, regeneração, etc)
    public void RecoveryMana(int amount)
    {
        mana += amount;
        if (mana >= maxMana) mana = maxMana;
        UpdateManaBar();
        UpdateSkillButtons();
    }

    // atualiza a interatividade dos botões de skill com base na mana atual
    private void UpdateSkillButtons()
    {
        if (skillButton1 != null) skillButton1.interactable = mana >= costSkill1;
        if (skillButton2 != null) skillButton2.interactable = mana >= costSkill2;
        if (skillButton3 != null) skillButton3.interactable = mana >= costSkill3;
        if (skillButton4 != null) skillButton4.interactable = mana >= costSkill4;
    }

    private void UpdateManaBar()
    {
        if (mpBar != null)
            mpBar.fillAmount = maxMana > 0 ? (float)mana / maxMana : 0f;

        if (mpText != null)
            mpText.text = $"{mana}/{maxMana}";
    }

    IEnumerator ManaRegen()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenInterval);

            if (mana < maxMana)
            {
                RecoveryMana(regenAmount);
            }
        }
    }
}