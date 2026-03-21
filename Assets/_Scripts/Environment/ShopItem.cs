using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ShopItem : MonoBehaviour
{
    // === 1. Static Data (ตั้งค่าใน Inspector) ===
    [Header("=== 1. Item Data ===")]
    [SerializeField] private ItemData itemData;

    [Header("=== 2. UI References ===")]
    [SerializeField] private GameObject interactUI;
    [SerializeField] private TextMeshProUGUI promptText;

    [Header("=== 3. Interaction Settings ===")]
    [SerializeField] private Key interactKey = Key.E;

    // === 2. Runtime Data (ข้อมูลตอนเล่น) ===
    [System.NonSerialized] private bool isPlayerNearby = false;
    [System.NonSerialized] private PlayerStatus cachedPlayer;

    // === 3. Properties ===
    public ItemData ItemData => itemData;
    public bool IsPlayerNearby => isPlayerNearby;

    // === 4. Lifecycle ===

    void Start()
    {
        // Cache player reference
        cachedPlayer = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStatus>();
    }

    void Update()
    {
        // ตรวจสอบว่าผู้เล่นอยู่ใกล้และกดปุ่มซื้อ
        if (isPlayerNearby && Keyboard.current[interactKey].wasPressedThisFrame && itemData != null && cachedPlayer != null)
        {
            AddToCart();
        }
    }

    /// <summary>
    /// Add item to player cart
    /// </summary>
    private void AddToCart()
    {
        cachedPlayer.AddItemToCart(itemData);
        Debug.Log($"<color=cyan>Shop:</color> เพิ่ม {itemData.itemName} ลงตะกร้า!");
    }

    /// <summary>
    /// Handle player enter trigger
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && itemData != null)
        {
            isPlayerNearby = true;

            if (interactUI != null)
                interactUI.SetActive(true);

            if (promptText != null)
                promptText.text = $"[E] เก็บ {itemData.itemName} ({itemData.price} บาท)";

            Debug.Log($"<color=cyan>Shop:</color> เข้าใกล้ {itemData.itemName}");
        }
    }

    /// <summary>
    /// Handle player exit trigger
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            if (interactUI != null)
                interactUI.SetActive(false);

            Debug.Log("<color=cyan>Shop:</color> ห่างจากสินค้า");
        }
    }
}