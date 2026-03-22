using UnityEngine;

public class SettingController : MonoBehaviour
{
    public enum CameraMode { FPS, Isometric }
    public enum MovementMode { WASD, ClickToMove }

    [Header("Current Settings")]
    public CameraMode currentCamMode = CameraMode.FPS;
    public MovementMode currentMoveMode = MovementMode.WASD;

    [Header("Camera References")]
    public GameObject fpsCamera;
    public GameObject isometricCamera;

    [Header("Movement References")]
    // ลากสคริปต์เดินแบบ WASD มาใส่ (ตัวแปร MonoBehaviour รับได้ทุกสคริปต์)
    public MonoBehaviour wasdMovementScript; 
    // ลากสคริปต์เดินแบบคลิกจอ (NavMesh) มาใส่
    public MonoBehaviour clickMovementScript; 

    // สั่งเปลี่ยนกล้อง
    public void SetCameraMode(CameraMode mode)
    {
        currentCamMode = mode;
        if (mode == CameraMode.FPS)
        {
            if (fpsCamera != null) fpsCamera.SetActive(true);
            if (isometricCamera != null) isometricCamera.SetActive(false);
        }
        else if (mode == CameraMode.Isometric)
        {
            if (fpsCamera != null) fpsCamera.SetActive(false);
            if (isometricCamera != null) isometricCamera.SetActive(true);
        }
    }

    // สั่งเปลี่ยนระบบเดิน
    public void SetMovementMode(MovementMode mode)
    {
        currentMoveMode = mode;
        if (mode == MovementMode.WASD)
        {
            if (wasdMovementScript != null) wasdMovementScript.enabled = true;
            if (clickMovementScript != null) clickMovementScript.enabled = false;
        }
        else if (mode == MovementMode.ClickToMove)
        {
            if (wasdMovementScript != null) wasdMovementScript.enabled = false;
            if (clickMovementScript != null) clickMovementScript.enabled = true;
        }
    }

    public void SaveSettings()
    {
        // บันทึก Enum โดยแปลงเป็นตัวเลข (int)
        PlayerPrefs.SetInt("Set_CamMode", (int)currentCamMode);
        PlayerPrefs.SetInt("Set_MoveMode", (int)currentMoveMode);
    }

    public void LoadSettings()
    {
        // โหลดกลับมาเป็น Enum (ค่า Default คือ 0 หรือแบบแรกสุด)
        currentCamMode = (CameraMode)PlayerPrefs.GetInt("Set_CamMode", 0);
        currentMoveMode = (MovementMode)PlayerPrefs.GetInt("Set_MoveMode", 0);

        // นำค่าที่โหลดมาไปใช้งานจริงทันที
        SetCameraMode(currentCamMode);
        SetMovementMode(currentMoveMode);
    }
    
    // ไว้ใช้ผูกกับปุ่ม UI Dropdown
    public void ChangeCameraFromDropdown(int index)
    {
        SetCameraMode((CameraMode)index);
    }

    public void ChangeMovementFromDropdown(int index)
    {
        SetMovementMode((MovementMode)index);
    }
}