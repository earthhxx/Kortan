using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    // === 1. Static Data (ตั้งค่าใน Inspector) ===
    [Header("=== 1. UI References ===")]
    [SerializeField] private GameObject inventoryPanel;  // ลาก InventoryPanel มาใส่
    [SerializeField] private Transform itemContainer;    // ลาก ItemContainer มาใส่
    [SerializeField] private GameObject slotPrefab;      // ลาก ItemSlot Prefab มาใส่

    [Header("=== 2. Input Settings ===")]
    // เปลี่ยนจาก KeyCode เป็น Key (ของ UnityEngine.InputSystem)
    [SerializeField] private Key inventoryKeyNew = Key.I;

    // === 2. Runtime Data (ข้อมูลตอนเล่น - ซ่อนจาก Inspector) ===
    [System.NonSerialized] private bool isOpen = false;
    [System.NonSerialized] private PlayerStatus cachedPlayer;

    // === 3. Properties (ทำให้โค้ดสะอาด) ===
    public bool IsOpen => isOpen;
    public GameObject InventoryPanel => inventoryPanel; // Allow external access to close inventory on death

    // === 4. Lifecycle ===

    void Start()
    {
        inventoryPanel.SetActive(false);
        // Cache PlayerStatus reference เพื่อไม่ต้องเรียก FindGameObject ทุก frame
        cachedPlayer = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStatus>();

        if (cachedPlayer == null)
        {
            Debug.LogError("[InventoryManager] Player not found!");
        }

        RefreshInventory();
    }

    void Update()
    {
        // ใช้ cached player แทน FindGameObject
        if (cachedPlayer == null) return;

        // ถ้าตายแล้ว ให้ปิดกระเป๋าและหยุดทำงาน
        if (cachedPlayer.isDead)
        {
            if (isOpen) ToggleInventory();
            return;
        }

        // ตรวจสอบปุ่มเปิดกระเป๋า
        if (Keyboard.current[inventoryKeyNew].wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        // Get player movement component
        PlayerMovement movementScript = cachedPlayer?.GetComponent<PlayerMovement>();

        if (isOpen)
        {
            // Open inventory: unlock cursor & disable movement
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (movementScript != null) movementScript.enabled = false;
            RefreshInventory();

            Debug.Log("<color=yellow>Inventory:</color> เปิดกระเป๋า");
        }
        else
        {
            // Close inventory: lock cursor & enable movement
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (movementScript != null) movementScript.enabled = true;

            Debug.Log("<color=yellow>Inventory:</color> ปิดกระเป๋า");
        }
    }

    /// <summary>
    /// Refresh inventory display - recreate all slots from player inventory
    /// </summary>
    public void RefreshInventory()
    {
        if (cachedPlayer == null || cachedPlayer.InventorySystem == null) return;

        // Clear old slots
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        // Create slots for each item
        foreach (ItemData data in cachedPlayer.InventorySystem.InventoryList)
        {
            GameObject newSlot = Instantiate(slotPrefab, itemContainer);

            InventorySlot slotScript = newSlot.GetComponent<InventorySlot>();
            if (slotScript != null)
            {
                slotScript.Initialize(data, cachedPlayer, this);
            }

            // Set visual
            var iconImage = newSlot.transform.Find("Icon")?.GetComponent<UnityEngine.UI.Image>();
            if (iconImage != null)
                iconImage.sprite = data.icon;
        }
    }
}