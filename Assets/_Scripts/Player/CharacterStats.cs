using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Player/Character Stats")]
public class CharacterStats : ScriptableObject
{
    [Header("=== 1. Factory Settings (ค่าเริ่มต้น/เพดานสูงสุด) ===")]
    [SerializeField] private int startingMoney = 200;
    
    [Space(5)]
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] private float startingHunger = 100f; 
    
    [Space(5)]
    [SerializeField] private float maxWater = 100f;
    [SerializeField] private float startingWater = 100f;
    
    [Space(5)]
    [SerializeField] private float maxCold = 100f;
    [SerializeField] private float startingCold = 0f;  

    [Space(5)]
    [SerializeField] private float decreaseRate = 1.0f;

    // === 2. Runtime Data (ข้อมูลชั่วคราว) ===
    [System.NonSerialized] private float currentHunger;
    [System.NonSerialized] private float currentWater;
    [System.NonSerialized] private int currentMoney;
    [System.NonSerialized] private float currentCold;
    [System.NonSerialized] private bool isDead = false;

    // === 3. Properties ===
    public float Hunger => currentHunger;
    public float Water => currentWater;
    public int Money => currentMoney;
    public float Cold => currentCold;
    public bool IsDead => isDead;
    public float MaxHunger => maxHunger;
    public float MaxWater => maxWater;
    public float MaxCold => maxCold;

    public void UpdateStatus(float deltaTime)
    {
        if (isDead) return;

        // หิวและน้ำลดลงตามเวลา (เฉพาะตอนที่ยังมากกว่า 0) และความหนาวเพิ่มขึ้น (เฉพาะตอนที่ยังน้อยกว่า max)
        if (currentHunger > 0) currentHunger -= decreaseRate * deltaTime;
        if (currentWater > 0) currentWater -= decreaseRate * deltaTime;
        if (currentCold < maxCold) currentCold += decreaseRate * deltaTime;

        CheckDeath();
    }

    private void CheckDeath()
    {
        // ตายเมื่อหิวหรือกระหายจนหมด (0)
        if (currentHunger <= 0 || currentWater <= 0 || currentCold >= maxCold) Die();
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("<color=red>Player Died!</color>");
    }

    /// <summary>
    /// ใช้ค่า Starting จาก Inspector มาตั้งต้นใหม่
    /// </summary>
    public void ResetForNewGame()
    {
        currentHunger = startingHunger; 
        currentWater = startingWater;   
        currentMoney = startingMoney;
        currentCold = startingCold;    
        isDead = false;
        Debug.Log("<color=green>Stats Reset:</color> เริ่มต้นใหม่ด้วยค่า Starting!");
    }

    public void AddCold(float amount)
    {
        currentCold = Mathf.Clamp(currentCold + amount, 0, maxCold);
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            return true;
        }
        return false;
    }

    public void AddHunger(float amount)
    {
        currentHunger = Mathf.Clamp(currentHunger + amount, 0, maxHunger);
    }

    public void AddWater(float amount)
    {
        currentWater = Mathf.Clamp(currentWater + amount, 0, maxWater);
    }

    public void UpdateUI(Slider hungerSlider, Slider waterSlider, Slider coldSlider, TextMeshProUGUI moneyText)
    {
        if (hungerSlider != null) hungerSlider.value = currentHunger;
        if (waterSlider != null) waterSlider.value = currentWater;
        if (coldSlider != null) coldSlider.value = currentCold;
        if (moneyText != null) moneyText.text = $"Money: {currentMoney} Bath";
    }

    // === 4. Save/Load ===
    public void Save()
    {
        PlayerPrefs.SetFloat("SavedHunger", currentHunger);
        PlayerPrefs.SetFloat("SavedWater", currentWater);
        PlayerPrefs.SetFloat("SavedCold", currentCold);
        PlayerPrefs.SetInt("SavedMoney", currentMoney);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("SavedMoney"))
        {
            currentHunger = PlayerPrefs.GetFloat("SavedHunger");
            currentWater = PlayerPrefs.GetFloat("SavedWater");
            currentCold = PlayerPrefs.GetFloat("SavedCold");
            currentMoney = PlayerPrefs.GetInt("SavedMoney");
        }
        else
        {
            ResetForNewGame();
        }
    }
}