using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreditsPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image darkOverlay;
    [SerializeField] private RectTransform creditsScrollContent;
    [SerializeField] private Text creditsTextComponent;

    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float scrollDuration = 12f;
    [SerializeField] private float overlayAlpha = 0.5f;

    [TextArea(10, 20)]
    [SerializeField] private string creditsText = @"HARI YANG TERUS BERULANG

Diproduksi oleh:
Kelompok Ganjil

━━━━━━━━━━━━━━━━━━━━━━━━

CREATIVE TEAM

Game Design
Tim Kreatif

Programming
Tim Programming

Art & Graphics
Tim Seni

Sound & Music
Tim Audio

Narrative Design
Tim Cerita

━━━━━━━━━━━━━━━━━━━━━━━━

SPECIAL THANKS

Terima kasih kepada semua yang
telah mendukung pengembangan
game ini.

━━━━━━━━━━━━━━━━━━━━━━━━

Terima kasih sudah memainkan game kami :)";

    private bool isPlaying = false;

    private void Start()
    {
        InitializePanel();
    }

    private void InitializePanel()
    {
        // Setup CanvasGroup
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        canvasGroup.alpha = 0f;

        // Setup dark overlay
        if (darkOverlay != null)
        {
            Color overlayColor = darkOverlay.color;
            overlayColor.a = 0f;
            darkOverlay.color = overlayColor;
        }

        // Setup credits text
        if (creditsTextComponent != null)
        {
            creditsTextComponent.text = creditsText;
            creditsTextComponent.alignment = TextAnchor.UpperCenter;
            creditsTextComponent.color = Color.white;
        }
    }

    // ✅ IMPORTANT: This just prepares the panel, does NOT start coroutine!
    public void PrepareCredits()
    {
        if (creditsScrollContent != null)
        {
            creditsScrollContent.anchoredPosition = Vector2.zero;
        }
        isPlaying = false;
    }

    // ✅ Call this from MainMenuManager's coroutine when safe!
    public void PlayCredits()
    {
        if (!isPlaying)
        {
            StartCoroutine(PlayCreditsSequence());
        }
    }

    private IEnumerator PlayCreditsSequence()
    {
        isPlaying = true;

        // Fade in overlay
        yield return StartCoroutine(FadeInOverlay());

        // Fade in panel
        yield return StartCoroutine(FadeInPanel());

        // Reset scroll position
        if (creditsScrollContent != null)
        {
            creditsScrollContent.anchoredPosition = Vector2.zero;
        }

        yield return new WaitForSeconds(0.5f);

        // Scroll animation
        yield return StartCoroutine(ScrollCreditsContent());

        yield return new WaitForSeconds(2f);

        isPlaying = false;
    }

    private IEnumerator FadeInOverlay()
    {
        if (darkOverlay == null) yield break;

        float elapsedTime = 0f;
        Color overlayColor = darkOverlay.color;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeInDuration;
            overlayColor.a = Mathf.Lerp(0f, overlayAlpha, progress);
            darkOverlay.color = overlayColor;
            yield return null;
        }

        overlayColor.a = overlayAlpha;
        darkOverlay.color = overlayColor;
    }

    private IEnumerator FadeInPanel()
    {
        if (canvasGroup == null) yield break;

        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator ScrollCreditsContent()
    {
        if (creditsScrollContent == null) yield break;

        RectTransform contentRect = creditsScrollContent;
        float contentHeight = contentRect.sizeDelta.y;
        float targetYPos = contentHeight + 500f;

        float elapsedTime = 0f;

        while (elapsedTime < scrollDuration && isPlaying)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / scrollDuration;
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            float yPos = Mathf.Lerp(0f, targetYPos, easedProgress);
            contentRect.anchoredPosition = new Vector2(0f, yPos);

            yield return null;
        }

        if (isPlaying)
        {
            contentRect.anchoredPosition = new Vector2(0f, targetYPos);
        }
    }

    public void HideCredits()
    {
        StopAllCoroutines();
        isPlaying = false;
        StartCoroutine(FadeOutAndDeactivate());
    }

    private IEnumerator FadeOutAndDeactivate()
    {
        if (canvasGroup == null || darkOverlay == null) yield break;

        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeInDuration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);

            Color overlayColor = darkOverlay.color;
            overlayColor.a = Mathf.Lerp(overlayAlpha, 0f, progress);
            darkOverlay.color = overlayColor;

            yield return null;
        }

        canvasGroup.alpha = 0f;
        Color finalOverlayColor = darkOverlay.color;
        finalOverlayColor.a = 0f;
        darkOverlay.color = finalOverlayColor;

        gameObject.SetActive(false);
    }
}