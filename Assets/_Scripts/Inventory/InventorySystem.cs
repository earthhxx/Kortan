using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "InventorySystem", menuName = "Player/Inventory System")]
public class InventorySystem : ScriptableObject
{
    [Header("Inventory Data")]
    [SerializeField] private List<ItemData> cartList = new List<ItemData>();
    [SerializeField] private List<ItemData> inventoryList = new List<ItemData>();

    // Properties
    public List<ItemData> CartList => cartList;
    public List<ItemData> InventoryList => inventoryList;

    /// <summary>
    /// Add item to cart
    /// </summary>
    public void AddItemToCart(ItemData item)
    {
        cartList.Add(item);
        Debug.Log($"<color=cyan>Cart:</color> เพิ่ม {item.itemName} ลงตะกร้า (รวม {cartList.Count} ชิ้น)");
    }

    /// <summary>
    /// Checkout cart items to inventory
    /// </summary>
    public bool Checkout(CharacterStats stats)
    {
        int totalPrice = 0;
        foreach (ItemData item in cartList)
        {
            totalPrice += item.price;
        }

        if (stats.SpendMoney(totalPrice))
        {
            // Success - move items to inventory
            foreach (ItemData item in cartList)
            {
                inventoryList.Add(item);
            }
            cartList.Clear();
            Debug.Log("<color=green>Checkout สำเร็จ!</color>");
            return true;
        }
        else
        {
            // Failed - clear cart
            cartList.Clear();
            Debug.Log("<color=red>เงินไม่พอ! ตะกร้าถูกล้างทิ้งทั้งหมด</color>");
            return false;
        }
    }

    /// <summary>
    /// Use item from inventory
    /// </summary>
  // เปลี่ยนพารามิเตอร์เป็น PlayerStatus player (ตามที่เราคุยกันไปก่อนหน้านี้)
    public bool UseItem(ItemData item, PlayerStatus player)
    {
        if (!inventoryList.Contains(item))
            return false;

        // แยกการทำงานตามหมวดหมู่ไอเทม!
        switch (item.type)
        {
            case ItemData.ItemType.Food:
                player.CharacterStats.AddHunger(item.restoreAmount);
                break;

            case ItemData.ItemType.Water:
                player.CharacterStats.AddWater(item.restoreAmount);
                break;

            case ItemData.ItemType.Equipment:
                // สั่งให้ Player ใส่เสื้อ (และเริ่มนับถอยหลัง 14 วัน)
                player.EquipWinterCoat(14);
                break;
        }

        // ลบไอเทมออกจากกระเป๋า (เพราะกินไปแล้ว หรือเอาไปใส่บนตัวแล้ว)
        inventoryList.Remove(item);
        return true;
    }

    /// <summary>
    /// Clear all inventory and cart
    /// </summary>
    public void ClearAll()
    {
        cartList.Clear();
        inventoryList.Clear();
    }

    /// <summary>
    /// Get total cart price
    /// </summary>
    public int GetCartTotalPrice()
    {
        int total = 0;
        foreach (ItemData item in cartList)
        {
            total += item.price;
        }
        return total;
    }

    /// <summary>
    /// Debug inventory contents
    /// </summary>
    public void DebugInventory()
    {
        Debug.Log("=== ในกระเป๋าของคุณมี ===");
        if (inventoryList.Count == 0)
        {
            Debug.Log("กระเป๋าว่างเปล่า");
        }
        else
        {
            foreach (ItemData item in inventoryList)
            {
                Debug.Log($"- {item.itemName}");
            }
        }
    }

    /// <summary>
    /// Save inventory to PlayerPrefs
    /// </summary>
    public void Save()
    {
        // Save inventory items
        List<string> itemNames = new List<string>();
        foreach (ItemData item in inventoryList)
        {
            itemNames.Add(item.name);
        }
        string allItems = string.Join(",", itemNames);
        PlayerPrefs.SetString("SavedInventory", allItems);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Load inventory from PlayerPrefs
    /// </summary>
    public void Load()
    {
        inventoryList.Clear();

        string savedItems = PlayerPrefs.GetString("SavedInventory", "");
        if (!string.IsNullOrEmpty(savedItems))
        {
            string[] names = savedItems.Split(',');
            foreach (string itemName in names)
            {
                ItemData loadedItem = Resources.Load<ItemData>("Items/" + itemName);
                if (loadedItem != null)
                {
                    inventoryList.Add(loadedItem);
                }
            }
        }
    }
}