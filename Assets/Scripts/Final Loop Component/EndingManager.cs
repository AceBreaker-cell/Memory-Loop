using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manager untuk scene ending.
/// Tiap ending punya scene sendiri — script ini dipasang di semua scene ending.
/// Setelah teks ending selesai → otomatis trigger Credits (bukan tombol manual lagi).
/// </summary>
public class EndingManager : MonoBehaviour
{
    [Header("— Ending Type —")]
    // PENTING: pakai EmotionFlagSystem.EndingType, BUKAN enum sendiri,
    // supaya kompatibel dengan FinalLoopManager dan AudioManager.PlayEndingMusic()
    public EmotionFlagSystem.EndingType thisEnding;

    [Header("— UI —")]
    public Image           fadeOverlay;
    public TextMeshProUGUI endingTitleText;
    public TextMeshProUGUI endingBodyText;

    [Header("— Ending Content —")]
    [TextArea(3, 8)]
    public string endingTitle;
    [TextArea(5, 15)]
    public string endingBody;

    [Header("— Audio (opsional) —")]
    [Tooltip("Kosongkan untuk pakai musik default dari AudioManager sesuai thisEnding")]
    public AudioClip endingMusicOverride;

    [Header("— Timing —")]
    public float fadeInDuration     = 2.0f;
    public float textDelaySeconds   = 1.0f;
    public float holdBeforeCredits  = 3.0f;

    private void Start()
    {
        Time.timeScale = 1f;

        if (endingTitleText) endingTitleText.alpha = 0f;
        if (endingBodyText)  endingBodyText.alpha  = 0f;

        StartCoroutine(PlayEndingSequence());
    }

    private IEnumerator PlayEndingSequence()
    {
        // Fade in dari hitam
        if (fadeOverlay)
        {
            var c = fadeOverlay.color; c.a = 1f; fadeOverlay.color = c;
            fadeOverlay.gameObject.SetActive(true);
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                c.a = Mathf.Lerp(1f, 0f, elapsed / fadeInDuration);
                fadeOverlay.color = c;
                yield return null;
            }
            c.a = 0f; fadeOverlay.color = c;
            fadeOverlay.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(textDelaySeconds);

        // Fade in judul ending
        if (endingTitleText)
        {
            endingTitleText.text = endingTitle;
            yield return StartCoroutine(FadeText(endingTitleText, 0f, 1f, 1.5f));
        }

        yield return new WaitForSeconds(1.5f);

        // Fade in teks ending
        if (endingBodyText)
        {
            endingBodyText.text = endingBody;
            yield return StartCoroutine(FadeText(endingBodyText, 0f, 1f, 2.0f));
        }

        // Tahan agar player sempat membaca
        yield return new WaitForSeconds(holdBeforeCredits);

        // Fade out teks ending sebelum credits scroll muncul
        if (endingBodyText)  yield return StartCoroutine(FadeText(endingBodyText, 1f, 0f, 1.5f));
        if (endingTitleText) yield return StartCoroutine(FadeText(endingTitleText, 1f, 0f, 1.0f));

        yield return new WaitForSeconds(0.5f);

        // Trigger Credits — fromEnding: true → tampilkan tombol Play Again + Back to Menu
        CreditsScroller.Instance?.ShowCredits(fromEnding: true);
    }

    private IEnumerator FadeText(TextMeshProUGUI tmp, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            tmp.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        tmp.alpha = to;
    }
}