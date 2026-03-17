using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CartUI : MonoBehaviour
{
    public Transform container;     // ลากตัวที่มี Vertical Layout Group มาใส่
    public GameObject textPrefab;   // ลาก Prefab ตัวหนังสือรายการของมาใส่
    public TextMeshProUGUI totalText; // ตัวหนังสือโชว์ราคารวม

    public void RefreshCart(List<ItemData> cartList)
    {
        // 1. ล้างรายการเก่า
        foreach (Transform child in container) Destroy(child.gameObject);

        int total = 0;

        // 2. สร้างรายการใหม่ตามของในตะกร้า
        foreach (ItemData item in cartList)
        {
            GameObject newLine = Instantiate(textPrefab, container);
            newLine.GetComponent<TextMeshProUGUI>().text = $"{item.itemName} ({item.price}.-)";
            total += item.price;
        }

        // 3. อัปเดตราคาทั้งหมด
        if (totalText != null) totalText.text = $"รวม: {total} บาท";
    }
}