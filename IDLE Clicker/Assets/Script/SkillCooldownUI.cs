using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    public Button skill1Button;
    public Button skill2Button;
    public Button skill3Button;

    public GameObject skill1CooldownOverlay; // 🔥 Image ที่ซ่อน/แสดง
    public GameObject skill2CooldownOverlay;
    public GameObject skill3CooldownOverlay;

    public Player player; // อ้างถึง Player

    private void Start()
    {
        UpdateButtonState(skill1CooldownOverlay, player.IsSkill1Ready);
        UpdateButtonState(skill2CooldownOverlay, player.IsSkill2Ready);
        UpdateButtonState(skill3CooldownOverlay, player.IsSkill3Ready);
    }

    public void StartSkillCooldown(int skillNumber, float cooldownTime)
    {
        GameObject cooldownOverlay = GetSkillCooldownOverlay(skillNumber);
        if (cooldownOverlay == null) return;

        StartCoroutine(CooldownRoutine(cooldownOverlay, cooldownTime));
    }

    private IEnumerator CooldownRoutine(GameObject cooldownOverlay, float cooldownTime)
    {
        Image overlayImage = cooldownOverlay.GetComponent<Image>();
        cooldownOverlay.SetActive(true); // ✅ แสดง Overlay

        float elapsed = 0f;
        while (elapsed < cooldownTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01((cooldownTime - elapsed) / cooldownTime); // ทำให้ค่อยๆ โปร่งใส
            overlayImage.color = new Color(overlayImage.color.r, overlayImage.color.g, overlayImage.color.b, alpha);
            yield return null;
        }

        cooldownOverlay.SetActive(false); // ✅ ซ่อน Overlay เมื่อจางหมด
    }

    private GameObject GetSkillCooldownOverlay(int skillNumber)
    {
        switch (skillNumber)
        {
            case 1: return skill1CooldownOverlay;
            case 2: return skill2CooldownOverlay;
            case 3: return skill3CooldownOverlay;
            default: return null;
        }
    }

    private void UpdateButtonState(GameObject overlay, bool isReady)
    {
        overlay.SetActive(!isReady);
    }
}