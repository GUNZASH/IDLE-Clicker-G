using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // นำเข้า TMPro namespace

public class UIController : MonoBehaviour
{
    public GameObject upgradePanel; // UI Panel
    public Button upgradeButton;    // ปุ่มเปิดปิด UI
    public Button upgradeAttackButton; // ปุ่มอัปเกรดพลังโจมตี
    public Button upgradeMoneyButton;  // ปุ่มอัปเกรดเงิน
    public TMP_Text attackPowerText;    // เปลี่ยนจาก Text เป็น TMP_Text
    public TMP_Text moneyText;          // เปลี่ยนจาก Text เป็น TMP_Text

    private bool isPanelOpen = false;  // ตรวจสอบว่า panel เปิดอยู่หรือไม่
    private int attackPower = 10;      // พลังโจมตีเริ่มต้น
    private int moneyPerClick = 1;    // เงินที่ได้รับต่อคลิกเริ่มต้น

    public static UIController Instance; // เพื่อเข้าถึงจากที่อื่น

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
        // ซ่อน UI Panel ตอนเริ่มต้น
        upgradePanel.SetActive(false);

        // กำหนด Event ให้กับปุ่ม
        upgradeButton.onClick.AddListener(ToggleUpgradePanel);
        upgradeAttackButton.onClick.AddListener(UpgradeAttackPower);
        upgradeMoneyButton.onClick.AddListener(UpgradeMoney);
    }

    private void ToggleUpgradePanel()
    {
        // สลับการเปิด/ปิด UI Panel
        isPanelOpen = !isPanelOpen;
        upgradePanel.SetActive(isPanelOpen);

        if (isPanelOpen)
        {
            // ถ้าเปิด Panel ให้นำ panel มาจากขอบล่างของหน้าจอ
            RectTransform panelTransform = upgradePanel.GetComponent<RectTransform>();
            panelTransform.anchoredPosition = new Vector2(panelTransform.anchoredPosition.x, -panelTransform.rect.height); // ตั้งค่าตำแหน่งเริ่มต้นที่ขอบล่าง
            panelTransform.LeanMoveLocalY(0f, 0.5f); // ใช้ LeanTween ในการเคลื่อน panel
        }
        else
        {
            // ถ้าปิด Panel ให้นำ panel กลับไปที่ขอบล่าง
            RectTransform panelTransform = upgradePanel.GetComponent<RectTransform>();
            panelTransform.LeanMoveLocalY(-panelTransform.rect.height, 0.5f); // ปรับตำแหน่งกลับไปขอบล่าง
        }
    }

    // ฟังก์ชันสำหรับอัปเกรดพลังโจมตี
    private void UpgradeAttackPower()
    {
        int upgradeCost = 10 + attackPower * 5;  // ค่าใช้จ่ายในการอัปเกรดที่เพิ่มขึ้นเรื่อยๆ
        if (Player.Instance.SpendMoney(upgradeCost))
        {
            attackPower += 5;  // เพิ่มพลังโจมตี
            attackPowerText.text = "Attack Power: " + attackPower;  // อัปเดตข้อความ
            moneyPerClick += 1;  // เพิ่มเงินที่ได้ต่อคลิก
            moneyText.text = "Money per Click: " + moneyPerClick;  // อัปเดตข้อความ
            Debug.Log("Attack Power upgraded!");
        }
        else
        {
            Debug.Log("Not enough money to upgrade attack power!");
        }
    }

    // ฟังก์ชันสำหรับอัปเกรดเงิน
    private void UpgradeMoney()
    {
        int upgradeCost = 10 + moneyPerClick * 5;  // ค่าใช้จ่ายในการอัปเกรดที่เพิ่มขึ้นเรื่อยๆ
        if (Player.Instance.SpendMoney(upgradeCost))
        {
            moneyPerClick += 1;  // เพิ่มเงินที่ได้รับต่อคลิก
            moneyText.text = "Money per Click: " + moneyPerClick;  // อัปเดตข้อความ
            Debug.Log("Money per click upgraded!");
        }
        else
        {
            Debug.Log("Not enough money to upgrade money per click!");
        }
    }

    // ฟังก์ชันอัปเดตค่าเงิน (ถ้ามี)
    public void UpdatePlayerStats(int money)
    {
        moneyText.text = "Money per Click: " + moneyPerClick;  // อัปเดตข้อความ
    }

    public bool IsUpgradePanelOpen()
    {
        return isPanelOpen; // คืนค่าการเปิดหรือปิดของ Panel
    }
}