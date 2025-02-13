using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public int playerMoney = 0;
    public TextMeshProUGUI moneyText; // ใช้ TextMeshProUGUI แทน Text
    public int attackPower = 10; // พลังโจมตีเริ่มต้น

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
            Debug.Log("Money spent: " + amount); // แจ้งเตือนเมื่อใช้เงิน
            return true;  // ใช้เงินได้
        }
        else
        {
            Debug.Log("Not enough money for this upgrade!"); // แจ้งเตือนถ้าเงินไม่พอ
            return false;  // ถ้าเงินไม่พอ
        }
    }

    // อัปเดต UI ของเงิน
    private void UpdateMoneyText()
    {
        moneyText.text = "Money: " + playerMoney.ToString();
    }

    // ฟังก์ชันอัปเกรดพลังโจมตี
    public void UpgradeAttackPower()
    {
        attackPower += 5;  // เพิ่มพลังโจมตี
        Debug.Log("Attack Power upgraded to: " + attackPower);
    }
}