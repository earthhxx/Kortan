using UnityEngine;

// บรรทัดนี้จะทำให้เราคลิกขวาใน Unity แล้วสร้างไฟล์ไอเทมได้
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;          // รูปที่จะโชว์ใน Inventory
    public int price;
    
    public enum ItemType { Food, Water, Other }
    public ItemType type;

    public float restoreAmount;  // ค่าที่เพิ่มความหิว/น้ำ
}