using UnityEngine;
using System.Collections;

public class BeggingZone : MonoBehaviour
{
    [Header("Settings")]
    public GameObject moneyPrefab; // ลาก Prefab เหรียญมาใส่ที่นี่
    public Transform spawnPoint;   // ลาก SpawnPoint มาใส่ที่นี่
    public float minWait = 3f;     // สุ่มรออย่างน้อย 3 วิ
    public float maxWait = 10f;    // สุ่มรอไม่เกิน 10 วิ

    private bool isPlayerInZone = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            Debug.Log("เริ่มนั่งขอทาน...");
            StartCoroutine(SpawnMoneyRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            Debug.Log("ลุกจากที่ขอทาน");
            StopAllCoroutines(); // หยุดสุ่มเงินทันทีที่ลุก
        }
    }

    IEnumerator SpawnMoneyRoutine()
    {
        while (isPlayerInZone)
        {
            float waitTime = Random.Range(minWait, maxWait);
            yield return new WaitForSeconds(waitTime);

            if (isPlayerInZone && moneyPrefab != null && spawnPoint != null)
            {
                // --- เพิ่มส่วนการสุ่มตำแหน่ง (Random Offset) ---
                // สุ่มค่า X และ Z รอบๆ จุดเกิดในรัศมี 0.5 หน่วย
                float randomX = Random.Range(-0.5f, 0.5f);
                float randomZ = Random.Range(-0.5f, 0.5f);
                Vector3 spawnPos = spawnPoint.position + new Vector3(randomX, 0, randomZ);

                // เสกเหรียญตามตำแหน่งที่สุ่มได้
                Instantiate(moneyPrefab, spawnPos, Quaternion.identity);
                // -------------------------------------------

                Debug.Log("มีคนโยนเงินให้แล้ว!");
            }
        }
    }
}