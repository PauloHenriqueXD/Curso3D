using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI inventoryCoinText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public TextMeshProUGUI speedText;
    public GameObject canvas;
    public GameObject inventoryCanvas;
    public GameObject statusCanvas;
    public GameObject panelMenu;
    public GameObject bgKeybinds;
    public GameObject PanelOptions;
    public GameObject Bg_sounds;

    [Header("Enemy Target HUD")]
    public GameObject enemyInfoPanel;
    public TextMeshProUGUI enemyNameText;
    public Image enemyHpFill;

    public void ActiveCanvas(bool isActive)
    {
        canvas.SetActive(isActive);
    }

    public void ActivateInventory(bool isActive)
    {
        inventoryCanvas.SetActive(isActive);
    }
    public void ToggleInventory()
    {
        bool isActive = !inventoryCanvas.activeSelf;
        inventoryCanvas.SetActive(isActive);
        Player.Instance.isPaused = isActive;
    }

    public void CloseInventory()
    {
        if (inventoryCanvas.activeSelf)
        {
            inventoryCanvas.SetActive(false);
            Player.Instance.isPaused = false;
        }
    }

    public void ToggleKeybinds()
    {
        bool isActive = !bgKeybinds.activeSelf;
        bgKeybinds.SetActive(isActive);
        Player.Instance.isPaused = isActive;
    }

    public void CloseKeybinds()
    {
        if (bgKeybinds.activeSelf)
        {
            bgKeybinds.SetActive(false);
            Player.Instance.isPaused = false;
        }
    }

    public void ToggleMenu()
    {
        bool isActive = !panelMenu.activeSelf;
        panelMenu.SetActive(isActive);
        Player.Instance.isPaused = isActive;
    }

    public void CloseMenu()
    {
        if (panelMenu.activeSelf)
        {
            panelMenu.SetActive(false);
            Player.Instance.isPaused = false;
        }
    }

    public void ToggleOptions()
    {
        bool isActive = !PanelOptions.activeSelf;
        PanelOptions.SetActive(isActive);
        Player.Instance.isPaused = isActive;
    }

    public void CloseOptions()
    {
        if (PanelOptions.activeSelf)
        {
            PanelOptions.SetActive(false);
            Player.Instance.isPaused = false;
        }
    }

    public void ToggleStatus()
    {
        bool isActive = !statusCanvas.activeSelf;
        statusCanvas.SetActive(isActive);
        Player.Instance.isPaused = isActive;
    }

    public void CloseStatus()
    {
        if (statusCanvas.activeSelf)
        {
            statusCanvas.SetActive(false);
            Player.Instance.isPaused = false;
        }
    }

    public void ToggleSounds()
    {
        bool isActive = !Bg_sounds.activeSelf;
        Bg_sounds.SetActive(isActive);
        Player.Instance.isPaused = isActive;
    }

    public void CloseSounds()
    {
        if (Bg_sounds.activeSelf)
        {
            Bg_sounds.SetActive(false);
            Player.Instance.isPaused = false;
        }
    }

    public void UpdateCoinText()
    {
        string value = Player.Instance.coinScore.ToString();
        coinText.text = value;
        if (inventoryCoinText != null)
            inventoryCoinText.text = value;
    }

    public void ShowEnemyInfo(string enemyName)
    {
        enemyInfoPanel.SetActive(true);
        enemyNameText.text = enemyName;
    }

    public void HideEnemyInfo()
    {
        enemyInfoPanel.SetActive(false);
    }

    public void UpdateEnemyHP(int currentLife, int maxLife)
    {
        if (enemyHpFill == null) return;
        enemyHpFill.fillAmount = maxLife > 0 ? (float)currentLife / maxLife : 0f;
    }
}
