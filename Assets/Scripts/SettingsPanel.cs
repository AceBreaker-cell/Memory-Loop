using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image darkOverlay;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Text musicVolumeLabel;
    [SerializeField] private Text sfxVolumeLabel;

    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float overlayAlpha = 0.5f;

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

        // Setup sliders
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.minValue = 0f;
            musicVolumeSlider.maxValue = 1f;
            musicVolumeSlider.value = AudioListener.volume;
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.value = 0.8f;
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        // Update labels
        UpdateMusicLabel();
        UpdateSFXLabel();
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioListener.volume = value;
        UpdateMusicLabel();
        Debug.Log($"Music Volume: {value * 100:F0}%");
    }

    private void OnSFXVolumeChanged(float value)
    {
        UpdateSFXLabel();
        Debug.Log($"SFX Volume: {value * 100:F0}%");
    }

    private void UpdateMusicLabel()
    {
        if (musicVolumeLabel != null && musicVolumeSlider != null)
        {
            int volumePercent = Mathf.RoundToInt(musicVolumeSlider.value * 100);
            musicVolumeLabel.text = $"Music: {volumePercent}%";
        }
    }

    private void UpdateSFXLabel()
    {
        if (sfxVolumeLabel != null && sfxVolumeSlider != null)
        {
            int volumePercent = Mathf.RoundToInt(sfxVolumeSlider.value * 100);
            sfxVolumeLabel.text = $"SFX: {volumePercent}%";
        }
    }

    // ✅ IMPORTANT: This just prepares the panel, does NOT start coroutine!
    public void PrepareSettings()
    {
        // Just prepare, don't animate yet
    }

    // ✅ Call this from MainMenuManager's coroutine when safe!
    public void PlaySettings()
    {
        StartCoroutine(FadeInPanel());
    }

    public void HideSettings()
    {
        StartCoroutine(FadeOutAndDeactivate());
    }

    private IEnumerator FadeInPanel()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeInDuration;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, progress);

            if (darkOverlay != null)
            {
                Color overlayColor = darkOverlay.color;
                overlayColor.a = Mathf.Lerp(0f, overlayAlpha, progress);
                darkOverlay.color = overlayColor;
            }

            yield return null;
        }

        canvasGroup.alpha = 1f;
        if (darkOverlay != null)
        {
            Color overlayColor = darkOverlay.color;
            overlayColor.a = overlayAlpha;
            darkOverlay.color = overlayColor;
        }
    }

    private IEnumerator FadeOutAndDeactivate()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeInDuration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);

            if (darkOverlay != null)
            {
                Color overlayColor = darkOverlay.color;
                overlayColor.a = Mathf.Lerp(overlayAlpha, 0f, progress);
                darkOverlay.color = overlayColor;
            }

            yield return null;
        }

        canvasGroup.alpha = 0f;
        if (darkOverlay != null)
        {
            Color overlayColor = darkOverlay.color;
            overlayColor.a = 0f;
            darkOverlay.color = overlayColor;
        }

        gameObject.SetActive(false);
    }
}