using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    public GameObject spawnButton;  // ปุ่มที่จะแสดงเมื่อผู้เล่นตาย
    public Spawner spawner;         // เชื่อมต่อกับ Spawner ที่คุณมีอยู่
    public PlayerDeath playerDeath; // อ้างอิงถึงสคริปต์ PlayerDeath

    private bool isEnemySpawned = false;  // เช็คว่าได้สปอนศัตรูหรือยัง

    private void Start()
    {
        // ปุ่มจะถูกซ่อนตอนเริ่มต้น
        spawnButton.SetActive(false);
    }

    // ฟังก์ชันที่เรียกเมื่อผู้เล่นตาย
    public void HandlePlayerDeath()
    {
        // เมื่อผู้เล่นตายให้หยุดเกม
        Time.timeScale = 0f;

        // แสดงปุ่ม
        spawnButton.SetActive(true);
    }

    // ฟังก์ชันเมื่อกดปุ่มเพื่อเริ่มสปอนศัตรู
    public void SpawnEnemy()
    {
        if (isEnemySpawned) return;

        // ซ่อนปุ่มหลังจากกด
        spawnButton.SetActive(false);

        // เริ่มการสปอนศัตรูใหม่
        StartCoroutine(StartEnemySpawn());

        // เริ่มให้เกมทำงานต่อ
        Time.timeScale = 1f;
    }

    private IEnumerator StartEnemySpawn()
    {
        // รอให้การสปอนศัตรูที่มีอยู่เสร็จสิ้นก่อน
        yield return new WaitUntil(() => spawner.currentEnemy == null);

        // เรียกใช้การสปอนศัตรูจาก Spawner
        spawner.StartCoroutine(spawner.SpawnEnemies());

        // ตั้งค่าให้สามารถสปอนศัตรูใหม่ได้หลังจากนั้น
        isEnemySpawned = true;
    }

    // ฟังก์ชันที่เรียกเมื่อศัตรูตัวเก่าตาย
    public void OnEnemyDeath()
    {
        // รีเซ็ตการสปอนศัตรู
        isEnemySpawned = false;

        // เมื่อฆ่าศัตรูตัวเก่าหมด ให้แสดงปุ่ม
        spawnButton.SetActive(true);
    }
}