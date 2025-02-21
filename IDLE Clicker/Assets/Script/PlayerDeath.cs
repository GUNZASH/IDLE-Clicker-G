using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeath : MonoBehaviour
{
    public static PlayerDeath Instance;
    public GameObject fadePanel;  // กรอบเฟดที่แสดงเมื่อ Player ตาย

    private Player player;
    private Vector3 playerStartPosition;  // ตัวแปรสำหรับเก็บตำแหน่งเริ่มต้นของ Player

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
        player = Player.Instance;
        playerStartPosition = player.transform.position;  // บันทึกตำแหน่งเริ่มต้นของ Player
    }

    public void HandlePlayerDeath()
    {
        // เช็คว่าตายจริงหรือไม่ และยังไม่เคยตั้งค่า isDead มาก่อน
        if (player.currentHP > 0 || player.GetComponent<Animator>().GetBool("isDead"))
        {
            return; // ถ้า Player ยังไม่ตาย หรือ isDead ถูกเซ็ตแล้ว ก็ไม่ต้องทำอะไร
        }

        Debug.Log("Player Die");

        // เล่นอนิเมชั่นตาย
        player.GetComponent<Animator>().SetBool("isDead", true);

        AudioManager.Instance.PlayDeathSound();

        // รอให้อะนิเมชันจบก่อน แล้วค่อยเฟด
        StartCoroutine(WaitForDieAnimation());
    }
    private IEnumerator WaitForDieAnimation()
    {
        Animator animator = player.GetComponent<Animator>();

        // รอจนกว่าจะเริ่มเล่นอนิเมชั่น "DIE"
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("DIE"))
        {
            yield return null;
        }

        // รอจนกว่าอนิเมชั่นจะเล่นครบ 100% (จบ)
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        // หลังจากอนิเมชั่น DIE จบ -> เริ่มการเฟด
        StartCoroutine(FadeOutAndRestart());
    }

    private IEnumerator FadeOutAndRestart()
    {
        // หยุดเกมระหว่างการเฟด
        Time.timeScale = 0f;

        // เฟดมืด
        fadePanel.SetActive(true);  // เปิด Panel ที่จะใช้ในการเฟด
        Color panelColor = fadePanel.GetComponent<Image>().color;
        float timer = 0f;

        // เฟดออก
        while (timer < 1f)
        {
            timer += Time.unscaledDeltaTime*0.5f;  // ใช้ Time.unscaledDeltaTime เพื่อไม่ให้เวลาในเกมมีผล
            float alpha = Mathf.Lerp(0f, 1f, timer);  // ทำให้สีดำเพิ่มขึ้น
            fadePanel.GetComponent<Image>().color = new Color(panelColor.r, panelColor.g, panelColor.b, alpha);
            yield return null;
        }

        // รีเซ็ต Player ตำแหน่งและ HP
        player.transform.position = playerStartPosition;  // ใช้ตำแหน่งที่บันทึกไว้
        player.currentHP = player.maxHP;  // รีเซ็ต HP ให้เต็ม

        player.GetComponent<Animator>().SetBool("isDead", false);

        // รีเซ็ต Enemy HP
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.ResetHealth();  // ฟังก์ชัน ResetHealth ใน Enemy.cs
        }

        // เฟดกลับ
        timer = 0f;
        while (timer < 1f)
        {
            timer += Time.unscaledDeltaTime;  // ใช้ Time.unscaledDeltaTime
            float alpha = Mathf.Lerp(1f, 0f, timer);  // ค่อยๆ ลดการมืด
            fadePanel.GetComponent<Image>().color = new Color(panelColor.r, panelColor.g, panelColor.b, alpha);
            yield return null;
        }

        fadePanel.SetActive(false);  // ซ่อน Panel หลังจากเฟดเสร็จ

        // กลับมาทำงานของเกม
        Time.timeScale = 1f;
    }
}