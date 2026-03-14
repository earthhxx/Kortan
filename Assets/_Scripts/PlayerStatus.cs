using UnityEngine;
using UnityEngine.UI;
using TMPro; // สำหรับใช้ TextMeshPro

public class PlayerStatus : MonoBehaviour
{
    [Header("Status Settings")]
    public float hunger = 100f;
    public float water = 100f;
    public int money = 0;
    public float decreaseRate = 1.0f; // ลดลงวินาทีละ 1 หน่วย

    [Header("UI References")]
    public Slider hungerSlider;
    public Slider waterSlider;
    public TextMeshProUGUI moneyText; // ใช้ตัวเดียวที่เป็น TMP

    void Start()
    {
        // อัปเดต UI ครั้งแรกตอนเริ่มเกม
        UpdateMoneyUI();
    }

    void Update()
    {
        // 1. ลดค่าความหิวและน้ำตามเวลา
        if (hunger > 0) hunger -= decreaseRate * Time.deltaTime;
        if (water > 0) water -= decreaseRate * Time.deltaTime;

        // 2. อัปเดตหลอดอาหาร/น้ำ (อัปเดตทุกเฟรมเพื่อให้หลอดค่อยๆ ลดสวยๆ)
        if (hungerSlider != null) hungerSlider.value = hunger;
        if (waterSlider != null) waterSlider.value = water;

        // 3. เช็กถ้าหิวจนตาย
        if (hunger <= 0 || water <= 0)
        {
            Debug.Log("คุณตายแล้ว!");
        }
    }

    // Method สำหรับเพิ่มเงิน (เรียกจาก Script เหรียญ)
    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyUI(); // อัปเดตตัวเลขเงินทันทีที่ได้เงิน
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "Money: " + money + " Bath";
        }
    }

    // Method สำหรับเพิ่มอาหาร/น้ำ
    public void AddHunger(float amount) => hunger = Mathf.Min(hunger + amount, 100f);
    public void AddWater(float amount) => water = Mathf.Min(water + amount, 100f);
}