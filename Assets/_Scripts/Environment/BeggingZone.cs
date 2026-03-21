using UnityEngine;
using System.Collections;

public class BeggingZone : MonoBehaviour
{
    // === 1. Static Data (ตั้งค่าใน Inspector) ===
    [Header("=== 1. Spawn Settings ===")]
    [SerializeField] private GameObject moneyPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("=== 2. Timing ===")]
    [SerializeField] private float minWaitTime = 3f;
    [SerializeField] private float maxWaitTime = 10f;

    [Header("=== 3. Spawn Randomization ===")]
    [SerializeField] private float spawnRadius = 0.5f;

    // === 2. Runtime Data (ข้อมูลตอนเล่น) ===
    [System.NonSerialized] private bool isPlayerInZone = false;
    [System.NonSerialized] private Coroutine spawnRoutine;

    // === 3. Properties ===
    public bool IsPlayerInZone => isPlayerInZone;

    // === 4. Lifecycle ===

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            spawnRoutine = StartCoroutine(SpawnMoneyRoutine());
            Debug.Log("<color=yellow>BeggingZone:</color> เริ่มนั่งขอทาน...");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            
            if (spawnRoutine != null)
            {
                StopCoroutine(spawnRoutine);
                spawnRoutine = null;
            }
            
            Debug.Log("<color=yellow>BeggingZone:</color> ลุกจากที่ขอทาน");
        }
    }

    /// <summary>
    /// Spawn money at random intervals while player is in zone
    /// </summary>
    private IEnumerator SpawnMoneyRoutine()
    {
        while (isPlayerInZone)
        {
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            if (isPlayerInZone)
            {
                SpawnMoney();
            }
        }
    }

    /// <summary>
    /// Spawn money at random position near spawn point
    /// </summary>
    private void SpawnMoney()
    {
        if (moneyPrefab == null || spawnPoint == null)
        {
            Debug.LogError("<color=red>BeggingZone:</color> moneyPrefab หรือ spawnPoint ไม่ได้ถูกตั้งค่า!");
            return;
        }

        // Random offset
        float randomX = Random.Range(-spawnRadius, spawnRadius);
        float randomZ = Random.Range(-spawnRadius, spawnRadius);
        Vector3 spawnPos = spawnPoint.position + new Vector3(randomX, 0, randomZ);

        Instantiate(moneyPrefab, spawnPos, Quaternion.identity);
        Debug.Log("<color=yellow>BeggingZone:</color> มีคนโยนเงินให้");
    }
}