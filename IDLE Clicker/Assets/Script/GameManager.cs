using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(); // ปิดเกม
            Debug.Log("Game Exited"); // แสดงใน Console (ใช้ได้เฉพาะตอนทดสอบใน Editor)
        }
    {
        if (Input.GetKeyDown(KeyCode.F)) // กด F เพื่อสลับโหมดจอ
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }
    }
}