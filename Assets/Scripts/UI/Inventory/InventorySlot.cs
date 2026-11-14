using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public ItemData itemData;
    public Button slotBt;
    public GameObject tooltip;
    public TextMeshProUGUI tooltipTitleText;
    public TextMeshProUGUI tooltipDescText;
    public TextMeshProUGUI tooltipEffectText;
    public TextMeshProUGUI tooltipPriceText;
    public TextMeshProUGUI countText;
    public Image itemIcon;
    private float lastClickTime;
    private float doubleClickThreshold = 0.3f;
    private int count = 0;

    [Header("Selection")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    private static InventorySlot _selectedSlot;
    public int Count => count;
    public static InventorySlot SelectedSlot => _selectedSlot;

    private void OnEnable()
    {
        UpdateSlotUI();
    }

    public void ResetSlot()
    {
        itemData = null;
        count = 0;
        UpdateSlotUI();
    }

    public void AddItem(ItemData item)
    {
        AddItem(item, 1);
    }

    public void AddItem(ItemData item, int amount)
    {
        itemData = item;
        count += amount;
        UpdateSlotUI();
    }

    private void UpdateSlotUI()
    {
        if (itemData != null && count > 0)
        {
            itemIcon.sprite = itemData.icon;
            itemIcon.color = (_selectedSlot == this) ? selectedColor : normalColor;
            countText.gameObject.SetActive(count > 1);
            countText.text = count.ToString();
            SetHUDText(itemData.effect, count.ToString(), 1f);
        }
        else
        {
            itemIcon.sprite = null;
            itemIcon.color = Color.clear;
            countText.gameObject.SetActive(false);
        }
    }

    public void UseItem()
    {
        if (itemData == null) return;

        // equipáveis — duplo clique
        if (itemData.allowedSlot != ItemData.EquipmentSlot.None)
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            lastClickTime = Time.time;

            if (timeSinceLastClick <= doubleClickThreshold)
            {
                tooltip.SetActive(false);
                bool ok = Player.Instance.equipmentManager.TryEquip(itemData, out string reason);
                if (ok)
                {
                    ItemData toEquip = itemData;
                    ResetSlot();
                    toEquip.Apply();
                    // Debug.Log($"Item equipado: {toEquip.itemName}");
                }
                else
                {
                    Debug.Log(reason);
                }
            }
            return;
        }

        // consumíveis — clique esquerdo só seleciona/move (sem mudança)
        SelectForMove();
    }

    public void UseConsumable()
    {
        if (itemData == null) return;
        if (itemData.allowedSlot != ItemData.EquipmentSlot.None) return;

        tooltip.SetActive(false);

        PlayerSoundManager.Instance.PlayUseBottleSfx();

        var effect = itemData.effect;
        itemData.Apply();

        count--;

        if (count <= 0)
        {
            itemData = null;
            count = 0;
            ZeroHUDText(effect);
        }

        UpdateSlotUI();
    }

    // Chamado pelo slot de equipamento da HUD para desequipar.
    public void UnequipItem()
    {
        if (itemData == null) return;

        Inventory.Instance.RemoveApplyEffect();
        tooltip.SetActive(false);
        Player.Instance.equipmentManager.Unequip(itemData.allowedSlot); // ← aqui
    }

    private void ApplyItem()
    {
        if (itemData != null)
        {
            var effect = itemData.effect;
            itemData.Apply();

            if (effect != ItemData.ItemEffects.weapon)
            {
                count--;

                if (count <= 0)
                {
                    itemData = null;
                    count = 0;
                    ZeroHUDText(effect);
                }

                UpdateSlotUI();
                Inventory.Instance.ActivateInventory(false);
            }
        }
    }

    private void ZeroHUDText(ItemData.ItemEffects effect)
    {
        SetHUDText(effect, "0", 0.35f);
    }

    public void UseItemDirect()
    {
        if (itemData != null)
        {
            itemData.Apply();
            count--;

            if (count <= 0)
            {
                var effect = itemData.effect;
                itemData = null;
                count = 0;
                ZeroHUDText(effect);
            }

            UpdateSlotUI();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(itemData != null)
        {
            tooltip.SetActive(true);
            tooltipTitleText.text = itemData.itemName;
            tooltipDescText.text = itemData.desc;
            //tooltipPriceText.text = $"Price: {itemData.price}c";

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
            else if (itemData.effect == ItemData.ItemEffects.armor)
            {
                tooltipEffectText.text = $"Effect: {itemData.defense}";
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
        if (itemData != null)
        {
            tooltip.SetActive(false);
        }
    }

    private void SetHUDText(ItemData.ItemEffects effect, string value, float alpha)
    {
        TextMeshProUGUI target = effect switch
        {
            ItemData.ItemEffects.health => Player.Instance.playerHUD.hpText,
            ItemData.ItemEffects.mana => Player.Instance.playerHUD.mpText,
            ItemData.ItemEffects.speed => Player.Instance.playerHUD.speedText,
            _ => null
        };

        if (target == null) return;
        target.text = value;
        var c = target.color;
        c.a = alpha;
        target.color = c;
    }

    private bool _justSelected = false;
    public void SelectForMove()
    {
        // slot vazio só aceita receber um item
        if (itemData == null)
        {
            if (_selectedSlot != null)
                SwapItems(_selectedSlot, this);
            return;
        }

        // clicou no mesmo slot — cancela
        if (_selectedSlot == this)
        {
            if (_justSelected)
            {
                _justSelected = false;
                return; // ignora o segundo disparo
            }
            Deselect();
            return;
        }

        // já tem selecionado — troca
        if (_selectedSlot != null)
        {
            SwapItems(_selectedSlot, this);
            return;
        }

        // seleciona este slot
        _selectedSlot = this;
        _justSelected = true;
        itemIcon.color = selectedColor;
    }

    private static void SwapItems(InventorySlot from, InventorySlot to)
    {
        ItemData tempData = to.itemData;
        int tempCount = to.count;

        to.itemData = from.itemData;
        to.count = from.count;

        from.itemData = tempData;
        from.count = tempCount;

        from.UpdateSlotUI();
        to.UpdateSlotUI();

        Deselect();
    }

    private static void Deselect()
    {
        if (_selectedSlot != null)
        {
            _selectedSlot.UpdateSlotUI();
            _selectedSlot = null;
        }
    }

    public static void CancelSelection()
    {
        Deselect();
    }

    public void DiscardItems(int amount)
    {
        if (itemData == null) return;

        var effect = itemData.effect;
        count -= amount;

        if (count <= 0)
        {
            itemData = null;
            count = 0;
            ZeroHUDText(effect);
        }

        UpdateSlotUI();
        CancelSelection();
    }
}