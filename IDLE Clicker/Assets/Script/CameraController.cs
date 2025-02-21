using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera; // ✅ ลาก Main Camera ใส่ Inspector
    public Vector3 startPosition = new Vector3(0, -288, -10);  // จุดเริ่มต้นของกล้อง
    public Vector3 targetPosition = new Vector3(0, -299, -10); // จุดที่กล้องจะเลื่อนลงมา
    public float moveSpeed = 2f; // ความเร็วในการเลื่อน

    public GameObject[] uiElementsToHide; // UI ที่ต้องซ่อน (ลากจาก Inspector)
    public TextMeshProUGUI text1; // TMPro ตัวที่ 1
    public TextMeshProUGUI text2; // TMPro ตัวที่ 2

    public static bool isCameraReady = false; // ✅ ตรวจสอบว่ากล้องพร้อมหรือยัง

    private bool isMoving = false;

    private void Start()
    {
        isCameraReady = false; // ✅ กล้องยังไม่พร้อม

        if (mainCamera == null) // ✅ ถ้าไม่ได้ตั้งค่าใน Inspector ให้หาจาก Scene
        {
            mainCamera = Camera.main;
        }

        mainCamera.transform.position = startPosition; // ✅ ใช้ mainCamera ในการกำหนดตำแหน่ง
        Time.timeScale = 0; // หยุดเกมไว้ก่อน

        // ซ่อน UI ทั้งหมด
        foreach (GameObject ui in uiElementsToHide)
        {
            ui.SetActive(false);
        }

        // แสดงแค่ TMPro 2 ตัว
        text1.gameObject.SetActive(true);
        text2.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isMoving && Input.anyKeyDown)
        {
            isMoving = true;
            Time.timeScale = 1; // ให้กล้องเคลื่อนที่ได้ (แต่ยังไม่ให้เกมทำงานเต็มที่)
            StartCoroutine(MoveCamera());
        }
    }

    private IEnumerator MoveCamera()
    {
        while (Vector3.Distance(mainCamera.transform.position, targetPosition) > 0.1f)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, moveSpeed * Time.unscaledDeltaTime);
            yield return null;
        }

        mainCamera.transform.position = targetPosition; // ให้แน่ใจว่าตำแหน่งตรงเป๊ะ

        // ✅ เปิด UI กลับมา
        foreach (GameObject ui in uiElementsToHide)
        {
            ui.SetActive(true);
        }

        // ✅ รีเซ็ตพวก UI ที่ต้องเริ่มทำงานใหม่
        foreach (GameObject ui in uiElementsToHide)
        {
            ui.SetActive(false);
            ui.SetActive(true);
        }

        // ✅ ซ่อน TMPro 2 ตัว
        text1.gameObject.SetActive(false);
        text2.gameObject.SetActive(false);

        isCameraReady = true; // ✅ กล้องพร้อมแล้ว!
    }
}