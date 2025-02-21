using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource backgroundMusic; // เพลงพื้นหลัง
    public AudioSource attackSound;     // เสียงโจมตี
    public AudioSource clickSound;      // เสียงคลิก
    public AudioSource deathSound;      // เสียงตาย

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

    public void PlayAttackSound()
    {
        attackSound.Play();
    }

    public void PlayClickSound()
    {
        clickSound.Play();
    }

    public void PlayDeathSound()
    {
        StartCoroutine(HandleDeathSound());
    }

    private IEnumerator HandleDeathSound()
    {
        // หยุดเพลงพื้นหลัง
        backgroundMusic.Pause();

        // เล่นเสียงตาย
        deathSound.Play();

        // รอให้เสียงตายดัง 1 วิ
        yield return new WaitForSeconds(1f);

        // กลับมาเปิดเพลงพื้นหลัง
        backgroundMusic.Play();
    }
}