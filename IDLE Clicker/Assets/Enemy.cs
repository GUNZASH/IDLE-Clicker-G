using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public float attackInterval = 3f;
    public int damage = 10;
    public int moneyReward = 1000; // จำนวนเงินที่ได้รับเมื่อล้มศัตรู

    public Image healthBar;
    public Color hitColor = new Color(1, 0, 0, 0.5f);
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    public GameObject newEnemyPrefab; // Prefab ของศัตรูใหม่

    private bool isDead = false; // ตรวจสอบว่าสิ่งนี้ตายไปแล้วหรือไม่

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
            Debug.Log("Enemy attacks Player!");
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return; // ถ้าตายแล้ว ไม่ให้ทำอะไรต่อ
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
        isDead = true;

        // ให้ผู้เล่นได้รับเงิน (เพิ่มขึ้นนิดหน่อยในแต่ละครั้ง)
        Player.Instance.AddMoney(moneyReward);
        moneyReward += 200; // เงินเพิ่มขึ้นเรื่อย ๆ ในแต่ละรอบ

        // ตำแหน่งของศัตรูที่ตาย
        Vector3 spawnPosition = transform.position;

        Destroy(gameObject); // ทำลายตัวศัตรูเก่า

        // สร้างศัตรูใหม่ที่ตำแหน่งเดิม
        SpawnNewEnemy(spawnPosition);
    }

    private void SpawnNewEnemy(Vector3 position)
    {
        Debug.Log("Spawning new enemy...");

        // สร้างศัตรูใหม่ที่ตำแหน่งเดิม
        GameObject newEnemy = Instantiate(newEnemyPrefab, position, Quaternion.identity);
        Debug.Log("New enemy spawned at position: " + newEnemy.transform.position);

        // กำหนดค่าของศัตรูใหม่
        Enemy newEnemyScript = newEnemy.GetComponent<Enemy>();
        newEnemyScript.maxHealth = maxHealth + 10;  // เพิ่มเลือดขึ้นเล็กน้อย
        newEnemyScript.currentHealth = newEnemyScript.maxHealth; // ให้เลือดเต็ม
        newEnemyScript.healthBar.fillAmount = 1.0f;
        newEnemyScript.damage = damage + 2; // เพิ่มดาเมจนิดหน่อย
        newEnemyScript.moneyReward = moneyReward; // กำหนดเงินรางวัลใหม่
        newEnemyScript.isDead = false; // รีเซ็ตค่า isDead

        // เปิดใช้งาน BoxCollider2D ถ้ามันถูกปิดอยู่
        BoxCollider2D collider = newEnemy.GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.enabled = true; // เปิด Collider
            Debug.Log("BoxCollider2D enabled for new enemy.");
        }
        else
        {
            Debug.LogWarning("No BoxCollider2D found on new enemy!");
        }

        // เปิดใช้งาน Script Enemy ถ้ามันถูกปิดอยู่
        newEnemyScript.enabled = true;
        Debug.Log("Enemy script enabled for new enemy.");

        // สุ่มสีใหม่ (ยกเว้นสีแดง)
        Color randomColor = new Color(Random.value, Random.value, Random.value);
        while (randomColor == Color.red)
        {
            randomColor = new Color(Random.value, Random.value, Random.value);
        }

        // กำหนดสีใหม่ให้ศัตรู
        SpriteRenderer newEnemySprite = newEnemy.GetComponent<SpriteRenderer>();
        newEnemySprite.color = randomColor;

        Debug.Log("New enemy fully ready with new color and increased stats.");
    }

    private void OnMouseDown()
    {
        TakeDamage(damage);
    }
}