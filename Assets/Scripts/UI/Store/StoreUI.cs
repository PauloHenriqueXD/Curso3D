using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreUI : MonoBehaviour
{
    public GameObject container;
    public Button confirmBt;
    public TextMeshProUGUI confirmPriceText;
    public GameObject confirmWindow;

    [Header("Quantidade (opcional — só a loja de poções usa)")]
    public TextMeshProUGUI qtdText;
    public TextMeshProUGUI costText;

    public List<ItemData> itemsForSale;

    private List<StoreSlot> slots = new List<StoreSlot>();
    private ItemData selectedItem;
    private int selectedQuantity = 1;
    private bool HasQuantityUI => qtdText != null && costText != null;

    private void Awake()
    {
        foreach (StoreSlot slot in container.GetComponentsInChildren<StoreSlot>())
            slots.Add(slot);
    }

    private void OnEnable()
    {
        PopulateStore();
    }

    private void PopulateStore()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < itemsForSale.Count)
                slots[i].SetItem(itemsForSale[i]);
            else
                slots[i].SetItem(null);
        }
    }

    public void SelectItem(ItemData item)
    {
        selectedItem = item;
        selectedQuantity = 1;

        if (HasQuantityUI)
        {
            confirmPriceText.text = "Comprar " + item.itemName + "?";
            UpdateQuantityUI();
        }
        else
        {
            confirmPriceText.text = "Comprar " + item.itemName + " por " + item.price + "c?";
        }

        confirmWindow.SetActive(true);
        confirmBt.onClick.RemoveAllListeners();
        confirmBt.onClick.AddListener(ConfirmPurchase);
    }

    public void IncreaseQuantity()
    {
        if (selectedItem == null || !HasQuantityUI) return;
        selectedQuantity++;
        UpdateQuantityUI();
    }

    public void DecreaseQuantity()
    {
        if (selectedItem == null || !HasQuantityUI) return;
        if (selectedQuantity > 1)
            selectedQuantity--;
        UpdateQuantityUI();
    }

    private void UpdateQuantityUI()
    {
        qtdText.text = selectedQuantity.ToString();
        costText.text = (selectedItem.price * selectedQuantity).ToString();
    }

    public void CancelPurchase()
    {
        confirmWindow.SetActive(false);
        selectedItem = null;
        selectedQuantity = 1;
    }

    public void ConfirmPurchase()
    {
        if (selectedItem == null) return;

        int totalCost = selectedItem.price * selectedQuantity;

        if (!Inventory.Instance.CanAdd(selectedItem, selectedQuantity))
        {
            Debug.Log("Inventário cheio!"); // não cobra se o item não cabe
        }
        else if (Player.Instance.coinScore >= totalCost)
        {
            Player.Instance.coinScore -= totalCost;
            Player.Instance.playerHUD.UpdateCoinText();

            for (int i = 0; i < selectedQuantity; i++)
                Inventory.Instance.AddItem(selectedItem);

            Debug.Log("Comprou " + selectedQuantity + "x: " + selectedItem.itemName);
        }
        else
        {
            Debug.Log("Coins insuficientes!");
        }

        confirmWindow.SetActive(false);
        selectedItem = null;
        selectedQuantity = 1;
    }
}