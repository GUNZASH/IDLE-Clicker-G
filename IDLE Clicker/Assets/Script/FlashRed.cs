using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashRed : MonoBehaviour
{
    public static FlashRed Instance { get; private set; } // Singleton pattern
    public Color hitColor = new Color(1, 0, 0, 0.5f); // สีที่ใช้กระพริบ
    private Color originalColor;

    private bool isFlashing = false; // ตัวแปรเช็คสถานะการกระพริบ

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // ทำลายถ้ามี instance อื่นอยู่แล้ว
        }
        else
        {
            Instance = this; // ตั้งค่าให้เป็น instance เดียว
        }
    }

    public void StartFlash(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer != null && !isFlashing)
        {
            isFlashing = true; // กำหนดสถานะว่ากำลังกระพริบ
            originalColor = spriteRenderer.color; // เก็บสีเดิม
            StartCoroutine(Flash(spriteRenderer));
        }
    }

    private IEnumerator Flash(SpriteRenderer spriteRenderer)
    {
        spriteRenderer.color = hitColor; // ตั้งสีเป็นสีแดง
        yield return new WaitForSeconds(0.2f); // กระพริบให้แสดงแค่ 0.2 วินาที
        spriteRenderer.color = originalColor; // กลับสีเดิม
        isFlashing = false; // กำหนดสถานะว่าไม่กระพริบแล้ว
    }
}