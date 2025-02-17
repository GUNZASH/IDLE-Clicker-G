using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    // ตัวแปรต่างๆ
    public GameObject upgradePanel;
    public Button upgradeButton;
    public Button upgradeAttackButton;
    public Button upgradeMoneyButton;
    public Button upgradeHPButton;  // ปุ่มอัปเกรด HP
    public Button upgradeCriticalButton; // ปุ่มอัปเกรด Critical Chance
    public TMP_Text moneyText;

    // เพิ่มการแยก Text สำหรับแต่ละประเภท
    public TMP_Text attackUpgradeCostText;  // แสดงราคาการอัปเกรดพลังโจมตี
    public TMP_Text moneyUpgradeCostText;   // แสดงราคาการอัปเกรดเงิน
    public TMP_Text hpUpgradeCostText;      // แสดงราคาการอัปเกรด HP
    public TMP_Text criticalUpgradeCostText; // แสดงราคาการอัปเกรด Critical Chance

    private bool isPanelOpen = false;
    private int moneyPerClick = 1;

    public static UIController Instance;

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

    private void Start()
    {
        upgradePanel.SetActive(false);
        upgradeButton.onClick.AddListener(ToggleUpgradePanel);
        upgradeAttackButton.onClick.AddListener(UpgradeAttackPower);
        upgradeMoneyButton.onClick.AddListener(UpgradeMoney);
        upgradeHPButton.onClick.AddListener(UpgradeHP);
        upgradeCriticalButton.onClick.AddListener(UpgradeCriticalChance);
    }

    private void ToggleUpgradePanel()
    {
        isPanelOpen = !isPanelOpen;
        upgradePanel.SetActive(isPanelOpen);

        if (isPanelOpen)
        {
            RectTransform panelTransform = upgradePanel.GetComponent<RectTransform>();
            panelTransform.anchoredPosition = new Vector2(panelTransform.anchoredPosition.x, -panelTransform.rect.height);
            panelTransform.LeanMoveLocalY(0f, 0.5f);
        }
        else
        {
            RectTransform panelTransform = upgradePanel.GetComponent<RectTransform>();
            panelTransform.LeanMoveLocalY(-panelTransform.rect.height, 0.5f);
        }
    }

    // ฟังก์ชันสำหรับอัปเกรดพลังโจมตี
    private void UpgradeAttackPower()
    {
        int upgradeCost = 10 + Player.Instance.attackPower * 5;
        if (Player.Instance.SpendMoney(upgradeCost))
        {
            Player.Instance.UpgradeAttackPower();
            UpdateUpgradeCostText();
        }
    }

    // ฟังก์ชันสำหรับอัปเกรดเงิน
    private void UpgradeMoney()
    {
        int upgradeCost = 10 + moneyPerClick * 5;
        if (Player.Instance.SpendMoney(upgradeCost))
        {
            moneyPerClick += 1;
            Player.Instance.moneyPerClick = moneyPerClick;  // แก้ไขจำนวนเงินจากการคลิกโดยตรง
            UpdateUpgradeCostText();
        }
    }

    // ฟังก์ชันสำหรับอัปเกรด HP
    private void UpgradeHP()
    {
        int upgradeCost = 20 + Player.Instance.maxHP * 3;
        if (Player.Instance.SpendMoney(upgradeCost))
        {
            Player.Instance.UpgradeHP();
            UpdateUpgradeCostText();
        }
    }

    // ฟังก์ชันสำหรับอัปเกรด Critical Chance
    private void UpgradeCriticalChance()
    {
        int upgradeCost = 15 + Player.Instance.criticalChance * 10;
        if (Player.Instance.SpendMoney(upgradeCost))
        {
            Player.Instance.UpgradeCriticalChance();
            UpdateUpgradeCostText();
        }
    }

    // ฟังก์ชันอัปเดตราคาอัปเกรด (แยกให้แสดงในแต่ละช่อง)
    public void UpdateUpgradeCostText()
    {
        int attackUpgradeCost = 10 + Player.Instance.attackPower * 5;
        int moneyUpgradeCost = 10 + moneyPerClick * 5;
        int hpUpgradeCost = 20 + Player.Instance.maxHP * 3;
        int criticalUpgradeCost = 15 + Player.Instance.criticalChance * 10;

        // อัปเดตราคาแต่ละประเภทให้แสดงใน Text ที่แยกกัน
        attackUpgradeCostText.text = $"Attack Upgrade: {attackUpgradeCost}";
        moneyUpgradeCostText.text = $"Money Upgrade: {moneyUpgradeCost}";
        hpUpgradeCostText.text = $"HP Upgrade: {hpUpgradeCost}";

        // เพิ่มเงื่อนไขที่ไม่ให้เพิ่มราคาถ้าคริติคอลถึง 100%
        if (Player.Instance.criticalChance >= 100)
        {
            criticalUpgradeCostText.text = "Critical Upgrade: MAXED";  // แสดงข้อความว่าเต็ม
        }
        else
        {
            criticalUpgradeCostText.text = $"Critical Upgrade: {criticalUpgradeCost}";
        }
    }

    public bool IsUpgradePanelOpen()
    {
        return isPanelOpen;
    }

    // ฟังก์ชันเพิ่มเงินจากการคลิก
    public void EarnMoneyFromClick()
    {
        Player.Instance.AddMoney(moneyPerClick);  // ใช้ moneyPerClick ที่ถูกอัปเกรด
    }
}