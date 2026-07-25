using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Efek lampu berkedip untuk Final Loop.
/// Bisa dipakai dengan Light2D (URP) atau dengan mengatur Color sprite lampu.
/// </summary>
public class FlickerLight : MonoBehaviour
{
    [Header("— Mode —")]
    [Tooltip("Pakai sprite color jika tidak pakai URP Light2D")]
    public bool useSpriteMode = true;

    [Header("— Sprite Mode (tanpa URP) —")]
    [Tooltip("SpriteRenderer lampu yang warnanya akan diubah-ubah")]
    public SpriteRenderer lampSprite;
    public Color normalColor = new Color(1f, 0.9f, 0.7f, 1f);
    public Color dimColor    = new Color(0.3f, 0.25f, 0.1f, 1f);
    public Color offColor    = new Color(0.05f, 0.05f, 0.05f, 1f);

    [Header("— Atmosphere Overlay Flicker —")]
    [Tooltip("Overlay layar yang ikut berkedip bersama lampu")]
    public UnityEngine.UI.Image flickerOverlay;
    public Color overlayFlickerColor = new Color(0f, 0f, 0f, 0.3f);

    [Header("— Timing —")]
    public float minInterval = 0.8f;
    public float maxInterval = 3.5f;
    public float minFlickerDur = 0.03f;
    public float maxFlickerDur = 0.15f;
    public int   flickersPerEvent = 3;

    private bool _isFlickering = false;
    private bool _started      = false;

    private void Awake()
    {
        if (flickerOverlay)
        {
            var c = flickerOverlay.color;
            c.a = 0f;
            flickerOverlay.color = c;
            flickerOverlay.gameObject.SetActive(false);
        }
    }

    public void StartFlicker()
    {
        if (_started) return;
        _started = true;
        StartCoroutine(FlickerLoop());
    }

    public void StopFlicker()
    {
        _started = false;
        StopAllCoroutines();

        // Reset ke normal
        if (lampSprite) lampSprite.color = normalColor;
        if (flickerOverlay)
        {
            flickerOverlay.gameObject.SetActive(false);
        }
    }

    private IEnumerator FlickerLoop()
    {
        while (_started)
        {
            // Tunggu interval random
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSecondsRealtime(wait);

            if (!_started) yield break;

            // Jalankan event kedip
            yield return StartCoroutine(FlickerEvent());
        }
    }

    private IEnumerator FlickerEvent()
    {
        _isFlickering = true;

        // Aktifkan overlay
        if (flickerOverlay) flickerOverlay.gameObject.SetActive(true);

        int count = Random.Range(1, flickersPerEvent + 1);

        for (int i = 0; i < count; i++)
        {
            // Lampu mati
            SetLampColor(offColor);
            SetOverlayAlpha(overlayFlickerColor.a);
            yield return new WaitForSecondsRealtime(
                Random.Range(minFlickerDur, maxFlickerDur));

            // Lampu redup sebentar
            SetLampColor(dimColor);
            SetOverlayAlpha(overlayFlickerColor.a * 0.4f);
            yield return new WaitForSecondsRealtime(
                Random.Range(minFlickerDur * 0.5f, maxFlickerDur * 0.5f));

            // Lampu nyala lagi
            SetLampColor(normalColor);
            SetOverlayAlpha(0f);
            yield return new WaitForSecondsRealtime(
                Random.Range(minFlickerDur, maxFlickerDur * 2f));
        }

        // Matikan overlay
        if (flickerOverlay)
        {
            var c = flickerOverlay.color;
            c.a = 0f;
            flickerOverlay.color = c;
            flickerOverlay.gameObject.SetActive(false);
        }

        _isFlickering = false;
    }

    private void SetLampColor(Color color)
    {
        if (lampSprite) lampSprite.color = color;
    }

    private void SetOverlayAlpha(float alpha)
    {
        if (!flickerOverlay) return;
        var c = flickerOverlay.color;
        c.a = alpha;
        flickerOverlay.color = c;
    }
}