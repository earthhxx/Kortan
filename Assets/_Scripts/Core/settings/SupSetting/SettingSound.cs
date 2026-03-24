using UnityEngine;
using UnityEngine.UI; // ถ้ามี Slider ให้ใส่ไว้ด้วย

public class SettingSound : MonoBehaviour
{
    [Header("Sound Values")]
    [Range(0f, 1f)] public float masterVolume = 1f;

    [Header("UI References (Optional)")]
    public Slider volumeSlider;

    // ฟังก์ชันนี้เอาไปผูกกับ OnValueChanged ของ Slider ใน UI ได้เลย
    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        // ปรับความดังของทั้งเกมผ่าน AudioListener (วิธีที่ง่ายที่สุด)
        AudioListener.volume = masterVolume; 
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Set_MasterVol", masterVolume);
    }

    public void LoadSettings()
    {
        // ถ้าเคยเซฟไว้ ให้ดึงค่ามาใช้ ถ้าไม่เคยให้ใช้ค่า 1 (ดังสุด)
        masterVolume = PlayerPrefs.GetFloat("Set_MasterVol", 1f);
        SetMasterVolume(masterVolume);

        // อัปเดต UI ให้ตรงกับค่าที่โหลดมา
        if (volumeSlider != null) 
        {
            volumeSlider.value = masterVolume;
        }
    }
}