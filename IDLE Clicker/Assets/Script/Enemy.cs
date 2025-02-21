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
    private Animator animator; // ✅ เพิ่ม Animator

    private bool isDead = false;
    private bool isFading = false;
    private bool canBeAttacked = true; // 🔴 ตัวแปรควบคุมการโจมตี

    public event System.Action onDeath;

    public bool IsDead => isDead;
    public bool IsFading => isFading;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // ✅ ดึง Animator มาใช้
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
            if (!isFading)
            {
                AttackPlayer();
            }
        }
    }

    public void AttackPlayer()
    {
        if (isFading || isDead) return;

        animator.SetBool("isAttack", true); // ✅ เปลี่ยนไป Attack Animation

        StartCoroutine(DealDamageAfterAnimation());
    }

    private IEnumerator DealDamageAfterAnimation()
    {
        yield return new WaitForSeconds(0.5f); // รอให้อนิเมชั่นโจมตีเล่นไป 0.5 วิ

        int damage = Player.Instance.CalculateDamage();
        Player.Instance.TakeDamage(damage);

        yield return new WaitForSeconds(0.5f); // รอให้อนิเมชั่นจบ
        animator.SetBool("isAttack", false); // ✅ กลับไป Idle หลังโจมตี
    }

    public void TakeDamage(int amount)
    {
        if (isDead || !canBeAttacked) return;

        currentHealth -= amount;
        UpdateHealthBar();

        if (!isFading)
        {
            FlashRed.Instance.StartFlash(spriteRenderer);
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
        if (isDead) return;

        isDead = true;
        canBeAttacked = false;

        Player.Instance.AddMoney(10);

        animator.SetBool("isDead", true); // ✅ เปลี่ยนไปเป็น Die Animation

        onDeath?.Invoke();

        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        isFading = true;
        float fadeDuration = 1.5f;
        float timer = 0f;
        Color color = spriteRenderer.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
        isFading = false;
    }

    public void FadeIn()
    {
        if (isFading) return;

        StartCoroutine(FadeInEnemy());
    }

    private IEnumerator FadeInEnemy()
    {
        isFading = true;
        canBeAttacked = false;
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
        canBeAttacked = true;
    }

    public void EndFade()
    {
        isFading = false;
        canBeAttacked = true;
        StartCoroutine(AutoAttackPlayer());
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
}