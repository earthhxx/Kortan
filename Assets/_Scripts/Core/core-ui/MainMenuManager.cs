using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // สำหรับเริ่มเกมใหม่แบบ "ล้างเซฟ"
    public void StartGameNewGame()
    {
        Debug.Log("<color=cyan>MainMenu:</color> กำลังเริ่มเกมใหม่ (New Game)...");
        Time.timeScale = 1f;

        // 1. ล้างข้อมูล
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("<color=yellow>PlayerPrefs:</color> ล้างข้อมูลเซฟเก่าทั้งหมดเรียบร้อย");

        // 2. เคลียร์ DDOL
        Debug.Log("กำลังเรียก GameManagerUtils.ClearAllDDOL()...");
        GameManagerUtils.ClearAllDDOL();

        // 3. ตั้งค่าเมาส์
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 4. โหลดฉาก
        Debug.Log("<color=green>SceneManager:</color> กำลังโหลดฉาก 02_MainCity...");
        SceneManager.LoadScene("02_MainCity");
    }

    // สำหรับโหลดเกมจากเซฟเดิม
    public void StartGameByLoad()
    {
        Debug.Log("<color=cyan>MainMenu:</color> กำลังโหลดเกมจากเซฟเดิม (Load Game)...");
        Time.timeScale = 1f;

        GameManagerUtils.ClearAllDDOL();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("<color=green>SceneManager:</color> กำลังโหลดฉาก 02_MainCity...");
        SceneManager.LoadScene("02_MainCity");
    }

    public void QuitGame()
    {
        Debug.Log("<color=red>System:</color> ผู้เล่นกดออกจากเกม (Quit)");
        Application.Quit();
    }
}