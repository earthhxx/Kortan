using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameOverManager : MonoBehaviour
{
    public void RestartGame()
    {
        // 1. คืนค่าเวลา (กรณีมีการ Pause เกมไว้ตอนตาย)
        Time.timeScale = 1f;

        TimeManager tm = UnityEngine.Object.FindFirstObjectByType<TimeManager>();
        if (tm != null)
        {
            tm.HardResetTime();
        }
        else
        {
            Debug.LogError("หา TimeManager ไม่เจอ!");
        }

        // 2. จัดการไฟล์ Save (ล้างสถิติเก่าให้เป็นค่าเริ่มต้นใหม่)
        PlayerPrefs.SetFloat("SavedHunger", 100f);
        PlayerPrefs.SetFloat("SavedWater", 100f);
        PlayerPrefs.SetInt("SavedMoney", 0);
        PlayerPrefs.SetFloat("SavedCold", 0f);
        PlayerPrefs.Save();

        // 3. จัดการตัวละคร Player
        // ใช้ FindFirstObjectByType แทน Find เฉยๆ จะเร็วกว่าใน Unity รุ่นใหม่ครับ
        // ระบุไปเลยว่าเป็น UnityEngine.Object
        PlayerStatus player = UnityEngine.Object.FindFirstObjectByType<PlayerStatus>();
        if (player != null)
        {
            player.ResetStatusForNewGame();
        }

        // 4. โหลดฉากใหม่ (เริ่มใหม่ที่ฉากปัจจุบัน)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // 5. ปิดหน้าจอตัวเอง (Panel) - ทำเป็นลำดับสุดท้าย
        gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("ออกจากเกม...");
        Application.Quit();
    }
}