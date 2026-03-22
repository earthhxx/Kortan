using UnityEngine;

public class SettingManager : MonoBehaviour
{
    [Header("Settings Modules")]
    public SettingSound soundSettings;
    public SettingController controllerSettings;

    void Start()
    {
        // โหลดการตั้งค่าทั้งหมดเมื่อเริ่มเกม
        LoadAllSettings();
    }

    // เรียกใช้ฟังก์ชันนี้เวลากดปุ่ม "Apply" หรือ "Save" ในหน้า UI ตั้งค่า
    public void SaveAllSettings()
    {
        if (soundSettings != null) soundSettings.SaveSettings();
        if (controllerSettings != null) controllerSettings.SaveSettings();
        
        // บังคับเขียนลงระบบของเครื่องทันที
        PlayerPrefs.Save(); 
        Debug.Log("<color=green>Settings:</color> บันทึกการตั้งค่าทั้งหมดเรียบร้อย!");
    }

    public void LoadAllSettings()
    {
        if (soundSettings != null) soundSettings.LoadSettings();
        if (controllerSettings != null) controllerSettings.LoadSettings();
    }
}