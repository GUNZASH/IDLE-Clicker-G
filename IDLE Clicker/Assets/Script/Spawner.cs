using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; private set; }

    public Transform[] spawnPoints; // จุดเกิดศัตรู
    public GameObject[] enemyPrefabs; // ศัตรูที่ต้องการ Spawn

    private int currentSpawnIndex = 0; // ลำดับของศัตรูที่เกิด
    public GameObject currentEnemy; // ศัตรูปัจจุบัน

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(WaitForCameraAndSpawn()); // ✅ เพิ่มเงื่อนไขให้รอจนกว่ากล้องพร้อม
    }

    private IEnumerator WaitForCameraAndSpawn()
    {
        // ✅ รอจนกว่ากล้องจะเลื่อนเสร็จ
        yield return new WaitUntil(() => CameraController.isCameraReady);

        // ✅ เริ่ม Spawn ศัตรูหลังจากกล้องพร้อม
        StartCoroutine(SpawnEnemies());
    }

    public IEnumerator SpawnEnemies()
    {
        while (currentSpawnIndex < enemyPrefabs.Length)
        {
            // รอให้ศัตรูตัวเก่าตายและทำ Fade Out เสร็จ
            yield return new WaitUntil(() => currentEnemy == null);

            // สปอนด์ศัตรูใหม่
            SpawnEnemy();
            StartCoroutine(FadeInEnemy());

            // รอให้ศัตรูตัวเก่าตายก่อนที่จะเริ่มสปอนด์ตัวใหม่
            while (currentEnemy != null)
            {
                yield return null; // รอจนกว่าศัตรูจะถูกทำลาย
            }

            yield return new WaitForSeconds(1f); // รอให้ Fade Out เสร็จสิ้น
        }

        Debug.Log("Spawn Completed!");
    }

    private void SpawnEnemy()
    {
        if (currentSpawnIndex >= enemyPrefabs.Length) return;

        int spawnPointIndex = currentSpawnIndex % spawnPoints.Length;
        Transform spawnPoint = spawnPoints[spawnPointIndex];

        Debug.Log($"Spawning Enemy: {enemyPrefabs[currentSpawnIndex].name} at {spawnPoint.position} (Index: {currentSpawnIndex})"); // ✅ ตรวจสอบการ Spawn

        currentEnemy = Instantiate(enemyPrefabs[currentSpawnIndex], spawnPoint.position, Quaternion.identity);

        SpriteRenderer enemySprite = currentEnemy.GetComponent<SpriteRenderer>();
        Color originalColor = enemySprite.color;
        enemySprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        Enemy enemyScript = currentEnemy.GetComponent<Enemy>();
        enemyScript.onDeath += OnEnemyDeath;

        currentSpawnIndex++; // ✅ เพิ่มค่าตามลำดับ
    }

    private IEnumerator FadeInEnemy()
    {
        float fadeDuration = 1.5f;
        float timer = 0f;
        SpriteRenderer enemySprite = currentEnemy.GetComponent<SpriteRenderer>();

        // เก็บสีเดิมจาก Prefab
        Color originalColor = enemySprite.color;

        // ทำการ Fade In โดยไม่เปลี่ยนค่า R, G, B เพียงแค่ปรับค่า Alpha
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            enemySprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha); // ตั้งค่าค่า alpha เท่านั้น
            yield return null;
        }

        // ทำให้สีเป็นสีปกติเมื่อ Fade In เสร็จ
        enemySprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);
    }

    private void OnEnemyDeath()
    {
        currentEnemy = null; // ตั้งค่าให้ตัวปัจจุบันเป็น null เพื่อให้ศัตรูใหม่เกิดขึ้นได้
    }
}