using UnityEngine;
using UnityEngine.UI;

public class AutoAttackButtonUI : MonoBehaviour
{
    [Header("Referências")]
    public Image icon;
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.white;

    private bool lastState;

    // ligar no OnClick() do botão
    public void OnClick()
    {
        if (Player.Instance == null || Player.Instance.skills == null) return;
        Player.Instance.skills.ToggleAutoAttack();
    }

    private void Update()
    {
        if (Player.Instance == null || Player.Instance.skills == null) return;

        bool current = Player.Instance.skills.AutoAttackEnabled;
        if (current != lastState)
        {
            lastState = current;
            icon.color = current ? activeColor : inactiveColor;
        }
    }
}