using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ทำให้เป็น Singleton (มีแค่ตัวเดียวในเกม)
    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // สั่งให้ Object นี้ (และตัวลูกที่ติดกัน) ไม่ถูกทำลายตอนเปลี่ยนฉาก
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); // ถ้ามีซ้ำให้ลบทิ้ง
        }
    }
}