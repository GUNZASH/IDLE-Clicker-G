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

    private bool isFading = false;


    // คูลดาวน์ของสกิล
    public float skill1Cooldown = 40f;
    public float skill2Cooldown = 30f;
    public float skill3Cooldown = 20f; // ตั้งค่าเริ่มต้นได้

    private bool skill1Ready = true;
    private bool skill2Ready = true;
    private bool skill3Ready = true;
    private bool isReflectingDamage = false;

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
        moneyPerClick = 1;
    }

    private void Start()
    {
        StartAutoAttack();
    }

    private void StartAutoAttack()
    {
        InvokeRepeating("AutoAttackEnemy", 0f, autoAttackInterval);
    }

    private void AutoAttackEnemy()
    {
        if (isFading) return;
        Enemy closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            AttackEnemy(closestEnemy);
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

    private void AddMoneyFromClick()
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
        moneyText.text = " " + playerMoney.ToString();
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
        if (Random.Range(0, 100) < criticalChance)
        {
            damage *= criticalDamageMultiplier;
        }
        return damage;
    }

    public void TakeDamage(int amount)
    {
        if (isReflectingDamage)
        {
            Enemy closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                closestEnemy.TakeDamage(amount);
                Debug.Log("Reflected " + amount + " damage back to the enemy!");
            }
        }

        currentHP -= amount;

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    public void AttackEnemy(Enemy enemy)
    {
        if (isFading) return;
        int damage = attackPower;
        if (Random.value <= criticalChance / 100f)
        {
            damage *= 2;
            Debug.Log("Critical Hit!");
        }
        enemy.TakeDamage(damage);
        Debug.Log("Player attacks Enemy with " + damage + " damage!");

        AddMoneyFromClick();
    }

    private void Die()
    {
        Debug.Log("Player is dead");
        PlayerDeath.Instance.HandlePlayerDeath();
    }

    public void StartFade()
    {
        isFading = true;
    }

    public void EndFade()
    {
        isFading = false;
    }

    private void Update()
    {
        if (!isFading && Input.GetMouseButtonDown(0))
        {
            AttackOnClick();
        }
    }

    private void AttackOnClick()
    {
        if (isFading || UIController.Instance.IsUpgradePanelOpen()) return; // เช็คว่าเปิด Panel หรือยัง

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                AttackEnemy(enemy);
            }
        }
    }

    // --------------- ระบบสกิล -----------------

    public void UseSkill1() // สกิลโจมตีแรง 5 เท่า
    {
        if (!skill1Ready) return;

        Enemy closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            int damage = attackPower * 5;
            closestEnemy.TakeDamage(damage);
            Debug.Log("Skill 1 activated! Dealt " + damage + " damage.");
            StartCoroutine(SkillCooldown(1, skill1Cooldown));
        }
    }

    public void UseSkill2() // สกิลฮีล 20% ของ Max HP
    {
        if (!skill2Ready) return;

        int healAmount = Mathf.RoundToInt(maxHP * 0.2f);
        currentHP += healAmount;
        if (currentHP > maxHP) currentHP = maxHP;

        Debug.Log("Skill 2 activated! Healed " + healAmount + " HP.");
        StartCoroutine(SkillCooldown(2, skill2Cooldown));
    }

    public void UseSkill3() // สกิลสะท้อนดาเมจ
    {
        if (!skill3Ready) return;

        isReflectingDamage = true;
        Debug.Log("Skill 3 activated! Reflecting damage for 5 seconds.");
        StartCoroutine(SkillCooldown(3, skill3Cooldown));
        StartCoroutine(DisableReflect(5f)); // สะท้อน 5 วิ
    }

    private IEnumerator SkillCooldown(int skillNumber, float cooldownTime)
    {
        switch (skillNumber)
        {
            case 1: skill1Ready = false; break;
            case 2: skill2Ready = false; break;
            case 3: skill3Ready = false; break;
        }

        yield return new WaitForSeconds(cooldownTime);

        switch (skillNumber)
        {
            case 1: skill1Ready = true; break;
            case 2: skill2Ready = true; break;
            case 3: skill3Ready = true; break;
        }
    }

    private IEnumerator DisableReflect(float duration)
    {
        yield return new WaitForSeconds(duration);
        isReflectingDamage = false;
        Debug.Log("Reflection effect ended.");
    }
}