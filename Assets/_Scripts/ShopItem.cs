using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ShopItem : MonoBehaviour
{
    // ลากไฟล์ ScriptableObject (เช่น Oishi_GreenTea) มาใส่ตรงนี้ใน Inspector
    public ItemData data;

    [Header("UI Reference")]
    public GameObject interactUI;
    public TextMeshProUGUI promptText;

    private bool isPlayerNearby = false;

    void Update()
    {
        // เช็กด้วยว่ามีข้อมูล data หรือยังก่อนจะกดซื้อ
        if (isPlayerNearby && Keyboard.current.eKey.wasPressedThisFrame && data != null)
        {
            AddToCart(); // เปลี่ยนชื่อฟังก์ชันให้ตรงกับสิ่งที่จะทำ
        }
    }

    void AddToCart()
    {
        PlayerStatus playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();

        if (playerStatus != null)
        {
            // ส่งข้อมูลไอเทม (data) ไปที่ตะกร้าในตัว Player
            // เดี๋ยวเราจะไปสร้างฟังก์ชัน AddToCart ใน PlayerStatus กันครับ
            playerStatus.AddItemToCart(data); 
            Debug.Log($"เพิ่ม {data.itemName} ลงในรถเข็นแล้ว!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && data != null)
        {
            isPlayerNearby = true;
            if (interactUI != null)
            {
                interactUI.SetActive(true);
                if (promptText != null)
                    // ดึงชื่อและราคามาจาก ScriptableObject โดยตรง
                    promptText.text = $"[E] เก็บ {data.itemName} ({data.price} บาท)";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }
}