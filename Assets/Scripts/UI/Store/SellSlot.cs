using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SellSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public InventorySlot sourceSlot; // slot real do inventário do player
    [HideInInspector] public ItemData itemData;

    public Button slotBt;
    public Image itemIcon;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI sellPriceText;

    public GameObject tooltip;
    public TextMeshProUGUI tooltipTitleText;
    public TextMeshProUGUI tooltipDescText;
    public TextMeshProUGUI tooltipEffectText;

    public void SetItem(InventorySlot source)
    {
        sourceSlot = source;
        itemData = source != null ? source.itemData : null;
        UpdateSlotUI();
    }

    private void UpdateSlotUI()
    {
        if (itemData != null)
        {
            slotBt.gameObject.SetActive(true);
            itemIcon.sprite = itemData.icon;
            itemIcon.color = Color.white;

            sellPriceText.gameObject.SetActive(true);
            sellPriceText.text = $"Sell: {itemData.sellPrice}c";

            countText.gameObject.SetActive(sourceSlot.Count > 1);
            countText.text = sourceSlot.Count.ToString();
        }
        else
        {
            slotBt.gameObject.SetActive(false);
            itemIcon.sprite = null;
            itemIcon.color = Color.clear;
            countText.gameObject.SetActive(false);
            sellPriceText.gameObject.SetActive(false);
        }
    }

    public void OnSellClick()
    {
        if (itemData == null) return;
        GetComponentInParent<SellUI>().SelectItem(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData == null) return;

        tooltip.SetActive(true);
        tooltipTitleText.text = itemData.itemName;
        tooltipDescText.text = itemData.desc;

        if (itemData.effect == ItemData.ItemEffects.weapon)
        {
            tooltipEffectText.text = $"Damage: {itemData.damage}";
            tooltipEffectText.gameObject.SetActive(true);
        }
        else if (itemData.effect == ItemData.ItemEffects.shield || itemData.effect == ItemData.ItemEffects.armor)
        {
            tooltipEffectText.text = $"Defense: {itemData.defense}";
            tooltipEffectText.gameObject.SetActive(true);
        }
        else
        {
            tooltipEffectText.gameObject.SetActive(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }
}