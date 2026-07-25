using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Efek retakan layar — merepresentasikan kondisi emosi Mono.
/// Loop 3: retakan muncul secara bertahap seiring cerita berlanjut.
/// </summary>
public class CrackEffect : MonoBehaviour
{
    public static CrackEffect Instance;

    [Header("— Crack Sprites —")]
    [Tooltip("Sprite retakan tahap 1 (ringan)")]
    public Sprite crackSprite1;
    [Tooltip("Sprite retakan tahap 2 (sedang)")]
    public Sprite crackSprite2;
    [Tooltip("Sprite retakan tahap 3 (berat — Final Loop)")]
    public Sprite crackSprite3;

    [Header("— UI Image —")]
    [Tooltip("Image component yang menampilkan sprite retakan")]
    public Image crackImage;

    [Header("— Timing —")]
    public float fadeInDuration  = 1.2f;
    public float fadeOutDuration = 0.8f;

    // State
    private int   _currentLevel = 0; // 0 = tidak ada, 1/2/3 = level retakan
    private bool  _isShowing    = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (crackImage)
        {
            crackImage.gameObject.SetActive(false);
            var c = crackImage.color;
            c.a = 0f;
            crackImage.color = c;
        }
    }

    // ════════════════════════════════════════════════════════
    //  PUBLIC API
    // ════════════════════════════════════════════════════════

    /// Tampilkan retakan level 1 (saat masuk rumah Loop 3)
    public void ShowCrackLevel1()
    {
        if (_currentLevel >= 1) return;
        _currentLevel = 1;
        SetCrackSprite(crackSprite1);
        StartCoroutine(FadeInCrack(0.6f));
    }

    /// Tampilkan retakan level 2 (saat dialog branching selesai)
    public void ShowCrackLevel2()
    {
        if (_currentLevel >= 2) return;
        _currentLevel = 2;
        StartCoroutine(TransitionCrack(crackSprite2, 0.8f));
    }

    /// Tampilkan retakan level 3 (Final Loop — rumah retak parah)
    public void ShowCrackLevel3()
    {
        if (_currentLevel >= 3) return;
        _currentLevel = 3;
        StartCoroutine(TransitionCrack(crackSprite3, 1.0f));
    }

    /// Flash retakan sebentar (untuk momen dramatis)
    public void FlashCrack(float targetAlpha = 0.8f, float duration = 0.3f)
    {
        StartCoroutine(FlashRoutine(targetAlpha, duration));
    }

    /// Sembunyikan retakan
    public void HideCrack()
    {
        StartCoroutine(FadeOutCrack());
    }

    // ════════════════════════════════════════════════════════
    //  COROUTINES
    // ════════════════════════════════════════════════════════

    private IEnumerator FadeInCrack(float targetAlpha)
    {
        if (!crackImage) yield break;
        _isShowing = true;
        crackImage.gameObject.SetActive(true);

        var c = crackImage.color;
        c.a = 0f;
        crackImage.color = c;

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, targetAlpha, Mathf.Clamp01(elapsed / fadeInDuration));
            crackImage.color = c;
            yield return null;
        }
        c.a = targetAlpha;
        crackImage.color = c;
    }

    private IEnumerator FadeOutCrack()
    {
        if (!crackImage) yield break;

        var c = crackImage.color;
        float startAlpha = c.a;
        float elapsed    = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(startAlpha, 0f, Mathf.Clamp01(elapsed / fadeOutDuration));
            crackImage.color = c;
            yield return null;
        }
        c.a = 0f;
        crackImage.color = c;
        crackImage.gameObject.SetActive(false);
        _isShowing = false;
    }

    private IEnumerator TransitionCrack(Sprite newSprite, float targetAlpha)
    {
        if (!crackImage) yield break;

        // Fade out sprite lama
        var c = crackImage.color;
        float startAlpha = c.a;
        float elapsed    = 0f;
        float half       = 0.4f;

        while (elapsed < half)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(startAlpha, 0f, Mathf.Clamp01(elapsed / half));
            crackImage.color = c;
            yield return null;
        }

        // Ganti sprite
        if (newSprite) crackImage.sprite = newSprite;

        // Fade in sprite baru
        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, targetAlpha, Mathf.Clamp01(elapsed / half));
            crackImage.color = c;
            yield return null;
        }
        c.a = targetAlpha;
        crackImage.color = c;
    }

    private IEnumerator FlashRoutine(float targetAlpha, float duration)
    {
        if (!crackImage) yield break;

        crackImage.gameObject.SetActive(true);
        var c = crackImage.color;
        float origAlpha = c.a;

        // Flash ke target
        float elapsed = 0f;
        while (elapsed < duration * 0.3f)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(origAlpha, targetAlpha, elapsed / (duration * 0.3f));
            crackImage.color = c;
            yield return null;
        }

        // Kembali ke original
        elapsed = 0f;
        while (elapsed < duration * 0.7f)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(targetAlpha, origAlpha, elapsed / (duration * 0.7f));
            crackImage.color = c;
            yield return null;
        }
        c.a = origAlpha;
        crackImage.color = c;
    }

    private void SetCrackSprite(Sprite sprite)
    {
        if (crackImage && sprite)
            crackImage.sprite = sprite;
    }
}