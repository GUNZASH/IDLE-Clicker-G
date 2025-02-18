using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public int playerMoney = 0;
    public TextMeshProUGUI moneyText;
    public int attackPower = 10;
    public int maxHP = 100;
    public int currentHP;
    public int criticalChance = 0;
    private int criticalDamageMultiplier = 2;
    public float autoAttackInterval = 1f;
    public int moneyPerClick = 1;

    private bool isFading = false;  // ตัวแปรสำหรับเช็คสถานะการ Fade

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        currentHP = maxHP;
    }

    private void Start()
    {
        StartAutoAttack();
    }

    private void StartAutoAttack()
    {
        InvokeRepeating("AutoAttackEnemy", 0f, autoAttackInterval); // เรียกฟังก์ชัน AutoAttackEnemy ทุกๆ interval
    }

    private void AutoAttackEnemy()
    {
        if (isFading) return;  // ถ้ากำลัง Fade อยู่ จะไม่ทำการโจมตีอัตโนมัติ
        Enemy closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            AttackEnemy(closestEnemy);  // โจมตีศัตรูที่ใกล้ที่สุด
            AddMoneyFromAutoAttack();  // เพิ่มเงินจากการโจมตีอัตโนมัติ
        }
    }

    private Enemy FindClosestEnemy()
    {
        Enemy closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    private void AddMoneyFromAutoAttack()
    {
        playerMoney += moneyPerClick;
        UpdateMoneyText();
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        UpdateMoneyText();
    }

    public bool SpendMoney(int amount)
    {
        if (playerMoney >= amount)
        {
            playerMoney -= amount;
            UpdateMoneyText();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateMoneyText()
    {
        moneyText.text = "Money: " + playerMoney.ToString();
    }

    public void EarnMoneyFromClick()
    {
        playerMoney += moneyPerClick;
        UpdateMoneyText();
    }

    public void UpgradeAttackPower()
    {
        attackPower += 5;
    }

    public void UpgradeHP()
    {
        maxHP += 10;
        currentHP = maxHP;
    }

    public void UpgradeCriticalChance()
    {
        if (criticalChance < 100)
        {
            criticalChance += 5;
            UIController.Instance.UpdateUpgradeCostText();
        }
        else
        {
            Debug.Log("Critical Chance has reached its maximum limit of 100%.");
        }
    }

    public int CalculateDamage()
    {
        int damage = attackPower;
        if (Random.Range(0, 100) < criticalChance)  // เช็คว่าเกิด Critical Hit หรือไม่
        {
            damage *= criticalDamageMultiplier;
        }
        return damage;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    public void AttackEnemy(Enemy enemy)
    {
        if (isFading) return;  // ถ้ากำลัง Fade อยู่ จะไม่ทำการโจมตี
        int damage = attackPower;
        if (Random.value <= criticalChance / 100f)
        {
            damage *= 2;
            Debug.Log("Critical Hit!");
        }
        enemy.TakeDamage(damage);
        Debug.Log("Player attacks Enemy with " + damage + " damage!");
    }

    private void Die()
    {
        Debug.Log("Player is dead");
    }

    // ฟังก์ชันเริ่มการ Fade (จาก FadeIn และ FadeOut)
    public void StartFade()
    {
        isFading = true;
    }

    public void EndFade()
    {
        isFading = false;
    }

    // ฟังก์ชันโจมตีแบบคลิก
    private void Update()
    {
        if (!isFading && Input.GetMouseButtonDown(0))// ตรวจสอบว่าผู้เล่นไม่กำลัง Fade อยู่ และคลิกที่ศัตรู
        {
            AttackOnClick();
        }
    }

    // ฟังก์ชันที่จะให้ผู้เล่นโจมตีเมื่อคลิก
    private void AttackOnClick()
    {
        if (isFading) return;  // ถ้ากำลัง Fade อยู่จะไม่โจมตี
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Enemy"))  // ถ้าคลิกโดนศัตรู
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                AttackEnemy(enemy);  // ทำการโจมตีศัตรูที่ถูกคลิก
            }
        }
    }
}