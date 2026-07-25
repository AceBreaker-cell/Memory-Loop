using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Sistem credits dengan scroll animation seperti film.
/// Bisa dipakai di Ending scene (dengan tombol Play Again + Back to Menu)
/// atau di Main Menu (hanya tombol Back to Menu).
/// </summary>
public class CreditsScroller : MonoBehaviour
{
    public static CreditsScroller Instance;

    [Header("— Panel —")]
    public GameObject creditsPanel;      // Panel fullscreen untuk credits
    public Image      backgroundOverlay; // Background hitam di belakang teks

    [Header("— Scroll Content —")]
    [Tooltip("RectTransform dari teks credits yang akan di-scroll naik")]
    public RectTransform scrollContent;
    public TextMeshProUGUI creditsText;

    [Header("— Buttons —")]
    public GameObject btnBackToMenuRoot;   // Selalu muncul
    public GameObject btnPlayAgainRoot;    // Hanya muncul jika fromEnding = true
    public Button      btnBackToMenu;
    public Button      btnPlayAgain;

    [Header("— Scroll Settings —")]
    public float scrollSpeed       = 40f;   // pixel per detik
    public float startDelay        = 1.0f;
    public float buttonFadeInDelay  = 2.0f; // delay sebelum tombol muncul
    public float buttonFadeInDuration = 1.0f;

    [Header("— Scene Names —")]
    public string mainMenuScene = "Main Menu";
    public string newGameScene  = "OpeningCutscene";

    [Header("— Credits Content —")]
    [TextArea(10, 30)]
    public string creditsContent =
        "GAME CREATOR\n" +
        "Muhammad Aziz Syah Dani\n\n" +
        "\n" +
        "ASSETS\n" +
        "Pixel Art — A Space for the Unbound (referensi visual)\n" +
        "Font — TextMesh Pro\n" +
        "\n" +
        "\n" +
        "AUDIO\n" +
        "Background Music — [Nama Komposer]\n" +
        "Sound Effects — [Sumber SFX]\n" +
        "\n" +
        "\n" +
        "SPECIAL THANKS\n" +
        "Dosen Pengenalan Pemrograman Game\n" +
        "Teman-teman Kelompok Ganjil\n" +
        "\n" +
        "\n" +
        "Terima kasih telah bermain.\n" +
        "\"Hari yang Terus Berulang\"\n" +
        "\n" +
        "\n" +
        "© 2026";

    // State
    private bool _fromEnding   = false;
    private bool _hasFinished  = false;

    // ─────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (creditsPanel) creditsPanel.SetActive(false);
    }

    // ════════════════════════════════════════════════════════
    //  PUBLIC API
    // ════════════════════════════════════════════════════════

    /// Tampilkan credits dengan opsi tombol Play Again
    /// fromEnding = true → tampilkan Play Again + Back to Menu
    /// fromEnding = false → tampilkan Back to Menu saja (dari Main Menu)
    public void ShowCredits(bool fromEnding)
    {
        _fromEnding  = fromEnding;
        _hasFinished = false;

        if (creditsText) creditsText.text = creditsContent;

        creditsPanel.SetActive(true);

        // Sembunyikan tombol dulu
        SetButtonAlpha(btnBackToMenuRoot, 0f);
        SetButtonAlpha(btnPlayAgainRoot, 0f);

        btnBackToMenuRoot.SetActive(true);
        btnPlayAgainRoot.SetActive(fromEnding); // hanya aktif kalau dari ending

        // Reset posisi scroll ke bawah
        ResetScrollPosition();

        StopAllCoroutines();
        StartCoroutine(RunCreditsSequence());
    }

    public void HideCredits()
    {
        StopAllCoroutines();
        creditsPanel.SetActive(false);
    }

    // ════════════════════════════════════════════════════════
    //  CORE SEQUENCE
    // ════════════════════════════════════════════════════════

    private IEnumerator RunCreditsSequence()
    {
        yield return new WaitForSecondsRealtime(startDelay);

        // Mulai scroll & fade tombol secara paralel
        StartCoroutine(ScrollCredits());
        StartCoroutine(FadeInButtons());

        yield return null;
    }

    private IEnumerator ScrollCredits()
    {
        if (!scrollContent) yield break;

        // Scroll terus selama panel aktif
        while (creditsPanel.activeSelf)
        {
            scrollContent.anchoredPosition += Vector2.up * scrollSpeed * Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeInButtons()
    {
        yield return new WaitForSecondsRealtime(buttonFadeInDelay);

        float elapsed = 0f;
        while (elapsed < buttonFadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / buttonFadeInDuration);

            SetButtonAlpha(btnBackToMenuRoot, t);
            if (_fromEnding) SetButtonAlpha(btnPlayAgainRoot, t);

            yield return null;
        }

        SetButtonAlpha(btnBackToMenuRoot, 1f);
        if (_fromEnding) SetButtonAlpha(btnPlayAgainRoot, 1f);
    }

    private void ResetScrollPosition()
    {
        if (!scrollContent) return;
        // Posisi awal di bawah area visible (akan di-scroll naik)
        var pos = scrollContent.anchoredPosition;
        pos.y = 0f; // sesuaikan dengan setup RectTransform kamu
        scrollContent.anchoredPosition = pos;
    }

    private void SetButtonAlpha(GameObject root, float alpha)
    {
        if (!root) return;
        var group = root.GetComponent<CanvasGroup>();
        if (!group) group = root.AddComponent<CanvasGroup>();
        group.alpha          = alpha;
        group.interactable   = alpha > 0.9f;
        group.blocksRaycasts = alpha > 0.9f;
    }

    // ════════════════════════════════════════════════════════
    //  BUTTON CALLBACKS
    // ════════════════════════════════════════════════════════

    public void OnClickBackToMenu()
    {
        if (_hasFinished) return;
        _hasFinished = true;

        AudioManager.Instance?.PlayClickSFX();
        EmotionFlagSystem.Instance?.ResetFlags();
        StartCoroutine(FadeAndLoadScene(mainMenuScene));
    }

    public void OnClickPlayAgain()
    {
        if (_hasFinished) return;
        _hasFinished = true;

        AudioManager.Instance?.PlayClickSFX();
        EmotionFlagSystem.Instance?.ResetFlags();
        CheckpointManager.Instance?.ClearCheckpoint();
        StartCoroutine(FadeAndLoadScene(newGameScene));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {

        // Fade out visual
        if (backgroundOverlay)
        {
            float elapsed = 0f;
            float dur     = 1.0f;
            var c = backgroundOverlay.color;
            while (elapsed < dur)
            {
                elapsed += Time.unscaledDeltaTime;
                c.a = Mathf.Lerp(c.a, 1f, elapsed / dur);
                backgroundOverlay.color = c;
                yield return null;
            }
        }

        yield return new WaitForSecondsRealtime(0.3f);
        SceneManager.LoadScene(sceneName);
    }
}