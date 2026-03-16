using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // เพิ่มตัวนี้เพื่อให้เรียกใช้ Keyboard ง่ายขึ้น

public class CheckoutPoint : MonoBehaviour
{
    public GameObject interactUI;
    public TextMeshProUGUI promptText;
    private bool isPlayerNearby = false;

    void Update()
    {
        // ใช้ Keyboard.current แทนการพิมพ์ยาวๆ
        if (isPlayerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            PlayerStatus player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
            
            if (player != null)
            {
                if (player.cartList.Count > 0)
                {
                    player.Checkout();
                    UpdateCheckoutUI(player); // อัปเดตข้อความหลังจ่ายเงินเสร็จ
                }
                else
                {
                    Debug.Log("ไม่มีสินค้าในตะกร้า!");
                    if (promptText != null) promptText.text = "<color=red>ตะกร้าว่างเปล่า!</color>";
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactUI != null) interactUI.SetActive(true);
            
            PlayerStatus player = other.GetComponent<PlayerStatus>();
            UpdateCheckoutUI(player);
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

    // ฟังก์ชันช่วยอัปเดตข้อความให้โชว์จำนวนของและราคาทั้งหมด
    void UpdateCheckoutUI(PlayerStatus player)
    {
        if (promptText == null || player == null) return;

        if (player.cartList.Count > 0)
        {
            int total = 0;
            foreach (var item in player.cartList) total += item.price;
            promptText.text = $"[E] จ่ายเงิน {player.cartList.Count} รายการ\nยอดรวม: <color=yellow>{total} บาท</color>";
        }
        else
        {
            promptText.text = "ตะกร้าว่างเปล่า";
        }
    }
}