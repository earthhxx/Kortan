using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Player/Character Stats")]
public class CharacterStats : ScriptableObject
{
    [Header("=== 1. Static Data (สเปคโรงงาน) ===")]
    // 🏭 ตั้งค่าได้เต็มที่ใน Inspector ข้อมูลนี้จะไม่มีวันถูกเขียนทับตอนเล่นเกม
    [SerializeField] private int startingMoney = 200;
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] private float maxWater = 100f;
    [SerializeField] private float decreaseRate = 1.0f;

    // === 2. Runtime Data (ข้อมูลชั่วคราวตอนเล่น) ===
    // 🏃‍♂️ [System.NonSerialized] จะซ่อนตัวแปรนี้จาก Inspector และป้องกันไม่ให้ Unity เซฟทับไฟล์
    // ทำให้ ScriptableObject ของเราสะอาดและเป็น "แม่แบบ" ที่แท้จริง
    [System.NonSerialized] private float currentHunger;
    [System.NonSerialized] private float currentWater;
    [System.NonSerialized] private int currentMoney;
    [System.NonSerialized] private bool isDead = false;

    // === 3. Properties (ท่อส่งข้อมูล) ===
    // สคริปต์อื่น (เช่น PlayerStatus) จะดึงข้อมูลผ่านช่องทางนี้ ระบบเก่าเลยไม่พัง!
    public float Hunger => currentHunger;
    public float Water => currentWater;
    public int Money => currentMoney;
    public bool IsDead => isDead;
    public float MaxHunger => maxHunger;
    public float MaxWater => maxWater;

    /// <summary>
    /// Update status decrease over time
    /// </summary>
    public void UpdateStatus(float deltaTime)
    {
        if (isDead) return;

        if (currentHunger > 0) currentHunger -= decreaseRate * deltaTime;
        if (currentWater > 0) currentWater -= decreaseRate * deltaTime;

        CheckDeath();
    }

    private void CheckDeath()
    {
        if (currentHunger <= 0 || currentWater <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("<color=red>Player Died!</color> Hunger or Water reached zero.");
    }

    /// <summary>
    /// Reset stats for new game (โคลนสเปคโรงงานมาใส่ Runtime)
    /// </summary>
    public void ResetForNewGame()
    {
        currentHunger = maxHunger;
        currentWater = maxWater;
        currentMoney = startingMoney;
        isDead = false;
        Debug.Log("<color=green>Stats:</color> เริ่มเกมใหม่! ดึงสเปคโรงงานมาใช้แล้ว");
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        Debug.Log($"<color=yellow>Money:</color> ได้รับเงินเพิ่ม {amount} บาท (รวม: {currentMoney})");
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            Debug.Log($"<color=yellow>Money:</color> จ่ายเงิน {amount} บาท (เหลือ: {currentMoney})");
            return true;
        }
        return false;
    }

    public void AddHunger(float amount)
    {
        currentHunger = Mathf.Min(currentHunger + amount, maxHunger);
        Debug.Log($"<color=orange>Hunger:</color> เพิ่ม {amount} (ปัจจุบัน: {currentHunger})");
    }

    public void AddWater(float amount)
    {
        currentWater = Mathf.Min(currentWater + amount, maxWater);
        Debug.Log($"<color=blue>Water:</color> เพิ่ม {amount} (ปัจจุบัน: {currentWater})");
    }

    public void UpdateUI(Slider hungerSlider, Slider waterSlider, TextMeshProUGUI moneyText)
    {
        if (hungerSlider != null) hungerSlider.value = currentHunger;
        if (waterSlider != null) waterSlider.value = currentWater;
        if (moneyText != null) moneyText.text = $"Money: {currentMoney} Bath";
    }

    // === 4. Persistent Data (ระบบ Save/Load ลงเครื่อง) ===
    public void Save()
    {
        // เซฟข้อมูลชั่วคราว (Runtime) ลงเครื่อง
        PlayerPrefs.SetFloat("SavedHunger", currentHunger);
        PlayerPrefs.SetFloat("SavedWater", currentWater);
        PlayerPrefs.SetInt("SavedMoney", currentMoney);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("SavedMoney"))
        {
            // ถ้ามีไฟล์เซฟ ให้ดึงขึ้นมาทับ Runtime Data
            currentHunger = PlayerPrefs.GetFloat("SavedHunger");
            currentWater = PlayerPrefs.GetFloat("SavedWater");
            currentMoney = PlayerPrefs.GetInt("SavedMoney");
            Debug.Log("<color=cyan>Stats:</color> โหลดข้อมูลจากไฟล์เซฟ PlayerPrefs สำเร็จ!");
        }
        else
        {
            // ถ้าไม่มีไฟล์เซฟ (New Game) ให้ดึงสเปคโรงงาน
            ResetForNewGame();
        }
    }
}