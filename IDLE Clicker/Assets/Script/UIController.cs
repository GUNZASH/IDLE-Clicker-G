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

    public GameObject skillButtons;  // ตัวแปรเก็บกลุ่มปุ่มสกิล
    public GameObject playerHealthBar; // ตัวแปรเก็บ UI หลอดเลือด

    // ประกาศตัวแปรราคาการอัปเกรด
    private int attackUpgradeCost = 150;
    private int moneyUpgradeCost = 50;
    private int hpUpgradeCost = 150;
    private int criticalUpgradeCost = 300;

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

        UpdateUpgradeCostText();
    }

    private void ToggleUpgradePanel()
    {
        isPanelOpen = !isPanelOpen;
        upgradePanel.SetActive(isPanelOpen);

        RectTransform panelTransform = upgradePanel.GetComponent<RectTransform>();

        if (isPanelOpen)
        {
            panelTransform.anchoredPosition = new Vector2(panelTransform.anchoredPosition.x, -panelTransform.rect.height);
            panelTransform.LeanMoveLocalY(0f, 0.5f);

            // 🔴 ปิดปุ่มสกิลและหลอดเลือดตอนเปิด Panel
            skillButtons.SetActive(false);
            playerHealthBar.SetActive(false);
        }
        else
        {
            panelTransform.LeanMoveLocalY(-panelTransform.rect.height, 0.5f);

            // 🟢 เปิดปุ่มสกิลและหลอดเลือดกลับมาตอนปิด Panel
            skillButtons.SetActive(true);
            playerHealthBar.SetActive(true);
        }
    }

    // ฟังก์ชันสำหรับอัปเกรดพลังโจมตี
    private void UpgradeAttackPower()
    {
        if (Player.Instance.SpendMoney(attackUpgradeCost))
        {
            Player.Instance.UpgradeAttackPower();
            attackUpgradeCost += 100;  // เพิ่มราคาหลังอัปเกรด
            UpdateUpgradeCostText();
        }
    }

    // ฟังก์ชันสำหรับอัปเกรดเงิน
    private void UpgradeMoney()
    {
        if (Player.Instance.SpendMoney(moneyUpgradeCost))
        {
            moneyPerClick += 1;
            Player.Instance.moneyPerClick = moneyPerClick;
            moneyUpgradeCost += 50;  // เพิ่มราคาหลังอัปเกรด
            UpdateUpgradeCostText();
        }
    }

    // ฟังก์ชันสำหรับอัปเกรด HP
    private void UpgradeHP()
    {
        if (Player.Instance.SpendMoney(hpUpgradeCost))
        {
            Player.Instance.UpgradeHP();
            hpUpgradeCost += 100;  // เพิ่มราคาหลังอัปเกรด
            UpdateUpgradeCostText();
        }
    }

    // ฟังก์ชันสำหรับอัปเกรด Critical Chance
    private void UpgradeCriticalChance()
    {
        if (Player.Instance.SpendMoney(criticalUpgradeCost))
        {
            Player.Instance.UpgradeCriticalChance();
            criticalUpgradeCost += 350;  // เพิ่มราคาหลังอัปเกรด
            UpdateUpgradeCostText();
        }
    }

    // ฟังก์ชันอัปเดตราคาอัปเกรด (แยกให้แสดงในแต่ละช่อง)
    public void UpdateUpgradeCostText()
    {
        // อัปเดตราคาแต่ละประเภทให้แสดงใน Text ที่แยกกัน
        attackUpgradeCostText.text = $"Cost: {attackUpgradeCost}";
        moneyUpgradeCostText.text = $"Cost: {moneyUpgradeCost}";
        hpUpgradeCostText.text = $"Cost: {hpUpgradeCost}";

        // เพิ่มเงื่อนไขที่ไม่ให้เพิ่มราคาถ้าคริติคอลถึง 100%
        if (Player.Instance.criticalChance >= 100)
        {
            criticalUpgradeCostText.text = "MAXED";  // แสดงข้อความว่าเต็ม
        }
        else
        {
            criticalUpgradeCostText.text = $"Cost: {criticalUpgradeCost}";
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