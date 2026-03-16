using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // ต้องมีเพื่อใช้ระบบคลิก

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public ItemData item; // ข้อมูลไอเทมในช่องนี้

    // ฟังก์ชันนี้จะทำงานเมื่อมีการคลิกที่ช่อง
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2) // เช็ก Double Click
        {
            UseItem();
        }
    }

    public void UseItem()
    {
        if (item == null)
        {
            Debug.LogError("ช่องนี้ไม่มีข้อมูลไอเทม!");
            return;
        }

        PlayerStatus player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();

        if (player != null)
        {
            // 1. เพิ่มค่าพลัง (เช็กชื่อตัวแปรใน ItemData ให้ตรง)
            if (item.type == ItemData.ItemType.Food)
                player.AddHunger(item.restoreAmount);
            else if (item.type == ItemData.ItemType.Water)
                player.AddWater(item.restoreAmount);

            // 2. ลบออกจาก List ของ Player (ลบตัวที่เลือกจริงๆ)
            player.inventoryList.Remove(item);

            // 3. สั่ง Refresh หน้าจอทันที
            Object.FindFirstObjectByType<InventoryManager>().RefreshInventory();

            Debug.Log($"ใช้ {item.itemName} แล้ว! ค่าที่ฟื้นฟู: {item.restoreAmount}");
        }
    }
}