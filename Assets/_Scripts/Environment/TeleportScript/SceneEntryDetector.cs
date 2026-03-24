using UnityEngine;

public class SceneEntryDetector : MonoBehaviour
{
    void Start()
    {
        MovePlayerToSpawnPoint();
    }

    private void MovePlayerToSpawnPoint()
    {
        // 1. เช็กก่อนว่ามีข้อมูลจุดเกิดฝากไว้ไหม
        if (string.IsNullOrEmpty(SceneData.TargetSpawnPointName)) return;

        // 2. หา Object จุดเกิดในฉากปัจจุบันที่มีชื่อตรงกับที่ฝากไว้
        GameObject spawnPoint = GameObject.Find(SceneData.TargetSpawnPointName);
        
        // 3. หาตัวละครตัวเดิม (ที่ติด DDOL มา)
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (spawnPoint != null && player != null)
        {
            // --- ส่วนสำคัญสำหรับการย้ายตำแหน่งตัวละคร DDOL ---
            
            // ดึง Component ทั้ง 2 ระบบเดินมาเตรียมไว้
            CharacterController cc = player.GetComponent<CharacterController>();
            UnityEngine.AI.NavMeshAgent agent = player.GetComponent<UnityEngine.AI.NavMeshAgent>();

            // 🔴 ปิดระบบเดินทั้งหมดก่อนย้าย! ไม่งั้นพิกัดจะเพี้ยนหรือเดินไม่ได้
            if (cc != null) cc.enabled = false;
            if (agent != null) agent.enabled = false; // <--- เพิ่มตรงนี้เพื่อแก้บั๊กโหมดคลิกเดิน!

            // ย้ายตำแหน่งและมุมหัน
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;

            // 🟢 เปิดกลับคืน (ให้ SettingController มันจัดการต่อ หรือจะเปิดทิ้งไว้ตามโหมดก็ได้)
            if (cc != null) cc.enabled = true;
            if (agent != null) agent.enabled = true;  // <--- เพิ่มตรงนี้เพื่อเปิดสมองกลกลับมา

            Debug.Log($"<color=green>Scene:</color> ย้ายผู้เล่นไปที่จุด {SceneData.TargetSpawnPointName}");
            
            // 4. ล้างค่าทิ้งเพื่อไม่ให้เกิดการวาร์ปซ้ำ
            SceneData.TargetSpawnPointName = null;
        }
    }
}