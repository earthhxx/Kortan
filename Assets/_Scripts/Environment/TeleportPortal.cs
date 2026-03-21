using UnityEngine;
using UnityEngine.InputSystem; // ต้องมีบรรทัดนี้
using UnityEngine.SceneManagement;

public class TeleportPortal : MonoBehaviour
{
    public string sceneName = "02_MainCity";
    public string spawnPointName;
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
        // 1. จดชื่อจุดเกิดที่ต้องการ (ตั้งค่าชื่อนี้ใน Inspector ของตัว Portal)
        SceneData.TargetSpawnPointName = spawnPointName;
        // 2. โหลดฉาก
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

    }
}