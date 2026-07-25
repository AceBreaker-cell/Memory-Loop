using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI lineText;
    public Image fadeOverlay;

    [Header("Audio")]
    public AudioSource windSource;
    public AudioClip windClip;

    [Header("Settings")]
    public float fadeDuration    = 1.2f;
    public float textFadeIn      = 1.0f;
    public float textHoldDuration = 2.5f;
    public float textFadeOut     = 0.8f;
    public float gapBetweenLines = 0.6f;

    [Header("Scene To Load")]
    public string nextScene = "Loop0Scene";

    private readonly string[] _lines = { "Mono ..", "Kembalilah.." };

    private void Start()
    {
        lineText.alpha = 0f;
        SetFadeAlpha(1f);

        // Angin fade in
        if (windClip)
        {
            windSource.clip = windClip;
            windSource.loop = true;
            windSource.volume = 0f;
            windSource.Play();
        }

        StartCoroutine(RunCutscene());
    }

    private IEnumerator RunCutscene()
    {
        // Fade in dari hitam
        yield return StartCoroutine(FadeOverlay(1f, 0f, fadeDuration));
        yield return StartCoroutine(FadeAudio(windSource, 0f, 0.6f, 1.5f));

        foreach (string line in _lines)
        {
            lineText.text = line;
            // Fade in teks
            yield return StartCoroutine(FadeText(0f, 1f, textFadeIn));
            yield return new WaitForSeconds(textHoldDuration);
            // Fade out teks
            yield return StartCoroutine(FadeText(1f, 0f, textFadeOut));
            yield return new WaitForSeconds(gapBetweenLines);
        }

        // Fade angin turun
        yield return StartCoroutine(FadeAudio(windSource, 0.6f, 0f, 1.5f));

        // Fade out ke scene berikutnya
        yield return StartCoroutine(FadeOverlay(0f, 1f, fadeDuration));
        SceneManager.LoadScene(nextScene);
    }

    private IEnumerator FadeText(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            lineText.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        lineText.alpha = to;
    }

    private IEnumerator FadeOverlay(float from, float to, float duration)
    {
        fadeOverlay.gameObject.SetActive(true);
        float t = 0f;
        Color c = fadeOverlay.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            fadeOverlay.color = c;
            yield return null;
        }
        c.a = to;
        fadeOverlay.color = c;
        if (to == 0f) fadeOverlay.gameObject.SetActive(false);
    }

    private IEnumerator FadeAudio(AudioSource src, float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            src.volume = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        src.volume = to;
    }

    private void SetFadeAlpha(float a)
    {
        Color c = fadeOverlay.color;
        c.a = a;
        fadeOverlay.color = c;
    }
}