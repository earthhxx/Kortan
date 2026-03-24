using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerClickToMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private Camera mainCam;

    [Header("ตั้งค่าเลเซอร์")]
    [Tooltip("เลือก Layer พื้นดิน เลเซอร์จะทะลุตึกไปโดนพื้นแทน")]
    public LayerMask groundLayer;

    [Header("ตั้งค่าการเดินค้าง")]
    [Tooltip("ระยะห่างที่ต้องขยับเมาส์ ก่อนจะสั่งคำนวณเส้นทางใหม่")]
    public float updateThreshold = 0.5f; // เปลี่ยนแปลงได้ตามความลื่นไหลที่ชอบ
    private Vector3 lastDestination;     // เก็บพิกัดเป้าหมายล่าสุด

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!enabled) return;

        bool isClickThisFrame = false;
        bool isHeld = false;
        Vector2 screenPos = Vector2.zero;

        // --- 1. เช็กเมาส์ (PC) ---
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame) isClickThisFrame = true; // เพิ่งกดเฟรมแรก
            if (Mouse.current.leftButton.isPressed) isHeld = true;                     // กดค้างอยู่
            screenPos = Mouse.current.position.ReadValue();
        }

        // --- 2. เช็กการทัช (Mobile) ---
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame) isClickThisFrame = true;
            isHeld = true;
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
        }

        // --- 3. สั่งทำงาน ---
        if (isClickThisFrame || isHeld)
        {
            // ส่งค่า isClickThisFrame ไปเพื่อบอกว่าเป็นการกดครั้งแรกหรือกดค้าง
            MoveToTarget(screenPos, isClickThisFrame);
        }
    }

    void MoveToTarget(Vector2 screenPosition, bool forceUpdate)
    {
        mainCam = Camera.main;
        if (mainCam == null) return;

        Ray ray = mainCam.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        // ยิงเลเซอร์หาพื้นดิน
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            // 💡 พระเอกของเราอยู่ตรงนี้ครับ!
            // ถ้าเป็นการ "กดคลิกครั้งแรก" (forceUpdate) 
            // หรือ "กดค้าง แต่ลากเมาส์ห่างจากเป้าหมายเดิมเกิน 0.5 เมตร" (อัปเดตเป้าหมายใหม่)
            if (forceUpdate || Vector3.Distance(hit.point, lastDestination) > updateThreshold)
            {
                agent.SetDestination(hit.point);
                lastDestination = hit.point; // จำไว้ว่าสั่งไปที่พิกัดนี้แล้ว
            }
        }
    }
}