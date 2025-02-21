using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdjust : MonoBehaviour
{
    public Camera mainCamera;
    public float referenceWidth = 1080f; // ความกว้างของดีไซน์ดั้งเดิม
    public float referenceHeight = 1920f; // ความสูงของดีไซน์ดั้งเดิม
    public float baseOrthographicSize = 5f; // ค่า Size ที่ต้องการใช้เป็นค่าเริ่มต้น

    void Start()
    {
        AdjustCameraSize();
    }

    void AdjustCameraSize()
    {
        float targetAspect = 9f / 16f; // อัตราส่วนจอที่ต้องการ (16:9)
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = screenAspect / targetAspect;

        Camera cam = mainCamera != null ? mainCamera : Camera.main;

        if (scaleHeight < 1.0f)
        {
            cam.orthographicSize = cam.orthographicSize / scaleHeight;
        }
    }
}