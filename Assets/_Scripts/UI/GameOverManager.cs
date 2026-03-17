using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void RestartGame()
    {
        // 1. คืนค่าเวลา
        Time.timeScale = 1f;

        // 2. ปิดหน้าจอตัวเอง (Panel)
        gameObject.SetActive(false);

        // 3. จัดการไฟล์ Save
        PlayerPrefs.SetFloat("SavedHunger", 100f);
        PlayerPrefs.SetFloat("SavedWater", 100f);
        PlayerPrefs.SetInt("SavedMoney", 0);
        PlayerPrefs.Save();

        // 4. สั่งให้ Player กลับมาเป็นปกติ
        PlayerStatus player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
        if (player != null)
        {
            player.ResetStatusForNewGame(); // เรียกฟังก์ชันเปิดการเดินและรีเซ็ตค่า
        }

        // 5. โหลดฉากใหม่
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("ออกจากเกม...");
        Application.Quit();
    }
}