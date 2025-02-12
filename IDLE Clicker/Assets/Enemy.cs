using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public float attackInterval = 3f;
    public int damage = 10;

    public Image healthBar;
    public Color hitColor = new Color(1, 0, 0, 0.5f);
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    public GameObject newEnemyPrefab; // สร้างตัวแปรเพื่อเก็บ prefab ของศัตรูใหม่
    public float spawnDelay = 0.5f; // เวลาในการเกิดศัตรูใหม่

    private bool isDead = false; // ตัวแปรตรวจสอบสถานะการตายของศัตรู

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        StartCoroutine(AttackPlayer());
    }

    private IEnumerator AttackPlayer()
    {
        while (currentHealth > 0)
        {
            yield return new WaitForSeconds(attackInterval);
            Debug.Log("Enemy attacks Player!"); // แทนที่ด้วยโค้ดโจมตี Player จริง ๆ
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return; // ถ้าตายแล้วไม่ให้ทำอะไร
        currentHealth -= amount;
        UpdateHealthBar();
        StartCoroutine(FlashRed());

        if (currentHealth <= 0 && !isDead) // ตรวจสอบว่าศัตรูไม่ตายแล้ว
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = originalColor;
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)currentHealth / maxHealth;
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        isDead = true; // ทำให้สถานะของศัตรูเป็นตายแล้ว
        // ให้ผู้เล่นได้รับเงิน 1000
        Player.Instance.AddMoney(1000); // คุณต้องสร้างสคริปต์ Player ที่มีเมธอด AddMoney()
        Destroy(gameObject); // ทำลายตัวศัตรูเก่า

        // สร้างศัตรูใหม่
        StartCoroutine(SpawnNewEnemy());
    }

    private IEnumerator SpawnNewEnemy()
    {
        Debug.Log("Spawning new enemy...");

        yield return new WaitForSeconds(spawnDelay); // รอ 0.5 วินาที

        // สร้างศัตรูใหม่
        GameObject newEnemy = Instantiate(newEnemyPrefab, transform.position + new Vector3(2, 0, 0), Quaternion.identity); // สร้างที่ตำแหน่งที่ต้องการ
        Debug.Log("New enemy spawned at position: " + newEnemy.transform.position);

        // กำหนดค่าหลอดเลือดให้เป็นค่าสูงสุดเต็ม (healthBar)
        Enemy newEnemyScript = newEnemy.GetComponent<Enemy>();
        newEnemyScript.maxHealth = maxHealth;  // เลือดเท่ากับตัวแรก
        newEnemyScript.currentHealth = maxHealth; // เลือดเท่ากับตัวแรก
        newEnemyScript.healthBar.fillAmount = 1.0f; // ทำให้เลือดเต็ม 100%

        // กำหนดดาเมจของศัตรูใหม่เท่ากับตัวแรก
        newEnemyScript.damage = damage; // ดาเมจเท่ากับตัวแรก
    }

    private void OnMouseDown()
    {
        TakeDamage(damage);
    }
}