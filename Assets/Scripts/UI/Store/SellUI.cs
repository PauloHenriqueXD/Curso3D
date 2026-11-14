using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SellUI : MonoBehaviour
{
    public GameObject container;
    public Button confirmBt;
    public TextMeshProUGUI confirmPriceText;
    public GameObject confirmWindow;

    private List<SellSlot> slots = new List<SellSlot>();
    private SellSlot selectedSlot;

    private void Awake()
    {
        foreach (SellSlot slot in container.GetComponentsInChildren<SellSlot>())
            slots.Add(slot);
    }

    private void OnEnable()
    {
        PopulateSellSlots();
    }

    private void PopulateSellSlots()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogWarning("SellUI: Inventory.Instance ainda não estava pronto. Abortando população.");
            return;
        }

        List<InventorySlot> playerSlots = Inventory.Instance.GetSlots();
        int sellIndex = 0;

        for (int i = 0; i < playerSlots.Count; i++)
        {
            if (playerSlots[i].itemData != null)
            {
                if (sellIndex < slots.Count)
                {
                    slots[sellIndex].SetItem(playerSlots[i]);
                    sellIndex++;
                }
            }
        }

        // limpa o resto dos slots da UI de venda
        for (int i = sellIndex; i < slots.Count; i++)
            slots[i].SetItem(null);
    }

    public void SelectItem(SellSlot slot)
    {
        selectedSlot = slot;
        confirmPriceText.text = "Sell " + slot.itemData.itemName + " for " + slot.itemData.sellPrice.ToString() + "c?";
        confirmWindow.SetActive(true);
        confirmBt.onClick.RemoveAllListeners();
        confirmBt.onClick.AddListener(ConfirmSell);
    }

    public void CancelSell()
    {
        confirmWindow.SetActive(false);
        selectedSlot = null;
    }

    public void ConfirmSell()
    {
        if (selectedSlot == null || selectedSlot.sourceSlot == null) return;

        ItemData soldItem = selectedSlot.itemData;

        selectedSlot.sourceSlot.DiscardItems(1); // remove 1 unidade do inventário real
        Player.Instance.coinScore += soldItem.sellPrice;
        Player.Instance.playerHUD.UpdateCoinText();

        Debug.Log("Sold: " + soldItem.itemName);

        confirmWindow.SetActive(false);
        selectedSlot = null;

        PopulateSellSlots(); // re-sincroniza (item pode ter zerado ou só diminuído a pilha)
    }
}