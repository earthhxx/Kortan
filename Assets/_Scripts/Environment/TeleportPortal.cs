using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.SceneManagement;

public class TeleportPortal : MonoBehaviour
{
    public string sceneName = "02_MainCity";
    public string spawnPointName;
    public GameObject interactUI;

    private bool canTeleport = false;
    private PlayerStatus cachedPlayer;

    void Update()
    {
        // ใช้คำสั่งของ Input System แบบใหม่ในการเช็กปุ่ม E
        if (canTeleport && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // --- เช็กเงื่อนไข: ถ้าเป็นทางออกจาก 7-11 ให้เทตะกร้าทิ้ง ---
            if (spawnPointName == "Exit_From_711" && cachedPlayer != null)
            {
                // ดึง InventorySystem ผ่าน PlayerStatus มาเคลียร์ List ตะกร้า
                cachedPlayer.InventorySystem.CartList.Clear();
                Debug.Log("<color=orange>Shop:</color> เทตะกร้า 7-11 ทิ้งก่อนวาร์ปเรียบร้อย!");
            }

            Enter();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTeleport = true;
            // จำข้อมูล Player เอาไว้ เผื่อต้องใช้เทตะกร้า
            cachedPlayer = other.GetComponent<PlayerStatus>(); 
            
            if (interactUI != null) interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTeleport = false;
            // ล้างข้อมูล Player เมื่อเดินออกนอกวง
            cachedPlayer = null; 
            
            if (interactUI != null) interactUI.SetActive(false);
        }
    }

    void Enter()
    {
        // 1. จดชื่อจุดเกิดที่ต้องการ (ตั้งค่าชื่อนี้ใน Inspector ของตัว Portal)
        SceneData.TargetSpawnPointName = spawnPointName;
        // 2. โหลดฉาก
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}