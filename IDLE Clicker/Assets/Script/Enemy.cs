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

    public GameObject newEnemyPrefab; // Prefab ของศัตรูใหม่

    private bool isDead = false; // ตรวจสอบว่าสิ่งนี้ตายไปแล้วหรือไม่

    // ตัวแปรสำหรับบอส
    public static int enemyKillCount = 0; // นับจำนวนศัตรูที่ถูกฆ่า
    public static int moneyPerClick = 1;  // เงินที่ได้ต่อคลิก
    public static int bossReward = 200;   // เงินที่ได้จากบอส

    public int moneyReward = 0; // จำนวนเงินรางวัลของศัตรู

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        StartCoroutine(AutoAttackPlayer()); // เริ่มการโจมตีอัตโนมัติ
    }

    // การโจมตีอัตโนมัติทุกๆ 3 วินาที
    private IEnumerator AutoAttackPlayer()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(attackInterval);

            // ตรวจสอบว่า upgradePanel ยังเปิดอยู่หรือไม่ ถ้าเปิดอยู่จะยังโจมตี
            Debug.Log("Enemy attacks Player!");
            TakeDamage(damage);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return; // ถ้าตายแล้วไม่ให้ทำอะไร

        currentHealth -= amount;
        UpdateHealthBar();
        StartCoroutine(FlashRed());

        // ให้เงินตามค่าปัจจุบันที่ได้ต่อคลิก
        Player.Instance.AddMoney(moneyPerClick);

        if (currentHealth <= 0 && !isDead)
        {
            if (maxHealth > 1000) // เช็คว่าตัวนี้เป็นบอสหรือไม่ (บอสเลือดเยอะกว่า 1000)
            {
                BossDied();
            }
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

        // เพิ่มตัวนับศัตรูที่ถูกฆ่า
        enemyKillCount++;

        // ตำแหน่งของศัตรูที่ตาย
        Vector3 spawnPosition = transform.position;

        Destroy(gameObject); // ทำลายตัวศัตรูเก่า

        // เช็คว่าฆ่าไปครบ 20 ตัวหรือยัง ถ้าครบให้เกิดบอส
        if (enemyKillCount % 20 == 0)
        {
            SpawnBoss(spawnPosition);
        }
        else
        {
            SpawnNewEnemy(spawnPosition);
        }
    }

    private void SpawnNewEnemy(Vector3 position)
    {
        Debug.Log("Spawning new enemy...");

        GameObject newEnemy = Instantiate(newEnemyPrefab, position, Quaternion.identity);
        Enemy newEnemyScript = newEnemy.GetComponent<Enemy>();

        // เพิ่มค่าพลังของศัตรูใหม่
        newEnemyScript.maxHealth = maxHealth + 10;
        newEnemyScript.currentHealth = newEnemyScript.maxHealth;
        newEnemyScript.healthBar.fillAmount = 1.0f;
        newEnemyScript.damage = damage + 2;
        newEnemyScript.isDead = false;

        // เปิด Collider ถ้ามันถูกปิด
        BoxCollider2D collider = newEnemy.GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        newEnemyScript.enabled = true;

        // สุ่มสีใหม่ (ยกเว้นสีแดง)
        Color randomColor = new Color(Random.value, Random.value, Random.value);
        while (randomColor == Color.red)
        {
            randomColor = new Color(Random.value, Random.value, Random.value);
        }

        newEnemy.GetComponent<SpriteRenderer>().color = randomColor;
        Debug.Log("New enemy spawned with increased stats.");
    }

    private void SpawnBoss(Vector3 position)
    {
        Debug.Log("Spawning BOSS...");

        // สร้างบอส
        GameObject boss = Instantiate(newEnemyPrefab, position, Quaternion.identity);
        Enemy bossScript = boss.GetComponent<Enemy>();

        // ตั้งค่าบอส
        bossScript.maxHealth = 5000; // บอสมีเลือดเยอะ
        bossScript.currentHealth = bossScript.maxHealth;
        bossScript.healthBar.fillAmount = 1.0f;
        bossScript.damage = damage * 2;
        bossScript.moneyReward = bossReward; // กำหนดเงินรางวัล

        // ทำให้บอสมีสีดำ
        boss.GetComponent<SpriteRenderer>().color = Color.black;
        Debug.Log("BOSS spawned with reward: " + bossReward);

        // เปิด BoxCollider2D ของบอส
        BoxCollider2D bossCollider = boss.GetComponent<BoxCollider2D>();
        if (bossCollider != null)
        {
            bossCollider.enabled = true;  // เปิดการใช้งาน BoxCollider
        }
        else
        {
            Debug.LogWarning("No BoxCollider2D found on BOSS!");
        }

        // เปิดการทำงานของ Enemy script ของบอส
        if (bossScript != null)
        {
            bossScript.enabled = true;  // เปิดการทำงานของ Enemy script
        }
        else
        {
            Debug.LogWarning("No Enemy script found on BOSS!");
        }
    }

    private void BossDied()
    {
        Debug.Log("Boss died! Reward: " + bossReward);

        // ให้เงินจากบอส
        Player.Instance.AddMoney(bossReward);

        // เงินที่ได้ต่อคลิกเพิ่มขึ้นเป็น 2
        moneyPerClick = 2;

        // เพิ่มเงินรางวัลบอสครั้งถัดไป
        bossReward += 300;
    }

    private void OnMouseDown()
    {
        if (!UIController.Instance.IsUpgradePanelOpen()) // ถ้า panel เปิดอยู่จะไม่สามารถคลิกได้
        {
            TakeDamage(damage);
        }
    }
}