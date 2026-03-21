using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerStatus : MonoBehaviour
{
    //import CharacterStats และ InventorySystem มาเป็นส่วนประกอบหลักของ PlayerStatus
    [Header("Player Systems")]
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private InventorySystem inventorySystem;

    // Properties for external access (เช่นจาก InventorySlot) ให้เรียกผ่าน PlayerStatus แทนการเข้าถึง CharacterStats หรือ InventorySystem โดยตรง
    public CharacterStats CharacterStats => characterStats;
    public InventorySystem InventorySystem => inventorySystem;

    [Header("UI References")]
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider waterSlider;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private CartUI cartUI;
    [SerializeField] private GameObject gameOverUI;

    [Header("Movement Reference")]
    [SerializeField] private PlayerMovement playerMovement;

    // Private fields
    private bool wasDeadLastFrame = false;

    // Legacy properties for backward compatibility
    public float hunger => characterStats?.Hunger ?? 0f;
    public float water => characterStats?.Water ?? 0f;
    public int money => characterStats?.Money ?? 0;
    public bool isDead => characterStats?.IsDead ?? false;
    public List<ItemData> cartList => inventorySystem?.CartList;
    public List<ItemData> inventoryList => inventorySystem?.InventoryList;

    void Awake()
    {
        // Singleton System
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        // Create systems if not assigned
        if (characterStats == null)
        {
            characterStats = ScriptableObject.CreateInstance<CharacterStats>();
            Debug.LogWarning("CharacterStats not assigned, created default instance");
        }

        if (inventorySystem == null)
        {
            inventorySystem = ScriptableObject.CreateInstance<InventorySystem>();
            Debug.LogWarning("InventorySystem not assigned, created default instance");
        }

        // Get movement reference
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        LoadPlayerData();
        UpdateAllUI();
    }

    void Update()
    {
        if (characterStats.IsDead)
        {
            if (!wasDeadLastFrame) // เรียก HandleDeath() แค่ครั้งเดียว
            {
                HandleDeath();
                wasDeadLastFrame = true;
            }
            return;
        }

        wasDeadLastFrame = false;
        characterStats.UpdateStatus(Time.deltaTime);
        UpdateAllUI(); // <--- เพิ่มตรงนี้ ให้ UI อัพเดตตามสถานะทันที

        // Handle inventory debug input
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            inventorySystem.DebugInventory();
        }

        // Auto-find CartUI if not assigned
        if (cartUI == null)
        {
            cartUI = Object.FindFirstObjectByType<CartUI>();
        }
    }

    // --- Cart & Inventory Logic ---

    public void AddItemToCart(ItemData item)
    {
        inventorySystem.AddItemToCart(item);
        if (cartUI != null) cartUI.RefreshCart(inventorySystem.CartList);
    }

    public void Checkout()
    {
        bool success = inventorySystem.Checkout(characterStats);

        if (success)
        {
            Debug.Log("เย้ ซื้อของสำเร็จ");
        }
        else
        {
            Debug.Log("ว้า เงินไม่พอ");
        }

        UpdateAllUI();
        if (cartUI != null)
            cartUI.RefreshCart(inventorySystem.CartList);

        inventorySystem.Save();
        characterStats.Save();
    }

    // --- Public Methods (เรียกใช้จากสคริปต์อื่น) ---

    public void AddMoney(int amount)
    {
        characterStats.AddMoney(amount);
        UpdateAllUI();
        characterStats.Save();
    }

    public void AddHunger(float amount)
    {
        characterStats.AddHunger(amount);
        UpdateAllUI();
        characterStats.Save();
    }

    public void AddWater(float amount)
    {
        characterStats.AddWater(amount);
        UpdateAllUI();
        characterStats.Save();
    }

    public bool SpendMoney(int amount)
    {
        bool success = characterStats.SpendMoney(amount);
        if (success)
        {
            UpdateAllUI();
            characterStats.Save();
        }
        return success; //return true/false
    }

    // --- Death & Reset ---

    public void HandleDeath()
    {
        // Disable movement
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Close inventory if open
        var invManager = Object.FindFirstObjectByType<InventoryManager>();
        if (invManager != null)
        {
            invManager.InventoryPanel.SetActive(false);
        }

        // Show game over UI
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResetStatusForNewGame()
    {
        characterStats.ResetForNewGame();
        inventorySystem.ClearAll();

        // Re-enable movement
        if (playerMovement != null)
            playerMovement.enabled = true;

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateAllUI();
    }

    // --- UI & Data ---

    void UpdateAllUI()
    {
        characterStats.UpdateUI(hungerSlider, waterSlider, moneyText);
    }

    public void SavePlayerData()
    {
        characterStats.Save();
        inventorySystem.Save();
    }

    public void LoadPlayerData()
    {
        characterStats.Load();
        inventorySystem.Load();
    }

    // ทริค Debug: แค่คลิกขวาที่ชื่อสคริปต์ PlayerStatus ใน Inspector แล้วกด "Debug: Clear Save" เซฟก็จะหายไปเลย!
    [ContextMenu("Debug: Clear Save & Reset")]
    public void DebugClearSave()
    {
        PlayerPrefs.DeleteAll();
        characterStats.ResetForNewGame();
        inventorySystem.ClearAll(); // ถ้ามีฟังก์ชันล้างกระเป๋า
        UpdateAllUI();
        Debug.LogWarning("ลบไฟล์เซฟทดสอบเรียบร้อย! เริ่มค่าใหม่ทั้งหมด");
    }
}