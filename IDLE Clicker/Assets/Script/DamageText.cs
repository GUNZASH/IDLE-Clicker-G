using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public float moveSpeed = 1f;
    public float fadeDuration = 1f;

    public void Initialize(int damage, bool isCritical)
    {
        damageText.text = damage.ToString();
        damageText.color = isCritical ? Color.red : Color.white;
        damageText.fontStyle = FontStyles.Bold | FontStyles.Italic; // ตัวหนาและเอียง

        StartCoroutine(FadeAndMove());
    }

    private IEnumerator FadeAndMove()
    {
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, 1, 0);

        Color startColor = damageText.color;
        while (elapsedTime < fadeDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / fadeDuration);
            damageText.color = new Color(startColor.r, startColor.g, startColor.b, 1 - (elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}