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

    void Start()
    {
        // 1. โหลดข้อมูลก่อน
        LoadPlayerData();
        // 2. อัปเดต UI ให้ตรงกับค่าที่โหลดมา
        UpdateAllUI();
    }

    void Update()
    {
        // ลดค่าความหิวและน้ำตามเวลา
        if (hunger > 0) hunger -= decreaseRate * Time.deltaTime;
        if (water > 0) water -= decreaseRate * Time.deltaTime;

        // อัปเดตหลอด UI (เลื่อนไหลตาม Update)
        if (hungerSlider != null) hungerSlider.value = hunger;
        if (waterSlider != null) waterSlider.value = water;

        if (hunger <= 0 || water <= 0)
        {
            Debug.Log("คุณตายแล้ว!");
        }
    }

    // --- ระบบจัดการข้อมูล (Logic) ---

    public void AddMoney(int amount)
    {
        money += amount;
        SavePlayerData(); // เซฟทันทีที่ได้เงิน
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

    // --- ระบบ UI ---

    void UpdateAllUI()
    {
        // อัปเดตตัวเลขเงิน
        if (moneyText != null)
        {
            moneyText.text = "Money: " + money + " Bath";
        }

        // อัปเดตค่า Slider ทันที (เผื่อโหลดข้อมูลมาใหม่)
        if (hungerSlider != null) hungerSlider.value = hunger;
        if (waterSlider != null) waterSlider.value = water;
    }

    // --- ระบบ Save/Load (PlayerPrefs) ---

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("SavedMoney", money);
        PlayerPrefs.SetFloat("SavedHunger", hunger);
        PlayerPrefs.SetFloat("SavedWater", water); // เพิ่มเซฟค่าน้ำด้วยครับ
        PlayerPrefs.Save();
        Debug.Log("บันทึกข้อมูลเรียบร้อย!");
    }

    public void LoadPlayerData()
    {
        money = PlayerPrefs.GetInt("SavedMoney", 0);
        hunger = PlayerPrefs.GetFloat("SavedHunger", 100f);
        water = PlayerPrefs.GetFloat("SavedWater", 100f); // โหลดค่าน้ำด้วย
        Debug.Log("โหลดข้อมูลสำเร็จ!");
    }

    void OnApplicationQuit()
    {
        SavePlayerData(); // เซฟครั้งสุดท้ายก่อนปิดแอป
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SavePlayerData(); // เซฟเมื่อผู้เล่นพับจอ/สลับแอป
    }
}