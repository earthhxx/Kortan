using UnityEngine;
using UnityEngine.InputSystem; // ต้องมีบรรทัดนี้
using TMPro;

public class TeleportPortal : MonoBehaviour
{
    public string sceneName = "03_7Eleven";
    public GameObject interactUI;
    
    private bool canTeleport = false;

    void Update()
    {
        // ใช้คำสั่งของ Input System แบบใหม่ในการเช็กปุ่ม E
        if (canTeleport && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            EnterShop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTeleport = true;
            if (interactUI != null) interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTeleport = false;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }

    void EnterShop()
    {
        Debug.Log("กำลังเข้าสู่ 7-11...");
        // ขั้นตอนถัดไปเราจะใส่ระบบโหลดฉากจริง
    }
}