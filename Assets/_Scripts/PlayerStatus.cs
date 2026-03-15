using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatus : MonoBehaviour
{
    [Header("Status Settings")]
    public float hunger = 100f;
    public float water = 100f;
    public int money = 0;
    public float decreaseRate = 1.0f;

    [Header("UI References")]
    public Slider hungerSlider;
    public Slider waterSlider;
    public TextMeshProUGUI moneyText;

    [Header("Death Settings")]
    public GameObject gameOverUI;
    public bool isDead = false;

    // --- Unity Events ---

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
    }

    // --- Status Logic ---

    void HandleStatusDecrease()
    {
        // ลดค่าตามเวลา
        if (hunger > 0) hunger -= decreaseRate * Time.deltaTime;
        if (water > 0) water -= decreaseRate * Time.deltaTime;

        // อัปเดต Slider ตลอดเวลาให้เห็นการลดที่ลื่นไหล
        if (hungerSlider != null) hungerSlider.value = hunger;
        if (waterSlider != null) waterSlider.value = water;
    }

    void CheckDeath()
    {
        if (hunger <= 0 || water <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        isDead = true;
        Debug.Log("คุณตายแล้ว!");

        if (gameOverUI != null) gameOverUI.SetActive(true);

        // --- หยุดการเคลื่อนที่ ---
        // ปิดสคริปต์เดินที่คุณเขียนเอง
        if (GetComponent<PlayerMovement>() != null)
            GetComponent<PlayerMovement>().enabled = false;

        // ปลดล็อกเมาส์
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResetStatusForNewGame()
    {
        isDead = false;
        hunger = 100f;
        water = 100f;
        money = 0;

        // --- กลับมาเดินได้ ---
        // เปิดสคริปต์เดินให้กลับมาทำงาน
        if (GetComponent<PlayerMovement>() != null)
            GetComponent<PlayerMovement>().enabled = true;

        // ล็อกเมาส์กลับคืน
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateAllUI();
    }

    // --- Public Methods (เรียกใช้จากสคริปต์อื่น) ---

    public void AddMoney(int amount)
    {
        money += amount;
        SavePlayerData();
        UpdateAllUI();
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            SavePlayerData();
            UpdateAllUI();
            return true;
        }
        return false;
    }

    public void AddHunger(float amount)
    {
        hunger = Mathf.Min(hunger + amount, 100f);
        SavePlayerData();
        UpdateAllUI();
    }

    public void AddWater(float amount)
    {
        water = Mathf.Min(water + amount, 100f);
        SavePlayerData();
        UpdateAllUI();
    }

    // --- UI & Persistence ---

    void UpdateAllUI()
    {
        if (moneyText != null)
            moneyText.text = "Money: " + money + " Bath";

        if (hungerSlider != null) hungerSlider.value = hunger;
        if (waterSlider != null) waterSlider.value = water;
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("SavedMoney", money);
        PlayerPrefs.SetFloat("SavedHunger", hunger);
        PlayerPrefs.SetFloat("SavedWater", water);
        PlayerPrefs.Save();
        Debug.Log("บันทึกข้อมูลเรียบร้อย!");
    }
    public void LoadPlayerData()
    {
        money = PlayerPrefs.GetInt("SavedMoney", 0);
        hunger = PlayerPrefs.GetFloat("SavedHunger", 100f);
        water = PlayerPrefs.GetFloat("SavedWater", 100f);
        Debug.Log("โหลดข้อมูลสำเร็จ!");
    }

    // --- Lifecycle Hooks ---

    void OnApplicationQuit()
    {
        SavePlayerData();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SavePlayerData();
    }
}