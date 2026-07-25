using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CutscenePlayer : MonoBehaviour
{
    public static CutscenePlayer Instance;

    [Header("— UI References —")]
    public GameObject      cutscenePanel;   // Panel_Cutscene (fullscreen)
    public Image           cutsceneImage;   // Img_Cutscene (di dalam Panel_Cutscene)
    public Image           innerFadeOverlay;// Overlay_Fade (di DALAM Panel_Cutscene, antar gambar)
    public Image           globalFade;      // Global_Fade (fullscreen di luar, untuk masuk/keluar gameplay)
    public GameObject      titleCardPanel;  // Panel_TitleCard
    public TextMeshProUGUI titleCardText;   // Text_Title
    public TextMeshProUGUI titleCardSub;    // Text_Subtitle

    [Header("— Timing —")]
    public float defaultImageDuration  = 3f;
    public float defaultFadeDuration   = 0.5f;
    public float titleCardDuration     = 2.5f;

    [Header("— Gameplay Fade In —")]
    public float gameplayFadeInDuration = 1.8f; // Durasi fade in saat kembali ke gameplay

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Sembunyikan semua panel saat start
        if (cutscenePanel)  cutscenePanel.SetActive(false);
        if (titleCardPanel) titleCardPanel.SetActive(false);

        // Global fade mulai transparan
        if (globalFade)
        {
            var c = globalFade.color;
            c.a = 0f;
            globalFade.color = c;
            globalFade.gameObject.SetActive(false);
        }

        // Inner fade mulai hitam penuh (akan di-reset saat cutscene mulai)
        if (innerFadeOverlay)
        {
            var c = innerFadeOverlay.color;
            c.a = 1f;
            innerFadeOverlay.color = c;
            innerFadeOverlay.gameObject.SetActive(false);
        }
    }

    // ═══════════════════════════════════════════════════════
    //  PUBLIC API
    // ═══════════════════════════════════════════════════════

    /// Cutscene biasa — selesai → fade in gameplay → onDone (dialog, dll)
    public void PlayCutscene(CutsceneData data, Action onDone = null)
    {
        StartCoroutine(RunCutscene(data, onDone, loadSceneAfter: null));
    }

    /// Cutscene lalu LANGSUNG load scene — tidak balik gameplay sama sekali
    public void PlayCutsceneThenLoadScene(CutsceneData data, string sceneName)
    {
        StartCoroutine(RunCutscene(data, onDone: null, loadSceneAfter: sceneName));
    }

    /// Fade in gameplay perlahan (dipanggil dari GameFlowManager.Start)
    public void FadeInGameplay(float duration, Action onDone = null)
    {
        StartCoroutine(DoGlobalFadeIn(duration, onDone));
    }

    /// Title card saja
    public void ShowTitleCard(string title, string sub, Action onDone = null)
    {
        StartCoroutine(RunTitleCard(title, sub, onDone));
    }

    // ═══════════════════════════════════════════════════════
    //  CORE CUTSCENE
    // ═══════════════════════════════════════════════════════

    private IEnumerator RunCutscene(CutsceneData data, Action onDone, string loadSceneAfter)
    {
        var player = FindFirstObjectByType<PlayerController>();
        if (player) player.CanMove = false;

        // ── Reset gambar dulu — FIX: gambar lama tidak bocor ke cutscene baru
        if (cutsceneImage != null)
        {
            cutsceneImage.sprite = null;
            cutsceneImage.color  = new Color(1f, 1f, 1f, 0f);
        }

        // ── Global fade to black (sembunyikan gameplay)
        yield return StartCoroutine(DoGlobalFade(0f, 1f, 0.4f));

        // ── Aktifkan panel cutscene, inner overlay hitam penuh
        cutscenePanel.SetActive(true);
        SetInnerAlpha(1f);
        innerFadeOverlay.gameObject.SetActive(true);

        // ── Global fade kembali transparan (panel sudah muncul, gameplay tersembunyi oleh panel)
        yield return StartCoroutine(DoGlobalFade(1f, 0f, 0.3f));

        // ── Tampilkan setiap gambar
        for (int i = 0; i < data.frames.Length; i++)
        {
            var frame = data.frames[i];

            // Set gambar baru sebelum fade in
            cutsceneImage.sprite = frame.image;
            cutsceneImage.color  = Color.white;

            // Fade IN (inner overlay hitam → transparan, gambar terlihat)
            yield return StartCoroutine(DoInnerFade(1f, 0f, data.fadeDuration));

            // Tahan gambar
            float hold = frame.duration > 0f ? frame.duration : data.defaultDuration;
            yield return new WaitForSecondsRealtime(hold);

            // Fade OUT ke hitam sebelum gambar berikutnya
            if (i < data.frames.Length - 1)
            {
                yield return StartCoroutine(DoInnerFade(0f, 1f, data.fadeDuration));

                // ── FIX BUG BOCOR: kosongkan gambar lama setelah fade out
                cutsceneImage.sprite = null;
                cutsceneImage.color  = new Color(1f, 1f, 1f, 0f);
            }
        }

        // ── Fade out terakhir (gambar terakhir → hitam)
        yield return StartCoroutine(DoInnerFade(0f, 1f, data.closeFadeDuration));

        // ── Kosongkan dan matikan panel cutscene
        cutsceneImage.sprite = null;
        cutsceneImage.color  = new Color(1f, 1f, 1f, 0f);
        cutscenePanel.SetActive(false);
        innerFadeOverlay.gameObject.SetActive(false);

        // ═══ PILIHAN: load scene atau kembali ke gameplay ═══

        if (!string.IsNullOrEmpty(loadSceneAfter))
        {
            // Mode: langsung ke scene baru
            // Tampilkan title card di atas global fade hitam
            yield return StartCoroutine(DoGlobalFade(0f, 1f, 0.3f));
            yield return StartCoroutine(RunTitleCard("Loop 1", "Kenapa?", null));

            // Kecilkan sebentar lalu load
            yield return new WaitForSecondsRealtime(0.3f);
            UnityEngine.SceneManagement.SceneManager.LoadScene(loadSceneAfter);
        }
        else
        {
            // Mode: kembali ke gameplay dengan fade in lambat & sinematik
            // Global fade in perlahan — FIX: gameplay masuk secara smooth
            yield return StartCoroutine(DoGlobalFadeIn(gameplayFadeInDuration, null));

            // SETELAH fade in selesai, baru unlock player & panggil onDone
            if (player) player.CanMove = true;
            onDone?.Invoke();
        }
    }

    // ═══════════════════════════════════════════════════════
    //  TITLE CARD
    // ═══════════════════════════════════════════════════════

    private IEnumerator RunTitleCard(string title, string sub, Action onDone)
    {
        if (titleCardPanel == null) { onDone?.Invoke(); yield break; }

        titleCardText.text = title;
        if (titleCardSub != null) titleCardSub.text = sub;

        // Pastikan global fade hitam penuh sebagai background
        SetGlobalAlpha(1f);
        globalFade.gameObject.SetActive(true);

        titleCardPanel.SetActive(true);

        // Setup CanvasGroup untuk fade alpha
        var group = titleCardPanel.GetComponent<CanvasGroup>();
        if (group == null) group = titleCardPanel.AddComponent<CanvasGroup>();
        group.alpha = 0f;

        // Fade in teks
        float elapsed = 0f, fadeIn = 0.8f;
        while (elapsed < fadeIn)
        {
            elapsed += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(0f, 1f, Mathf.Clamp01(elapsed / fadeIn));
            yield return null;
        }
        group.alpha = 1f;

        // Tahan
        yield return new WaitForSecondsRealtime(titleCardDuration);

        // Fade out teks
        elapsed = 0f;
        float fadeOut = 0.8f;
        while (elapsed < fadeOut)
        {
            elapsed += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, Mathf.Clamp01(elapsed / fadeOut));
            yield return null;
        }
        group.alpha = 0f;
        titleCardPanel.SetActive(false);

        onDone?.Invoke();
    }

    // ═══════════════════════════════════════════════════════
    //  FADE HELPERS
    // ═══════════════════════════════════════════════════════

    // Global fade — overlay fullscreen di luar cutscene (untuk gameplay)
    private IEnumerator DoGlobalFade(float from, float to, float duration)
    {
        if (globalFade == null) yield break;
        globalFade.gameObject.SetActive(true);

        var c = globalFade.color;
        c.a = from;
        globalFade.color = c;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            globalFade.color = c;
            yield return null;
        }

        c.a = to;
        globalFade.color = c;
        if (to <= 0f) globalFade.gameObject.SetActive(false);
    }

    // Global fade IN khusus untuk kembali ke gameplay (lebih lambat dan sinematik)
    private IEnumerator DoGlobalFadeIn(float duration, Action onDone)
    {
        if (globalFade == null) { onDone?.Invoke(); yield break; }

        SetGlobalAlpha(1f);
        globalFade.gameObject.SetActive(true);

        float elapsed = 0f;
        var c = globalFade.color;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, Mathf.Clamp01(elapsed / duration));
            globalFade.color = c;
            yield return null;
        }

        c.a = 0f;
        globalFade.color = c;
        globalFade.gameObject.SetActive(false);

        onDone?.Invoke();
    }

    // Inner fade — overlay di DALAM panel cutscene (antar gambar)
    private IEnumerator DoInnerFade(float from, float to, float duration)
    {
        if (innerFadeOverlay == null) yield break;
        innerFadeOverlay.gameObject.SetActive(true);

        var c = innerFadeOverlay.color;
        c.a = from;
        innerFadeOverlay.color = c;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            innerFadeOverlay.color = c;
            yield return null;
        }

        c.a = to;
        innerFadeOverlay.color = c;
        if (to <= 0f) innerFadeOverlay.gameObject.SetActive(false);
    }

    private void SetGlobalAlpha(float a)
    {
        if (globalFade == null) return;
        var c = globalFade.color; c.a = a; globalFade.color = c;
    }

    private void SetInnerAlpha(float a)
    {
        if (innerFadeOverlay == null) return;
        var c = innerFadeOverlay.color; c.a = a; innerFadeOverlay.color = c;
    }
}

// ═══════════════════════════════════════════════════════
//  DATA CLASSES
// ═══════════════════════════════════════════════════════

[System.Serializable]
public class CutsceneFrame
{
    public Sprite image;
    public float  duration = 0f; // 0 = pakai defaultDuration dari CutsceneData
}

[System.Serializable]
public class CutsceneData
{
    public CutsceneFrame[] frames;
    public float defaultDuration   = 3f;
    public float fadeDuration      = 0.5f;
    public float openFadeDuration  = 0.6f;
    public float closeFadeDuration = 0.8f;
}