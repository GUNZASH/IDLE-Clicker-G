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

    private bool isDead = false;
    private Vector3 spawnPosition;  // เก็บตำแหน่งที่ศัตรูเกิด

    private void Start()
    {
        spawnPosition = transform.position;  // เก็บตำแหน่งเดิมของศัตรู
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        StartCoroutine(AutoAttackPlayer());
    }

    private void OnMouseDown()
    {
        if (!UIController.Instance.IsUpgradePanelOpen()) // ถ้า Panel เปิดอยู่จะไม่สามารถคลิกได้
        {
            Player.Instance.AttackEnemy(this);  // เรียกการโจมตีจาก Player
            Player.Instance.EarnMoneyFromClick(); // เพิ่มเงินจากการคลิก
        }
    }

    private IEnumerator AutoAttackPlayer()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(attackInterval);
            AttackPlayer(); // ทำการโจมตีไปยัง Player
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        UpdateHealthBar();
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
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
        isDead = true;
        // มอบเงิน 10 หน่วยให้ผู้เล่นเมื่อศัตรูตาย
        Player.Instance.AddMoney(10);
        // เรียกฟังก์ชัน Respawn เพื่อเกิดใหม่
        Respawn();
    }

    private void Respawn()
    {
        // สร้างศัตรูใหม่ในตำแหน่งเดิม
        transform.position = spawnPosition;
        currentHealth = maxHealth;
        isDead = false;
        UpdateHealthBar();  // รีเซ็ต HealthBar
        StartCoroutine(AutoAttackPlayer()); // เริ่มโจมตีอัตโนมัติอีกครั้ง
    }

    public void AttackPlayer()
    {
        int damage = Player.Instance.CalculateDamage();
        Player.Instance.TakeDamage(damage);
    }
}