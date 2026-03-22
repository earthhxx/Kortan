using UnityEngine;

public class IsometricCameraFollow : MonoBehaviour
{
    [Header("Target Setting")]
    public Transform target; // ลากตัว Player มาใส่ช่องนี้

    [Header("Camera Follow Setting")]
    public float smoothSpeed = 10f; // ความสมูทตอนกล้องตาม
    private Vector3 offset; // ระยะห่างระหว่างกล้องกับ Player

    void Start()
    {
        if (target != null)
        {
            // ให้กล้องจำระยะห่างจาก Player ณ ตอนเริ่มเกมเอาไว้
            offset = transform.position - target.position;
        }
        else
        {
            Debug.LogWarning("ลืมลาก Player มาใส่ในกล้อง Isometric หรือเปล่า?");
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // คำนวณจุดที่กล้องควรจะไปอยู่
        Vector3 targetPosition = target.position + offset;
        
        // ค่อยๆ เลื่อนกล้องตามไปแบบนุ่มนวล
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}