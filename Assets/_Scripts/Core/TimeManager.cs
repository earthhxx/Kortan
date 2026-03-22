using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    [Header("=== 1. Settings ===")]
    public float timeMultiplier = 100f;
    public DateTime currentTime = new DateTime(2020, 12, 30, 6, 0, 0);

    [Header("=== 2. UI References ===")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;

    [Header("=== 3. Day Pass UI ===")]
    [SerializeField] private GameObject dayPassPanel;     // ลากตัว Panel มาใส่
    [SerializeField] private TextMeshProUGUI dayPassText; // ลาก Text ที่จะโชว์วันที่มาใส่
    [SerializeField] private float uiDisplayDuration = 3f; // จะให้โชว์ค้างกี่วินาที

    // ระบบภายใน
    private float originalMultiplier;
    private bool isSleeping = false;
    private PlayerStatus cachedPlayer;
    private int lastDay; // ตัวแปรเก็บวันที่ล่าสุดที่เราเช็ก เพื่อให้รู้ว่าเปลี่ยนวันแล้ว
    void Awake() // เปลี่ยนจาก Start เป็น Awake เพื่อให้โหลดก่อน Update เฟรมแรก
    {
        LoadTime();
    }
    void Start()
    {
        cachedPlayer = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStatus>();
        originalMultiplier = timeMultiplier;

        // ตั้งค่าตัวจำวันจากเวลาที่โหลดมาแล้วใน Awake
        lastDay = currentTime.Day;
        if (dayPassPanel != null) dayPassPanel.SetActive(false);
    }

    void Update()
    {
        UpdateTime();
        // เช็กสภาพอากาศทุกเฟรม
        if (cachedPlayer != null)
        {
            CheckWeather(cachedPlayer);
        }
    }

    void UpdateTime()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);

        // บังคับใช้ระบบปฏิทินแบบสากล (คริสต์ศักราช)
        timeText.text = currentTime.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        dateText.text = currentTime.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture);

        if (currentTime.Day != lastDay)
        {
            lastDay = currentTime.Day; // อัปเดตวันที่ล่าสุด
            StartCoroutine(ShowDayPassUI()); // สั่งโชว์ UI
        }
    }

    public void LoadTime()
    {
        // 1. เช็กว่ามีเซฟเวลาเก่าไหม
        if (PlayerPrefs.HasKey("SavedGameTime"))
        {
            string savedTimeString = PlayerPrefs.GetString("SavedGameTime");

            // 2. พยายามโหลดเวลา (ใส่ try-catch กันเหนียวเผื่อไฟล์พัง)
            try
            {
                currentTime = DateTime.Parse(savedTimeString, null, System.Globalization.DateTimeStyles.RoundtripKind);
                lastDay = currentTime.Day;
                Debug.Log("<color=cyan>TimeManager:</color> โหลดเวลาจากเซฟ: " + currentTime.ToString());
            }
            catch
            {
                Debug.LogWarning("ไฟล์เซฟเวลาพัง! กลับไปเริ่มเช้าวันแรก");
                HardResetTime(); // ถ้าพังให้รีเซ็ตใหม่
            }
        }
        else
        {
            // 3. ถ้าไม่มีเซฟ ก็เริ่มเช้าวันแรก
            Debug.Log("<color=cyan>TimeManager:</color> ไม่มีไฟล์เซฟเวลา กำลังเริ่มใหม่");
            HardResetTime();
        }
    }

    public void UpdateUI()
    {
        if (timeText != null)
            timeText.text = currentTime.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        if (dateText != null)
            dateText.text = currentTime.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }

    IEnumerator ShowDayPassUI()
    {
        if (dayPassPanel != null)
        {
            // เซตข้อความวันที่ใน UI ให้ตรงกับปัจจุบัน
            if (dayPassText != null)
                dayPassText.text = currentTime.ToString("dd MMMM yyyy");

            // เปิด UI
            dayPassPanel.SetActive(true);

            // รอเวลาตามที่ตั้งไว้ (เช่น 3 วินาที)
            yield return new WaitForSeconds(uiDisplayDuration);

            // ปิด UI
            dayPassPanel.SetActive(false);
        }
    }

    // ในไฟล์ TimeManager.cs ฟังก์ชัน CheckWeather
    void CheckWeather(PlayerStatus player)
    {
        if (player.isDead) return;

        float floatTime = currentTime.Hour + (currentTime.Minute / 60f);
        bool isColdTime = (floatTime >= 21.5f || floatTime <= 6.5f);

        if (isColdTime)
        {
            if (player.hasWinterCoat)
            {
                // --- มีเสื้อกันหนาว ---
                if (player.cold < 50f)
                {
                    // ถ้ายังไม่ถึง 50 ให้เพิ่มช้าๆ
                    player.UpdateCold(0.5f * Time.deltaTime);
                }
                else if (player.cold > 50f)
                {
                    // ถ้าหนาวเกิน 50 ไปแล้ว (เช่น เพิ่งซื้อเสื้อมาใส่ตอนที่หนาวจัด) 
                    // ให้ความหนาวค่อยๆ ลดลงมาหยุดที่ 50
                    player.UpdateCold(-2.0f * Time.deltaTime);
                }
            }
            else
            {
                // --- ไม่มีเสื้อกันหนาว ---
                player.UpdateCold(5.0f * Time.deltaTime); // หนาวขึ้นเร็วมาก ทะลุ 100 ได้
            }
        }
        else
        {
            // กลางวัน ความหนาวลดลงปกติ
            player.UpdateCold(-2.0f * Time.deltaTime);
        }
    }

    public void StartSleeping()
    {
        if (isSleeping) return;
        isSleeping = true;
        timeMultiplier = originalMultiplier * 20f;
        Debug.Log("กำลังหลับ... เวลาผ่านไปอย่างรวดเร็ว");
    }

    public void StopSleeping()
    {
        isSleeping = false;
        timeMultiplier = originalMultiplier;
        Debug.Log("ตื่นแล้ว!");
    }

    // ฟังก์ชันนี้จะถูกเรียกจากหน้า Game Over เพื่อบังคับให้เวลากลับไปเริ่มใหม่
    public void HardResetTime()
    {
        // 1. บังคับเซตเวลาใน RAM กลับไปเป็นค่าเริ่มต้น
        currentTime = new DateTime(2020, 12, 30, 6, 0, 0);
        lastDay = currentTime.Day;

        // 2. ลบไฟล์เซฟเวลาทิ้งไปเลย เพื่อป้องกันการแอบโหลดกลับมา
        PlayerPrefs.DeleteKey("SavedGameTime");
        PlayerPrefs.Save();

        // 3. อัปเดต UI ทันที
        if (timeText != null)
            timeText.text = currentTime.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        if (dateText != null)
            dateText.text = currentTime.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture);

        Debug.Log("<color=green>TimeManager:</color> ล้างสมองสำเร็จ! เวลากลับไป 6 โมงเช้าแล้ว");
    }

    public void SaveTime()
    {
        // 1. แปลงเวลาปัจจุบันเป็นข้อความ (Format สากล "o")
        string timeString = currentTime.ToString("o", System.Globalization.CultureInfo.InvariantCulture);

        // 2. เซฟลงเครื่อง
        PlayerPrefs.SetString("SavedGameTime", timeString);
        PlayerPrefs.Save();

        Debug.Log("<color=cyan>TimeManager:</color> บันทึกเวลาสำเร็จ! (" + timeString + ")");
    }
}