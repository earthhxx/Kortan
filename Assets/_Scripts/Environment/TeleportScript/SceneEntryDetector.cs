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
            
            // ถ้ามี CharacterController ต้องปิดก่อนย้าย ไม่งั้นตำแหน่งจะเพี้ยนหรือกระตุกกลับ
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // ย้ายตำแหน่งและมุมหัน
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;

            // เปิดกลับคืน
            if (cc != null) cc.enabled = true;

            Debug.Log($"<color=green>Scene:</color> ย้ายผู้เล่นไปที่จุด {SceneData.TargetSpawnPointName}");
            
            // 4. ล้างค่าทิ้งเพื่อไม่ให้เกิดการวาร์ปซ้ำถ้าโหลดฉากเดิมอีกครั้งโดยไม่ได้ผ่าน Portal
            SceneData.TargetSpawnPointName = null;
        }
    }
}