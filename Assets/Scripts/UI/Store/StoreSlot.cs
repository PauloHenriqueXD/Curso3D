using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class StoreSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public ItemData itemData;
    public Button slotBt;
    public GameObject tooltip;
    public TextMeshProUGUI tooltipTitleText;
    public TextMeshProUGUI tooltipDescText;
    public TextMeshProUGUI tooltipEffectText;
    public TextMeshProUGUI priceText;

    private void OnEnable()
    {
        UpdateSlotUI();
    }

    public void SetItem(ItemData item)
    {
        itemData = item;
        UpdateSlotUI();
    }

    private void UpdateSlotUI()
    {
        if (itemData != null)
        {
            slotBt.gameObject.SetActive(true);
            slotBt.GetComponent<Image>().sprite = itemData.icon;
            priceText.gameObject.SetActive(true);
            priceText.text = $"Price: {itemData.price}c";
        }
        else
        {
            slotBt.gameObject.SetActive(false);
            priceText.gameObject.SetActive(false);
        }
    }

    public void OnBuyClick()
    {
        GetComponentInParent<StoreUI>().SelectItem(itemData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData != null)
        {
            tooltip.SetActive(true);
            tooltipTitleText.text = itemData.itemName;
            tooltipDescText.text = itemData.desc;

            if (itemData.effect == ItemData.ItemEffects.weapon)
            {
                tooltipEffectText.text = $"Damage: {itemData.damage}";
                tooltipEffectText.gameObject.SetActive(true);
            }
            else if (itemData.effect == ItemData.ItemEffects.shield)
            {
                tooltipEffectText.text = $"Defense: {itemData.defense}";
                tooltipEffectText.gameObject.SetActive(true);
            }
            else
            {
                tooltipEffectText.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }
}