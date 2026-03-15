using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ShopItem : MonoBehaviour
{
    // สร้างตัวเลือกประเภทไอเทม
    public enum ItemType { Food, Water }

    [Header("Item Settings")]
    public ItemType type = ItemType.Food; // เลือกใน Inspector ได้เลย
    public string itemName = "ของกิน/ของดื่ม";
    public int price = 25;
    public float restoreAmount = 30f; // ค่าพลังที่จะฟื้นฟู (หิว หรือ น้ำ)

    [Header("UI Reference")]
    public GameObject interactUI; 
    public TextMeshProUGUI promptText; 

    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            BuyItem();
        }
    }

    void BuyItem()
    {
        PlayerStatus playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();

        if (playerStatus != null)
        {
            if (playerStatus.SpendMoney(price))
            {
                // เช็กประเภทไอเทมแล้วเพิ่มค่าให้ถูกหลอด
                if (type == ItemType.Food)
                {
                    playerStatus.AddHunger(restoreAmount);
                }
                else if (type == ItemType.Water)
                {
                    playerStatus.AddWater(restoreAmount);
                }

                Debug.Log($"ซื้อ {itemName} สำเร็จ!");
            }
            else
            {
                Debug.Log("เงินไม่พอซื้อ " + itemName);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactUI != null)
            {
                interactUI.SetActive(true);
                if (promptText != null) 
                    promptText.text = $"[E] ซื้อ {itemName} ({price} บาท)";
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