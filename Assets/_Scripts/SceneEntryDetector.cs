using UnityEngine;

public class SceneEntryDetector : MonoBehaviour
{
    public Vector3 spawnPosition; // ตั้งค่าพิกัดที่จะให้โผล่ใน Inspector
    public Vector3 spawnRotation; // ตั้งค่ามุมหันหน้า

    void Start()
    {
        // หาตัว Player ที่วาร์ปมา
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            // ย้ายตำแหน่งตัวละครทันทีที่เริ่มฉากใหม่
            player.transform.position = spawnPosition;
            player.transform.eulerAngles = spawnRotation;
            Debug.Log("ย้าย Player ไปจุดเกิดใหม่เรียบร้อย!");
        }
    }
}