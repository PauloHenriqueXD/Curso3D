using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrashDropZone : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler
{
    public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (InventorySlot.SelectedSlot == null || InventorySlot.SelectedSlot.itemData == null)
            return;

        // abre confirmação de descarte
        Inventory.Instance.OpenDiscardFromTrash(InventorySlot.SelectedSlot);
    }
}