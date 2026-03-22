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
    [SerializeField] private TimeManager timeManager;

    // Properties for external access (เช่นจาก InventorySlot) ให้เรียกผ่าน PlayerStatus แทนการเข้าถึง CharacterStats หรือ InventorySystem โดยตรง
    public CharacterStats CharacterStats => characterStats;
    public InventorySystem InventorySystem => inventorySystem;
    public TimeManager TimeManager => timeManager;

    [Header("UI References")]
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider waterSlider;
    [SerializeField] private Slider coldSlider;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private CartUI cartUI;
    [SerializeField] private GameObject gameOverUI;

    [Header("Movement Reference")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Survival Settings")]
    public bool hasWinterCoat = false; // สวิตช์เช็กเสื้อกันหนาว
    public string coatExpiryString = ""; // เก็บวันที่เสื้อพังเป็น String เพื่อให้เซฟง่ายๆ

    // Private fields
    private bool wasDeadLastFrame = false;

    // Legacy properties for backward compatibility
    public float hunger => characterStats?.Hunger ?? 0f;
    public float water => characterStats?.Water ?? 0f;
    public int money => characterStats?.Money ?? 0;
    public float cold => characterStats?.Cold ?? 0f;
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
        CheckCoatDurability(); // เช็กสภาพเสื้อกันหนาวทุกเฟรม
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

    // --- Survival Logic (เรียกจาก TimeManager) ---
    public void UpdateCold(float amount)
    {
        if (characterStats != null)
        {
            characterStats.AddCold(amount); // เพิ่มหรือลดค่าความหนาวใน CharacterStats
            UpdateAllUI();
        }
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

    // เรียกใช้ตอนกดซื้อหรือกดใช้เสื้อ
    public void EquipWinterCoat(int daysToLast)
    {
        hasWinterCoat = true;

        // คำนวณวันหมดอายุ = เวลาในเกมปัจจุบัน + จำนวนวันที่กำหนด (14 วัน)
        System.DateTime expiryDate = timeManager.currentTime.AddDays(daysToLast);
        coatExpiryString = expiryDate.ToString("o", System.Globalization.CultureInfo.InvariantCulture);

        Debug.Log($"<color=cyan>Item:</color> ใส่เสื้อกันหนาวแล้ว! จะพังในวันที่: {expiryDate.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture)}");
        SavePlayerData(); // บังคับเซฟทันที
    }

    // ฟังก์ชันสำหรับเช็กว่าเสื้อพังหรือยัง
    private void CheckCoatDurability()
    {
        if (hasWinterCoat && !string.IsNullOrEmpty(coatExpiryString))
        {
            System.DateTime expiryDate = System.DateTime.Parse(coatExpiryString, null, System.Globalization.DateTimeStyles.RoundtripKind);

            // ถ้าเวลาปัจจุบัน เลยเวลาหมดอายุของเสื้อไปแล้ว
            if (timeManager.currentTime >= expiryDate)
            {
                hasWinterCoat = false;
                coatExpiryString = "";
                Debug.LogWarning("<color=red>System:</color> เสื้อกันหนาวของคุณพังแล้ว! ขาดรุ่ยทนความหนาวไม่ได้อีกต่อไป");
                SavePlayerData();
            }
        }
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
        timeManager?.HardResetTime();

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
        characterStats.UpdateUI(hungerSlider, waterSlider, coldSlider, moneyText);
        timeManager?.UpdateUI();
    }

    public void SavePlayerData()
    {
        characterStats.Save();
        inventorySystem.Save();
        timeManager?.SaveTime();
        PlayerPrefs.SetInt("SavedHasCoat", hasWinterCoat ? 1 : 0);
        PlayerPrefs.SetString("SavedCoatExpiry", coatExpiryString);
    }

    public void LoadPlayerData()
    {
        characterStats.Load();
        inventorySystem.Load();
        timeManager?.LoadTime();
        hasWinterCoat = PlayerPrefs.GetInt("SavedHasCoat", 0) == 1;
        coatExpiryString = PlayerPrefs.GetString("SavedCoatExpiry", "");
    }

    // ==========================================
    // --- Auto-Save System (ระบบเซฟอัตโนมัติ) ---
    // ==========================================

    // ฟังก์ชันนี้จะทำงานเมื่อผู้เล่น "พับจอ" (สำหรับเกมมือถือ/แท็บเล็ต)
    // หรือตอนกด Home Button
    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            // ถ้าไม่ได้อยู่ในสถานะตาย ให้เซฟเกมซะ
            if (!characterStats.IsDead)
            {
                SavePlayerData();
                Debug.Log("<color=yellow>System:</color> พับจอ! ทำการ Auto-Save เรียบร้อย");
            }
        }
    }

    // ฟังก์ชันนี้จะทำงานเมื่อผู้เล่น "ปิดเกม" 
    // (กดกากบาท, Alt+F4, หรืองัดแอปทิ้ง)
    private void OnApplicationQuit()
    {
        // ถ้าผู้เล่นตายอยู่ เราจะไม่เซฟทับ (ไม่งั้นโหลดมาก็จะตายอยู่ดี)
        if (!characterStats.IsDead)
        {
            SavePlayerData();
            Debug.Log("<color=red>System:</color> ออกจากเกม! ทำการ Auto-Save เรียบร้อย");
        }
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