using System.Collections.Generic;
using UnityEngine;

public class KeybindMenuUI : MonoBehaviour
{
    [Header("Referências")]
    public GameObject rowPrefab; // o BgInput, transformado em Prefab
    public Transform content;    // o "Content" do Scroll View
    public GameObject keybindScrollView; // o objeto "Scroll View" atual (lista de keybinds)
    public GameObject soundsPanel;       // o painel/objeto que tem os sliders de Music/Sounds (o antigo Bg_Sounds, ou só o Content dele)

    private List<KeybindEntry> allEntries;
    private KeybindCategory currentCategory = KeybindCategory.Basic;

    private void Start()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogError("InputManager.Instance é nulo. Verifique a ordem de carregamento da cena.");
            return;
        }

        allEntries = InputManager.Instance.GetKeybindEntries();
        ShowCategory(KeybindCategory.Basic);
    }

    public void ShowCategory(KeybindCategory category)
    {
        // tenta recuperar caso o Start tenha rodado cedo demais
        if (allEntries == null)
        {
            if (InputManager.Instance == null)
            {
                Debug.LogWarning("InputManager ainda não está pronto. Abortando troca de categoria.");
                return;
            }
            allEntries = InputManager.Instance.GetKeybindEntries();
        }

        currentCategory = category;

        bool isSounds = category == KeybindCategory.Sounds;

        if (keybindScrollView != null) keybindScrollView.SetActive(!isSounds);
        if (soundsPanel != null) soundsPanel.SetActive(isSounds);

        if (isSounds) return;

        foreach (Transform child in content)
            Destroy(child.gameObject);

        foreach (var entry in allEntries)
        {
            if (entry.Category != category) continue;

            GameObject rowObj = Instantiate(rowPrefab, content);
            rowObj.SetActive(true);
            rowObj.GetComponent<KeybindRow>().Setup(entry);
        }
    }

    public void ResetAllToDefault()
    {
        InputManager.Instance.ResetToDefaults(); // já salva via PlayerPrefs internamente
        allEntries = InputManager.Instance.GetKeybindEntries(); // recarrega os getters/setters
        ShowCategory(currentCategory); // re-renderiza a categoria aberta com os valores resetados
    }

    public void ResetCategoryToDefault(KeybindCategory category)
    {
        // Cria uma instância temporária só pra ler os valores default (sem afetar a real)
        var temp = new GameObject().AddComponent<InputManager>();
        var defaultEntries = temp.GetKeybindEntries();
        Destroy(temp.gameObject);

        foreach (var entry in allEntries)
        {
            if (entry.Category != category) continue;
            var defaultEntry = defaultEntries.Find(d => d.DisplayName == entry.DisplayName);
            if (defaultEntry != null)
                entry.Setter(defaultEntry.Getter());
        }

        InputManager.Instance.SaveKeybinds();
        ShowCategory(category);
    }

    // Liga esses métodos no OnClick() de cada botão de categoria
    public void ShowBasic() => ShowCategory(KeybindCategory.Basic);
    public void ShowAdvanced() => ShowCategory(KeybindCategory.Advanced);
    public void ShowSkills() => ShowCategory(KeybindCategory.Skills);
    public void ShowInterface() => ShowCategory(KeybindCategory.Interface);
    public void ShowOthers() => ShowCategory(KeybindCategory.Others);
    public void ShowSounds() => ShowCategory(KeybindCategory.Sounds);
}