using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Image healthBar;  // อ้างอิงไปที่ UI ของ Health Bar ของผู้เล่น
    private Player player;   // อ้างอิงไปที่สคริปต์ Player

    private void Start()
    {
        player = Player.Instance;
        UpdateHealthBar();
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (player != null && healthBar != null)
        {
            float healthPercentage = (float)player.currentHP / player.maxHP;
            healthBar.fillAmount = healthPercentage;
        }
    }
}