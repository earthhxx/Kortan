using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem; // เพิ่มตัวนี้เข้ามาเพื่อแก้ Error เรื่อง Input

public class PlayerStatus : MonoBehaviour
{
    [Header("Status Settings")]
    public float hunger = 100f;
    public float water = 100f;
    public int money = 0;
    public float decreaseRate = 1.0f;

    [Header("Inventory & Cart")]
    public List<ItemData> cartList = new List<ItemData>();
    public List<ItemData> inventoryList = new List<ItemData>();

    [Header("UI References")]
    public Slider hungerSlider;
    public Slider waterSlider;
    public TextMeshProUGUI moneyText;
    public CartUI cartUI;

    [Header("Death Settings")]
    public GameObject gameOverUI;
    public bool isDead = false;

    void Awake()
    {
        // Singleton System
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        LoadPlayerData();
        UpdateAllUI();
    }

    void Update()
    {
        if (isDead) return;

        HandleStatusDecrease();
        CheckDeath();

        // แก้ไขจุดนี้: เปลี่ยนจาก Input.GetKeyDown เป็นระบบใหม่
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            ShowInventoryDebug();
        }

        // ถ้าช่อง Cart UI ว่างอยู่ ให้ลองพยายามหาในฉากดู
        if (cartUI == null)
        {
            // ค้นหาวัตถุที่มีสคริปต์ CartUI ในฉากปัจจุบัน
            cartUI = Object.FindFirstObjectByType<CartUI>();
        }
    }

    // --- Cart & Inventory Logic ---

    public void AddItemToCart(ItemData item)
    {
        cartList.Add(item);
        if (cartUI != null) cartUI.RefreshCart(cartList);
        Debug.Log($"<color=cyan>Cart:</color> เพิ่ม {item.itemName} ลงตะกร้า (รวม {cartList.Count} ชิ้น)");
    }

    public void Checkout()
    {
        int totalPrice = 0;
        foreach (ItemData item in cartList)
        {
            totalPrice += item.price;
        }

        if (money >= totalPrice)
        {
            // --- กรณีเงินพอ (จ่ายได้) ---
            money -= totalPrice;
            foreach (ItemData item in cartList)
            {
                inventoryList.Add(item);
            }
            cartList.Clear(); // จ่ายแล้วล้างตะกร้า
            Debug.Log("<color=green>Checkout สำเร็จ!</color>");
        }
        else
        {
            // --- กรณีเงินไม่พอ (เงื่อนไขที่คุณต้องการ) ---
            cartList.Clear(); // ลบของออกทั้งหมดทันที
            if (cartUI != null) cartUI.RefreshCart(cartList);
            Debug.Log("<color=red>เงินไม่พอ! ตะกร้าถูกล้างทิ้งทั้งหมด</color>");
        }

        UpdateAllUI();
        // สั่งให้ UI ตะกร้าสะบัดของทิ้ง (Refresh ใหม่ตอนที่ List ว่างเปล่า)
        if (cartUI != null)
        {
            cartUI.RefreshCart(cartList);
        }
        SavePlayerData();
    }

    // --- Status Logic ---

    void HandleStatusDecrease()
    {
        if (hunger > 0) hunger -= decreaseRate * Time.deltaTime;
        if (water > 0) water -= decreaseRate * Time.deltaTime;

        if (hungerSlider != null) hungerSlider.value = hunger;
        if (waterSlider != null) waterSlider.value = water;
    }

    void CheckDeath()
    {
        if (hunger <= 0 || water <= 0) Die();
    }

    void Die()
    {
        isDead = true;

        // 1. ปิด UI เลือด/หิว/น้ำ/เงิน (ถ้าไม่อยากให้เห็นเกะกะ)
        // if (hungerSlider != null) hungerSlider.gameObject.SetActive(false);
        // if (waterSlider != null) waterSlider.gameObject.SetActive(false);
        // if (moneyText != null) moneyText.gameObject.SetActive(false);

        // 2. ปิดหน้าต่างกระเป๋า (ถ้าเปิดค้างไว้)
        var invManager = Object.FindFirstObjectByType<InventoryManager>();
        if (invManager != null)
        {
            invManager.inventoryPanel.SetActive(false);
            // อย่าลืมสั่งปลดล็อกเมาส์ให้ UI ตายกดปุ่ม Respawn ได้
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 3. เปิดหน้าจอ GameOver
        if (gameOverUI != null) gameOverUI.SetActive(true);

        // 4. หยุดการเดิน
        if (GetComponent<PlayerMovement>() != null) GetComponent<PlayerMovement>().enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResetStatusForNewGame()

    {

        isDead = false;

        hunger = 100f;

        water = 100f;

        money = 0;

        cartList.Clear(); // ล้างตะกร้าด้วย

        inventoryList.Clear(); // ล้างกระเป๋าด้วย



        if (GetComponent<PlayerMovement>() != null) GetComponent<PlayerMovement>().enabled = true;

        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;

        UpdateAllUI();

    }

    // --- Public Methods (เรียกใช้จากสคริปต์อื่น) ---

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateAllUI();
        SavePlayerData();
        Debug.Log("<color=yellow>Money:</color> ได้รับเงินเพิ่ม " + amount + " บาท");
    }

    public void AddHunger(float amount)
    {
        hunger = Mathf.Min(hunger + amount, 100f);
        UpdateAllUI();
        SavePlayerData();
        Debug.Log($"<color=orange>Status:</color> กินอาหารเพิ่ม Hunger: {amount}");
    }

    public void AddWater(float amount)
    {
        water = Mathf.Min(water + amount, 100f);
        UpdateAllUI();
        SavePlayerData();
        Debug.Log($"<color=blue>Status:</color> ดื่มน้ำเพิ่ม Water: {amount}");
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateAllUI();
            SavePlayerData();
            return true;
        }
        return false;
    }

    // --- UI & Data ---

    void UpdateAllUI()
    {
        if (moneyText != null) moneyText.text = "Money: " + money + " Bath";
        if (hungerSlider != null) hungerSlider.value = hunger;
        if (waterSlider != null) waterSlider.value = water;
    }

    void ShowInventoryDebug()
    {
        Debug.Log("=== ในกระเป๋าของคุณมี ===");
        if (inventoryList.Count == 0) Debug.Log("กระเป๋าว่างเปล่า");
        foreach (ItemData item in inventoryList)
        {
            Debug.Log("- " + item.itemName);
        }
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("SavedMoney", money);
        PlayerPrefs.SetFloat("SavedHunger", hunger);
        PlayerPrefs.SetFloat("SavedWater", water);

        // --- บันทึก Inventory ---
        // สร้างลิสต์ของชื่อไอเทม เช่น "Oishi,Oishi,Bread"
        List<string> itemNames = new List<string>();
        foreach (ItemData item in inventoryList)
        {
            itemNames.Add(item.name); // เก็บชื่อไฟล์ ScriptableObject
        }
        string allItems = string.Join(",", itemNames); // รวมชื่อด้วยเครื่องหมายคอมม่า
        PlayerPrefs.SetString("SavedInventory", allItems);

        PlayerPrefs.Save();
    }

    public void LoadPlayerData()
    {
        money = PlayerPrefs.GetInt("SavedMoney", 500);
        hunger = PlayerPrefs.GetFloat("SavedHunger", 100f);
        water = PlayerPrefs.GetFloat("SavedWater", 100f);

        // --- โหลด Inventory ---
        string savedItems = PlayerPrefs.GetString("SavedInventory", "");
        inventoryList.Clear();

        if (!string.IsNullOrEmpty(savedItems))
        {
            string[] names = savedItems.Split(',');
            foreach (string itemName in names)
            {
                // ไปโหลดไฟล์ ItemData จากโฟลเดอร์ Resources/Items ตามชื่อที่เซฟไว้
                ItemData loadedItem = Resources.Load<ItemData>("Items/" + itemName);
                if (loadedItem != null)
                {
                    inventoryList.Add(loadedItem);
                }
            }
        }
    }
}