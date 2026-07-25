using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mengontrol atmosfer visual setiap loop.
/// Loop 0: hangat, normal.
/// Loop 1: sedikit suram, tint biru ringan.
/// Loop 2: lebih gelap, desaturasi lebih kuat.
/// Loop 3: sangat suram, hampir grayscale.
/// </summary>
public class LoopAtmosphere : MonoBehaviour
{
    [Header("— Overlays —")]
    public Image atmosphereOverlay;
    public Image vignetteOverlay;
    public Image glitchOverlay;

    [Header("— Loop 1 —")]
    public Color loop1Tint     = new Color(0.10f, 0.15f, 0.30f, 0.18f);
    public Color loop1Vignette = new Color(0.00f, 0.00f, 0.05f, 0.35f);

    [Header("— Loop 2 —")]
    public Color loop2Tint     = new Color(0.05f, 0.08f, 0.20f, 0.28f);
    public Color loop2Vignette = new Color(0.00f, 0.00f, 0.05f, 0.50f);

    [Header("— Loop 3 —")]
    public Color loop3Tint     = new Color(0.02f, 0.04f, 0.12f, 0.42f);
    public Color loop3Vignette = new Color(0.00f, 0.00f, 0.02f, 0.65f);

    [Header("— Timing —")]
    public float atmosphereFadeInDuration = 2.5f;

    private void Awake()
    {
        SetAlpha(atmosphereOverlay, 0f);
        SetAlpha(vignetteOverlay,   0f);
        SetAlpha(glitchOverlay,     0f);
        if (glitchOverlay) glitchOverlay.gameObject.SetActive(false);
    }

    // ════════════════════════════════════════════════════════
    //  ACTIVATE PER LOOP
    // ════════════════════════════════════════════════════════

    public void ActivateLoop1Atmosphere()
    {
        EnableOverlays();
        StartCoroutine(FadeAtmosphere(loop1Tint, loop1Vignette, atmosphereFadeInDuration));
    }

    public void ActivateLoop2Atmosphere()
    {
        EnableOverlays();
        StartCoroutine(FadeAtmosphere(loop2Tint, loop2Vignette, atmosphereFadeInDuration));
    }

    public void ActivateLoop3Atmosphere()
    {
        EnableOverlays();
        // Loop 3 langsung mulai dari Loop 2 level, lalu intensify
        atmosphereOverlay.color = loop2Tint;
        vignetteOverlay.color   = loop2Vignette;
        StartCoroutine(FadeAtmosphere(loop3Tint, loop3Vignette, atmosphereFadeInDuration * 1.5f));
    }

    public void IntensifyAtmosphere()
    {
        StartCoroutine(IntensifyRoutine());
    }

    // ════════════════════════════════════════════════════════
    //  GLITCH EFFECTS
    // ════════════════════════════════════════════════════════

    public void TriggerMicroGlitch()
    {
        StartCoroutine(MicroGlitchRoutine());
    }

    public void TriggerFullGlitch()
    {
        StartCoroutine(FullGlitchRoutine());
    }

    // ════════════════════════════════════════════════════════
    //  COROUTINES
    // ════════════════════════════════════════════════════════

    private IEnumerator FadeAtmosphere(Color tintTarget, Color vigTarget, float duration)
    {
        float elapsed = 0f;
        Color tintStart = atmosphereOverlay ? atmosphereOverlay.color : Color.clear;
        Color vigStart  = vignetteOverlay   ? vignetteOverlay.color   : Color.clear;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            if (atmosphereOverlay) atmosphereOverlay.color = Color.Lerp(tintStart, tintTarget, t);
            if (vignetteOverlay)   vignetteOverlay.color   = Color.Lerp(vigStart, vigTarget, t);
            yield return null;
        }
        if (atmosphereOverlay) atmosphereOverlay.color = tintTarget;
        if (vignetteOverlay)   vignetteOverlay.color   = vigTarget;
    }

    private IEnumerator IntensifyRoutine()
    {
        if (!atmosphereOverlay) yield break;
        Color current = atmosphereOverlay.color;
        Color target  = new Color(current.r, current.g, current.b,
                                   Mathf.Min(current.a + 0.08f, 0.55f));
        float elapsed = 0f, dur = 3f;
        while (elapsed < dur)
        {
            elapsed += Time.unscaledDeltaTime;
            atmosphereOverlay.color = Color.Lerp(current, target, elapsed / dur);
            yield return null;
        }
    }

    private IEnumerator MicroGlitchRoutine()
    {
        if (!glitchOverlay) yield break;
        glitchOverlay.gameObject.SetActive(true);
        var c = glitchOverlay.color;
        for (int i = 0; i < 2; i++)
        {
            c.a = 0.25f; glitchOverlay.color = c;
            yield return new WaitForSecondsRealtime(0.05f);
            c.a = 0f;    glitchOverlay.color = c;
            yield return new WaitForSecondsRealtime(0.10f);
        }
        glitchOverlay.gameObject.SetActive(false);
    }

    private IEnumerator FullGlitchRoutine()
    {
        if (!glitchOverlay) yield break;
        glitchOverlay.gameObject.SetActive(true);
        var c = glitchOverlay.color;
        for (int i = 0; i < 6; i++)
        {
            c.a = Random.Range(0.15f, 0.45f);
            glitchOverlay.color = c;
            yield return new WaitForSecondsRealtime(Random.Range(0.03f, 0.09f));
            c.a = 0f; glitchOverlay.color = c;
            yield return new WaitForSecondsRealtime(Random.Range(0.04f, 0.12f));
        }
        glitchOverlay.gameObject.SetActive(false);
    }

    private void EnableOverlays()
    {
        if (atmosphereOverlay) atmosphereOverlay.gameObject.SetActive(true);
        if (vignetteOverlay)   vignetteOverlay.gameObject.SetActive(true);
    }

    private void SetAlpha(Image img, float a)
    {
        if (!img) return;
        var c = img.color; c.a = a; img.color = c;
    }
}