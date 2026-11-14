using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeybindRow : MonoBehaviour
{
    [Header("Referências (arraste no Inspector)")]
    public TMP_Text titleText;       // o objeto "Title"
    public Button inputButton;       // o objeto "Input" (precisa ter um Button component)
    public TMP_Text inputButtonText; // o texto dentro do "Input"

    private KeybindEntry entry;
    private bool isWaitingForKey = false;

    public void Setup(KeybindEntry keybindEntry)
    {
        entry = keybindEntry;
        titleText.text = entry.DisplayName;
        inputButtonText.text = entry.Getter().ToString();

        inputButton.onClick.RemoveAllListeners();
        inputButton.onClick.AddListener(StartRebind);
    }

    private void StartRebind()
    {
        if (isWaitingForKey) return;
        isWaitingForKey = true;
        inputButtonText.text = "...";
    }

    private void Update()
    {
        if (!isWaitingForKey) return;

        foreach (KeyCode kc in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kc))
            {
                entry.Setter(kc);
                inputButtonText.text = kc.ToString();
                isWaitingForKey = false;
                InputManager.Instance.SaveKeybinds();
                break;
            }
        }
    }
}