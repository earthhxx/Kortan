using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryPanel; // ลาก InventoryPanel มาใส่
    public Transform itemContainer;   // ลาก ItemContainer มาใส่
    public GameObject slotPrefab;     // ลาก ItemSlot (จาก Project/Prefab) มาใส่

    private bool isOpen = false;

    void Start()
    {
        inventoryPanel.SetActive(false); // เริ่มมาให้ปิดไว้ก่อน
        RefreshInventory();
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        // 1. หา PlayerStatus เพื่อเช็กว่าตายหรือยัง
        PlayerStatus player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();

        // 2. ถ้าตายแล้ว (isDead == true) ให้หยุดทำงานตรงนี้เลย ไม่ต้องเช็กปุ่ม I
        if (player != null && player.isDead)
        {
            // ถ้าเผลอเปิดกระเป๋าค้างไว้ตอนตาย ก็ให้ปิดซะ
            if (isOpen) ToggleInventory();
            return;
        }

        // โค้ดกดปุ่ม I เดิม
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        // หาตัวละคร Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        // หาปุ่มควบคุมกล้อง (ปรับชื่อ PlayerMovement หรือ LookCamera ให้ตรงกับของคุณ)
        var movementScript = player.GetComponent<PlayerMovement>();

        if (isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (movementScript != null) movementScript.enabled = false; // หยุดขยับ/หมุนกล้อง
            RefreshInventory();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (movementScript != null) movementScript.enabled = true; // กลับมาขยับได้ปกติ
        }
    }

    public void RefreshInventory()
    {
        foreach (Transform child in itemContainer) Destroy(child.gameObject);

        PlayerStatus player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();

        foreach (ItemData data in player.inventoryList)
        {
            GameObject newSlot = Instantiate(slotPrefab, itemContainer);

            // บรรทัดนี้สำคัญมาก! ส่งข้อมูลไอเทมเข้าสคริปต์ของช่องนั้น
            InventorySlot slotScript = newSlot.GetComponent<InventorySlot>();
            if (slotScript != null)
            {
                slotScript.item = data;
            }

            // เซตความสวยงาม
            var iconImage = newSlot.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>();
            iconImage.sprite = data.icon;
        }
    }
}