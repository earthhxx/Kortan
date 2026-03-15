using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemName = "ข้าวกล่อง";
    public int price = 25;
    public float hungerRestore = 30f;

    [Header("UI Reference")]
    public GameObject interactUI; // ลาก InteractPrompt มาใส่
    public TextMeshProUGUI promptText; // ลาก Text ใน InteractPrompt มาใส่

    private bool isPlayerNearby = false;

    void Update()
    {
        // ถ้าอยู่ใกล้และกด E
        if (isPlayerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            BuyItem();
        }
    }

    void BuyItem()
    {
        // หาตัว PlayerStatus
        PlayerStatus playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();

        if (playerStatus != null)
        {
            // ลองจ่ายเงิน
            if (playerStatus.SpendMoney(price))
            {
                // ถ้าจ่ายผ่าน ให้เพิ่มค่าความหิว
                playerStatus.AddHunger(hungerRestore);
                Debug.Log("ซื้อ " + itemName + " สำเร็จ!");
            }
            else
            {
                Debug.Log("เงินไม่พอซื้อ " + itemName);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger ทำงาน! สิ่งที่มาชนคือ: " + other.name);
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