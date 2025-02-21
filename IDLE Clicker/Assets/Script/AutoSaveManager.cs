using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSaveManager : MonoBehaviour
{
    public Player player; // เชื่อมกับ Player (ลากจาก Inspector)

    private void Start()
    {
        LoadData();
        CalculateOfflineEarnings();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void SaveData()
    {
        if (player == null) return;

        PlayerPrefs.SetInt("playerMoney", player.playerMoney);
        PlayerPrefs.SetInt("moneyPerClick", player.moneyPerClick);
        PlayerPrefs.SetString("lastLogoutTime", DateTime.UtcNow.ToString()); // บันทึกเวลาล่าสุดที่ออกเกม
        PlayerPrefs.Save();

        Debug.Log("✅ เกมถูกบันทึกเรียบร้อยแล้ว!");
    }

    private void LoadData()
    {
        if (player == null) return;

        if (PlayerPrefs.HasKey("playerMoney"))
        {
            player.playerMoney = PlayerPrefs.GetInt("playerMoney");
        }

        if (PlayerPrefs.HasKey("moneyPerClick"))
        {
            player.moneyPerClick = PlayerPrefs.GetInt("moneyPerClick");
        }
    }

    private void CalculateOfflineEarnings()
    {
        if (player == null || !PlayerPrefs.HasKey("lastLogoutTime")) return;

        string lastLogoutStr = PlayerPrefs.GetString("lastLogoutTime");
        DateTime lastLogoutTime;

        if (DateTime.TryParse(lastLogoutStr, out lastLogoutTime))
        {
            TimeSpan timePassed = DateTime.UtcNow - lastLogoutTime;
            int secondsPassed = Mathf.Clamp((int)timePassed.TotalSeconds, 0, 21600); // สูงสุด 6 ชั่วโมง
            int offlineEarnings = secondsPassed * player.moneyPerClick;

            player.playerMoney += offlineEarnings;
            Debug.Log($"💰 ได้รับเงินจากการฟาร์มออฟไลน์: {offlineEarnings} (เวลาผ่านไป {secondsPassed} วิ)");
        }
    }
}