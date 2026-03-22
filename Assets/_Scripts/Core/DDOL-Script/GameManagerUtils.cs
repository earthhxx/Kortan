using UnityEngine;

public static class GameManagerUtils
{
    // ฟังก์ชัน Static เรียกใช้ได้จากทุกที่ เช่น GameManagerUtils.ClearAllDDOL();
    public static void ClearAllDDOL()
    {
        // 1. หาและทำลาย Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Object.Destroy(player);
            Debug.Log("DDOL: Player destroyed");
        }

        // 2. ถ้าคุณมี UI หรือ Manager อื่นๆ ที่เป็น DDOL
        // แนะนำให้ใส่ Tag ไว้ เช่น "GameManager" หรือหาตามชื่อ
        GameObject uiSystem = GameObject.Find("[-- UI_SYSTEM --]"); // ตามชื่อใน Hierarchy ของคุณ
        if (uiSystem != null)
        {
            Object.Destroy(uiSystem);
            Debug.Log("DDOL: UI System destroyed");
        }
        
        // 3. ปลดล็อกเมาส์ให้เป็นค่าพื้นฐาน
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}