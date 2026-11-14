using UnityEngine;
using UnityEngine.EventSystems;

public class RightClickHandler : MonoBehaviour
{
    private InventorySlot slot;

    private void Awake()
    {
        slot = GetComponentInParent<InventorySlot>();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(1)) return;
        if (slot == null || slot.itemData == null) return;
        if (slot.itemData.allowedSlot != ItemData.EquipmentSlot.None) return;

        // verifica se o mouse está sobre este slot
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, results);

            foreach (var result in results)
            {
                if (result.gameObject.transform.IsChildOf(slot.transform) || result.gameObject == slot.gameObject)
                {
                    slot.UseConsumable();
                    return;
                }
            }
        }
    }
}