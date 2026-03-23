using UnityEngine;

public class SettingController : MonoBehaviour
{
    public enum CameraMode { FirstPerson, Isometric }
    public enum MovementMode { WASD, ClickToMove }

    [Header("Current Settings")]
    public CameraMode currentCamMode = CameraMode.FirstPerson;
    public MovementMode currentMoveMode = MovementMode.WASD;

    [Header("Camera References")]
    public GameObject firstPersonCamera;
    public GameObject isometricCamera;

    [Header("Movement References")]
    // เปลี่ยนจาก MonoBehaviour เป็นชื่อสคริปต์ของเราตรงๆ เลยครับ!
    public PlayerMovement wasdMovementScript;

    // ตรงนี้ผมสมมติว่าสคริปต์เดินคลิกจอคุณชื่อนี้นะครับ ถ้าชื่ออื่นก็เปลี่ยนให้ตรงกัน
    public PlayerClickToMove clickMovementScript;

    // สั่งเปลี่ยนกล้อง
    public void SetCameraMode(CameraMode mode)
    {
        currentCamMode = mode;
        if (mode == CameraMode.FirstPerson)
        {
            if (firstPersonCamera != null) firstPersonCamera.SetActive(true);
            if (isometricCamera != null) isometricCamera.SetActive(false);
        }
        else if (mode == CameraMode.Isometric)
        {
            if (firstPersonCamera != null) firstPersonCamera.SetActive(false);
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
        // ไฮไลต์อยู่ตรงนี้ครับ! เราใช้ (int)currentCamMode เป็นค่า Default แทนเลข 0
        // แปลว่า: "ถ้าหาเซฟไม่เจอ ให้เอาค่าที่ตั้งไว้ในหน้า Inspector มาใช้แทนซะ!"
        currentCamMode = (CameraMode)PlayerPrefs.GetInt("Set_CamMode", (int)currentCamMode);
        currentMoveMode = (MovementMode)PlayerPrefs.GetInt("Set_MoveMode", (int)currentMoveMode);

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