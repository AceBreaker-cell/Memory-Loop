using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manager untuk sistem inspect objek.
/// Saat di-inspect: layar sedikit gelap, muncul panel kertas dengan teks.
/// </summary>
public class InspectManager : MonoBehaviour
{
    public static InspectManager Instance;

    [Header("— UI References —")]
    public GameObject      inspectOverlay;    // Panel gelap semi-transparan
    public Image           overlayImage;      // Image component dari inspectOverlay
    public Image           paperBackground;   // Gambar kertas/panel putih
    public Image           itemImage;         // Gambar objek (opsional)
    public TextMeshProUGUI inspectText;       // Teks deskripsi

    [Header("— Settings —")]
    public float targetOverlayAlpha = 0.70f;
    public float fadeSpeed          = 0.35f;

    // State
    private bool _isOpen = false;

    // ─────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (inspectOverlay) inspectOverlay.SetActive(false);
    }

    // ════════════════════════════════════════════════════════
    //  PUBLIC API
    // ════════════════════════════════════════════════════════

    /// Buka inspect dengan InspectableObject (dari scene)
    public void OpenInspect(InspectableObject obj)
    {
        if (_isOpen) return;
        StartCoroutine(DoOpen(obj.lines, obj.inspectSprite, null));
    }

    /// Buka inspect hanya dengan array string (untuk ClockAnomaly, dll)
    public void OpenInspectWithLines(string[] lines, Action onClose = null)
    {
        if (_isOpen) return;
        StartCoroutine(DoOpen(lines, null, onClose));
    }

    // ════════════════════════════════════════════════════════
    //  COROUTINE UTAMA
    // ════════════════════════════════════════════════════════

    private IEnumerator DoOpen(string[] lines, Sprite sprite, Action onClose)
    {
        _isOpen = true;

        // Lock player
        var player = FindFirstObjectByType<PlayerController>();
        if (player) player.CanMove = false;

        // Setup konten
        if (sprite != null && itemImage != null)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = sprite;
        }
        else if (itemImage != null)
        {
            itemImage.gameObject.SetActive(false);
        }

        if (inspectText)
            inspectText.text = string.Join("\n\n", lines);

        // Tampilkan overlay dengan fade in
        inspectOverlay.SetActive(true);
        if (overlayImage)
        {
            var c = overlayImage.color; c.a = 0f; overlayImage.color = c;
        }

        yield return StartCoroutine(FadeOverlay(0f, targetOverlayAlpha));

        // Tunggu player tekan E / Space / Enter untuk tutup
        // Tunggu key UP dulu biar tidak langsung close
        yield return new WaitUntil(() =>
            !Input.GetKey(KeyCode.E) && !Input.GetKey(KeyCode.Space));

        yield return new WaitUntil(() =>
            Input.GetKeyDown(KeyCode.E)    ||
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return));

        // Fade out
        yield return StartCoroutine(FadeOverlay(targetOverlayAlpha, 0f));
        inspectOverlay.SetActive(false);

        _isOpen = false;
        if (player) player.CanMove = true;

        onClose?.Invoke();
    }

    // ════════════════════════════════════════════════════════
    //  FADE
    // ════════════════════════════════════════════════════════

    private IEnumerator FadeOverlay(float from, float to)
    {
        if (!overlayImage) yield break;

        float elapsed = 0f;
        var c = overlayImage.color;
        c.a = from;
        overlayImage.color = c;

        while (elapsed < fadeSpeed)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / fadeSpeed));
            overlayImage.color = c;
            yield return null;
        }

        c.a = to;
        overlayImage.color = c;
    }
}