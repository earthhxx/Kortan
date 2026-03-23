using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerClickToMove : MonoBehaviour
{
    private NavMeshAgent agent;

    void Start()
    {
        // ดึงคอมโพเนนต์สมอง AI (NavMeshAgent) จากตัว Player มาใช้งาน
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // เช็กว่ามีการคลิกเมาส์ซ้าย (หรือจิ้มจอ) ในเฟรมนี้ไหม
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // ดึงกล้องที่กำลังใช้งานอยู่ (รองรับการสลับกล้องจาก SettingController)
            Camera activeCamera = Camera.main; 
            if (activeCamera == null) return;

            // 1. อ่านพิกัดเมาส์บนหน้าจอ
            Vector2 mousePos = Mouse.current.position.ReadValue();
            
            // 2. ยิงเลเซอร์ (Ray) จากกล้อง พุ่งทะลุเมาส์ลงไปที่ฉาก
            Ray ray = activeCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;

            // 3. ถ้าเลเซอร์ชนกับพื้นฉาก (ที่มี Collider)
            if (Physics.Raycast(ray, out hit))
            {
                // 4. สั่งให้ตัวละครเดินไปที่จุดกระทบนั้นเลย!
                agent.SetDestination(hit.point);
            }
        }
    }
}