using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ItemData item;
    private PlayerStatus playerStatus; // Cache reference - ไม่ต้องเรียก FindGameObject ซ้ำ
    private InventoryManager inventoryManager;

    /// <summary>
    /// Initialize slot with all required references
    /// </summary>
    public void Initialize(ItemData itemData, PlayerStatus player, InventoryManager invManager)
    {
        item = itemData;
        playerStatus = player;
        inventoryManager = invManager;
    }

    /// <summary>
    /// Handle double-click event
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2) // Double click
        {
            UseItem();
        }
    }

    /// <summary>
    /// Use the item and remove it from inventory
    /// </summary>
    public void UseItem()
    {
        // Validate item
        if (item == null)
        {
            Debug.LogError("ช่องนี้ไม่มีข้อมูลไอเทม!");
            return;
        }

        // Validate player reference
        if (playerStatus == null)
        {
            Debug.LogError("PlayerStatus reference ไม่ได้ถูกตั้งค่า!");
            return;
        }

        // Use item through inventory system
        bool success = playerStatus.InventorySystem.UseItem(item, playerStatus.CharacterStats);

        if (success)
        {
            // Refresh UI
            if (inventoryManager != null)
                inventoryManager.RefreshInventory();

            Debug.Log($"<color=yellow>ใช้ {item.itemName} แล้ว!</color> ค่าที่ฟื้นฟู: {item.restoreAmount}");
        }
        else
        {
            Debug.LogError($"ไม่สามารถใช้ไอเทม {item.itemName} ได้!");
        }
    }
}