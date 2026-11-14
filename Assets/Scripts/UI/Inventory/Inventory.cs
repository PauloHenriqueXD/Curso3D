using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Button confirmBt;
    public Button yesBt;
    public Button noBt;
    public GameObject container;
    public static Inventory Instance;
    public GameObject confirmWindow;
    public TextMeshProUGUI confirmTitle;
    public GameObject inventoryCanvas;
    private List<InventorySlot> slots = new List<InventorySlot>();
    public List<InventorySlot> GetSlots() => slots;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            slots.Clear();

            foreach (InventorySlot slot in container.GetComponentsInChildren<InventorySlot>())
            {
                slots.Add(slot);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // o inventário vive no Player (DontDestroyOnLoad); só reseta ao começar um jogo novo,
        // senão entrar/sair de uma casa (cena House) apagava todos os itens
        if (scene.name != "main_menu" && scene.name != "customization") return;

        foreach (InventorySlot slot in slots)
        {
            if (slot != null)
                slot.ResetSlot();
        }
    }

    public void AddItem(ItemData item)
    {
        AddItem(item, 1);
    }

    public void AddItem(ItemData item, int amount)
    {
        // equipamentos (armas, escudos, trajes) nunca acumulam — vão direto para slot vazio
        if (IsStackable(item))
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot.itemData == item)
                {
                    slot.AddItem(item, amount);
                    return;
                }
            }
        }

        foreach (InventorySlot slot in slots)
        {
            if (slot.itemData == null)
            {
                slot.AddItem(item, amount);
                return;
            }
        }

        Debug.Log("Inventário cheio!");
    }

    private static bool IsStackable(ItemData item) =>
        item.effect != ItemData.ItemEffects.weapon && item.allowedSlot == ItemData.EquipmentSlot.None;

    // verifica se cabem "amount" unidades do item (usado pela loja antes de cobrar)
    public bool CanAdd(ItemData item, int amount)
    {
        int emptySlots = 0;
        foreach (InventorySlot slot in slots)
        {
            if (IsStackable(item) && slot.itemData == item) return true;
            if (slot.itemData == null) emptySlots++;
        }
        return IsStackable(item) ? emptySlots > 0 : emptySlots >= amount;
    }

    public void UsePotion(ItemData.ItemEffects effect)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.itemData != null && slot.itemData.effect == effect)
            {
                slot.UseItemDirect();
                return;
            }
        }
        Debug.Log("Nenhuma poção do tipo " + effect + " no inventário!");
    }

    public bool HasPotion(ItemData.ItemEffects effect)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.itemData != null && slot.itemData.effect == effect)
                return true;
        }
        return false;
    }

    public void ActivateInventory(bool isActive)
    {
        inventoryCanvas.SetActive(isActive);
    }

    public void RemoveApplyEffect()
    {
        confirmBt.onClick.RemoveAllListeners();
        yesBt.onClick.RemoveAllListeners();
        noBt.onClick.RemoveAllListeners();
    }

    private InventorySlot slotToDiscard;
    private void CancelDiscard()
    {
        CloseDiscardWindow();
    }

    private void CloseDiscardWindow()
    {
        confirmTitle.text = "Use this item Now?";
        RemoveApplyEffect();
        confirmWindow.SetActive(false);
        slotToDiscard = null;
        InventorySlot.CancelSelection();
    }

    public void OpenDiscardFromTrash(InventorySlot slot)
    {
        slotToDiscard = slot;

        confirmTitle.text = $"Descartar {slot.itemData.itemName}?";

        RemoveApplyEffect();
        yesBt.onClick.AddListener(ConfirmDiscard);
        noBt.onClick.AddListener(CancelDiscard);

        confirmWindow.SetActive(true);
    }

    private void ConfirmDiscard()
    {
        if (slotToDiscard != null && slotToDiscard.itemData != null)
            slotToDiscard.DiscardItems(slotToDiscard.Count); // descarta tudo

        CloseDiscardWindow();
    }

    public void DropAllItems(Vector3 dropPosition)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.itemData == null || slot.Count <= 0) continue;

            ItemData item = slot.itemData;
            int amount = slot.Count;

            if (item.worldPickupPrefab != null)
            {
                Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0.3f, Random.Range(-1f, 1f));
                GameObject dropped = Instantiate(item.worldPickupPrefab, dropPosition + offset, Quaternion.identity);

                if (dropped.TryGetComponent<IInventoryPickup>(out var pickup))
                {
                    pickup.itemData = item;
                    pickup.count = amount;
                    Debug.Log($"[DROP] {item.itemName} dropado com count={pickup.count}");
                }
                else
                {
                    Debug.LogWarning($"O prefab de {item.itemName} não implementa IInventoryPickup!");
                }
            }
            else
            {
                Debug.LogWarning($"Item {item.itemName} não tem worldPickupPrefab configurado, não foi dropado.");
            }

            slot.DiscardItems(amount);
        }
    }
}
