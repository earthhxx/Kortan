using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class CheckoutPoint : MonoBehaviour
{
    [Header("UI References")]
    public GameObject interactUI;       // ลากตัว interactive (BG) มาใส่ช่องนี้
    public TextMeshProUGUI checkoutText; // ลาก Checkout_text มาใส่ช่องนี้

    private bool isPlayerNearby = false;

    void Start()
    {
        // ปิดข้อความไว้ก่อนตอนเริ่มเกม
        if (checkoutText != null) checkoutText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            PlayerManager player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
            
            if (player != null)
            {
                if (player.cartList.Count > 0)
                {
                    player.Checkout();
                    UpdateCheckoutUI(player); 
                }
                else
                {
                    Debug.Log("ไม่มีสินค้าในตะกร้า!");
                    if (checkoutText != null) checkoutText.text = "<color=red>ตะกร้าว่างเปล่า!</color>";
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            
            // 1. เปิดแผ่นป้าย BG
            if (interactUI != null) interactUI.SetActive(true);
            
            // 2. เปิดเฉพาะข้อความ Checkout
            if (checkoutText != null) checkoutText.gameObject.SetActive(true);
            
            PlayerManager player = other.GetComponent<PlayerManager>();
            UpdateCheckoutUI(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            
            // ปิดแผ่นป้าย BG (พวก Text ที่เป็นลูกก็จะโดนซ่อนไปด้วยอัตโนมัติ)
            if (interactUI != null) interactUI.SetActive(false);
            
            // ปิดข้อความตัวเองเผื่อไว้ด้วย
            if (checkoutText != null) checkoutText.gameObject.SetActive(false);
        }
    }

    void UpdateCheckoutUI(PlayerManager player)
    {
        if (checkoutText == null || player == null) return;

        if (player.cartList.Count > 0)
        {
            int total = 0;
            foreach (var item in player.cartList) total += item.price;
            checkoutText.text = $"[E] จ่ายเงิน {player.cartList.Count} รายการ\nยอดรวม: <color=yellow>{total} บาท</color>";
        }
        else
        {
            checkoutText.text = "ตะกร้าว่างเปล่า";
        }
    }
}