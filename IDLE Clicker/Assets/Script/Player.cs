using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public int playerMoney = 0;
    public TextMeshProUGUI moneyText;
    public int attackPower = 10; // พลังโจมตีเริ่มต้น
    public int maxHP = 100; // พลังชีวิตเริ่มต้น
    public int currentHP;
    public int criticalChance = 0; // เริ่มต้นที่ 0%
    private int criticalDamageMultiplier = 2; // ดาเมจคริติคอลเป็น 2 เท่าของปกติ
    public float autoAttackInterval = 1f; // เวลาระหว่างการโจมตีอัตโนมัติ
    public int moneyPerClick = 1; // จำนวนเงินที่ได้จากการคลิก

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
        currentHP = maxHP;  // กำหนดค่า HP เริ่มต้น
    }

    private void Start()
    {
        StartAutoAttack(); // เริ่มการโจมตีอัตโนมัติเมื่อเริ่มเกม
    }

    // ฟังก์ชันเริ่มการโจมตีอัตโนมัติ
    private void StartAutoAttack()
    {
        InvokeRepeating("AutoAttackEnemy", 0f, autoAttackInterval); // เรียกฟังก์ชัน AutoAttackEnemy ทุกๆ interval
    }

    // ฟังก์ชันโจมตีอัตโนมัติ
    private void AutoAttackEnemy()
    {
        // หา Enemy ที่อยู่ใกล้ๆ
        Enemy closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            AttackEnemy(closestEnemy);  // โจมตีศัตรูที่ใกล้ที่สุด
            AddMoneyFromAutoAttack();  // เพิ่มเงินจากการโจมตีอัตโนมัติ
        }
    }

    // ฟังก์ชันหาศัตรูที่ใกล้ที่สุด
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

    // ฟังก์ชันเพิ่มเงินจากการโจมตีอัตโนมัติ
    private void AddMoneyFromAutoAttack()
    {
        playerMoney += moneyPerClick;  // เพิ่มเงินจากการโจมตีอัตโนมัติ
        UpdateMoneyText();  // อัปเดต UI
    }

    // ฟังก์ชันเพิ่มเงิน
    public void AddMoney(int amount)
    {
        playerMoney += amount;
        UpdateMoneyText();
    }

    // ฟังก์ชันใช้เงิน
    public bool SpendMoney(int amount)
    {
        if (playerMoney >= amount)
        {
            playerMoney -= amount;
            UpdateMoneyText();
            return true;  // ใช้เงินได้
        }
        else
        {
            return false;  // ถ้าเงินไม่พอ
        }
    }

    // อัปเดต UI ของเงิน
    private void UpdateMoneyText()
    {
        moneyText.text = "Money: " + playerMoney.ToString();
    }

    // ฟังก์ชันเพิ่มเงินจากการคลิก
    public void EarnMoneyFromClick()
    {
        playerMoney += moneyPerClick;  // เพิ่มเงินจากการคลิก
        UpdateMoneyText();
    }

    // ฟังก์ชันอัปเกรดพลังโจมตี
    public void UpgradeAttackPower()
    {
        attackPower += 5;
    }

    // ฟังก์ชันอัปเกรด HP
    public void UpgradeHP()
    {
        maxHP += 10;
        currentHP = maxHP;
    }

    // ฟังก์ชันอัปเกรด Critical Chance
    public void UpgradeCriticalChance()
    {
        if (criticalChance < 100) // ถ้ายังไม่ถึง 100%
        {
            criticalChance += 5;
            UIController.Instance.UpdateUpgradeCostText(); // เรียกฟังก์ชันใน UIController เพื่ออัปเดต UI
        }
        else
        {
            Debug.Log("Critical Chance has reached its maximum limit of 100%.");
        }
    }

    // ฟังก์ชันคำนวณดาเมจ (รวม Critical)
    public int CalculateDamage()
    {
        int damage = attackPower;
        if (Random.Range(0, 100) < criticalChance)  // เช็คว่าเกิด Critical Hit หรือไม่
        {
            damage *= criticalDamageMultiplier; // ดาเมจ * 2 ถ้า Critical
        }
        return damage;
    }

    // ฟังก์ชันรับความเสียหายจาก Enemy
    public void TakeDamage(int amount)
    {
        currentHP -= amount;

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();  // ถ้า HP เหลือ 0 จะตาย
        }
    }

    // ฟังก์ชันโจมตีศัตรู
    public void AttackEnemy(Enemy enemy)
    {
        int damage = attackPower;  // ใช้ค่า attackPower จาก Player
        if (Random.value <= criticalChance / 100f)  // เช็คว่าเป็นการโจมตีคริติคอลไหม
        {
            damage *= 2;  // ดาเมจคริติคอล
            Debug.Log("Critical Hit!");
        }
        enemy.TakeDamage(damage);  // ส่งดาเมจไปยังศัตรู
        Debug.Log("Player attacks Enemy with " + damage + " damage!");
    }

    // ฟังก์ชันตายเมื่อ HP หมด
    private void Die()
    {
        // การจัดการเมื่อ Player ตาย
        Debug.Log("Player is dead");
        // สามารถเพิ่มโค้ดให้ Player ตายที่นี่ เช่น เกมโอเวอร์
    }
}