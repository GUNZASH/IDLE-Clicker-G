using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public int playerMoney = 0;
    public TextMeshProUGUI moneyText; // ใช้ TextMeshProUGUI แทน Text

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

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        moneyText.text = " " + playerMoney.ToString();
    }
}