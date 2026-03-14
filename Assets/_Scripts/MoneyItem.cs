using UnityEngine;

public class MoneyItem : MonoBehaviour
{
    public int amount = 5;
    public float lifeTime = 20f; // ตั้งเวลาให้เหรียญอยู่ในเกมได้ 20 วินาที

    void Start()
    {
        // สั่งให้ทำลายตัวเองทิ้งเมื่อเวลาผ่านไป lifeTime
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus status = other.GetComponent<PlayerStatus>();
            if (status != null)
            {
                status.AddMoney(amount);
            }
            Destroy(gameObject); // เก็บแล้วหายไปทันที (ไม่ต้องรอจนครบ lifeTime)
        }
    }
}