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
    private SpriteRenderer spriteRenderer;

    private bool isDead = false;
    private bool isFading = false;
    private bool canBeAttacked = true; // 🔴 ตัวแปรควบคุมการโจมตี

    // ประกาศ event สำหรับการตายของศัตรู
    public event System.Action onDeath;

    public bool IsDead => isDead; // ✅ ให้ Player เช็คได้
    public bool IsFading => isFading; // ✅ ให้ Player เช็คได้

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        StartCoroutine(AutoAttackPlayer());
    }

    private IEnumerator AutoAttackPlayer()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(attackInterval);
            if (!isFading)  // ตรวจสอบว่าไม่ได้อยู่ในระหว่างการ Fade
            {
                AttackPlayer();
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead || !canBeAttacked) return; // 🔴 หยุดการโจมตีถ้า HP หมด หรือกำลัง Fade Out

        currentHealth -= amount;
        UpdateHealthBar();

        // เริ่มทำ Flash Red แต่เฉพาะตอนที่ไม่กำลังทำ Fade
        if (!isFading)
        {
            FlashRed.Instance.StartFlash(spriteRenderer); // เรียกใช้งาน FlashRed
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        isDead = true;
        canBeAttacked = false; // 🔴 ห้ามโจมตีหลังจากตายแล้ว
        Player.Instance.AddMoney(10);

        // เรียก event เมื่อศัตรูตาย
        onDeath?.Invoke();

        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        isFading = true;
        float fadeDuration = 1.5f;
        float timer = 0f;
        Color color = spriteRenderer.color;

        // เริ่ม Fade out
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        Destroy(gameObject);  // หลังจาก Fade เสร็จ
        isFading = false;  // เปิดใช้งานการโจมตีใหม่
    }

    public void FadeIn()
    {
        if (isFading) return;

        StartCoroutine(FadeInEnemy());
    }

    private IEnumerator FadeInEnemy()
    {
        isFading = true;
        canBeAttacked = false; // 🔴 ห้ามโจมตีก่อน Fade In เสร็จ
        float fadeDuration = 1.5f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            yield return null;
        }

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        isFading = false;
        canBeAttacked = true; // ✅ ศัตรูโจมตีได้หลัง Fade In เสร็จ
    }

    public void AttackPlayer()
    {
        if (isFading) return;  // ถ้ากำลัง Fade อยู่ไม่ให้โจมตี

        int damage = Player.Instance.CalculateDamage();
        Player.Instance.TakeDamage(damage);
    }

    public void EndFade()
    {
        isFading = false;
        canBeAttacked = true; // ✅ เปิดให้ศัตรูถูกโจมตีได้อีกครั้ง
        StartCoroutine(AutoAttackPlayer());
    }

    // ฟังก์ชันรีเซ็ต HP ของศัตรู
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

}