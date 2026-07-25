using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("— Panel References —")]
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("— Fade Panel —")]
    [SerializeField] private Image fadeImage;

    [Header("— Fade Duration —")]
    [SerializeField] private float fadeInDuration = 1.5f;
    [SerializeField] private float fadeOutDuration = 1.0f;

    [Header("— Scene Names —")]
    [SerializeField] private string newGameSceneName = "OpeningCutscene";
    [SerializeField] private string continueSceneName = "GameScene";

    // ════════════════════════════════════════════════════════════════
    private void Start()
    {
        if (menuRoot != null)
            menuRoot.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        StartCoroutine(FadeIn());
    }

    // ════════════════════════════════════════════════════════════════
    // BUTTON CALLBACKS
    // ════════════════════════════════════════════════════════════════

    public void OnNewGameButtonClicked()
    {
        Debug.Log("New Game Button Clicked");
        StartCoroutine(FadeOutAndLoadScene(newGameSceneName));
    }

    public void OnContinueButtonClicked()
    {
        Debug.Log("Continue Button Clicked");
        StartCoroutine(FadeOutAndLoadScene(continueSceneName));
    }

    public void OnSettingsButtonClicked()
    {
        Debug.Log("Settings Button Clicked");
        
        if (menuRoot != null)
            menuRoot.SetActive(false);

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            StartCoroutine(ShowSettingsPanelSafely());
        }
    }

    public void OnCreditsButtonClicked()
    {
        Debug.Log("Credits Button Clicked");
        
        if (menuRoot != null)
            menuRoot.SetActive(false);

        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
            StartCoroutine(ShowCreditsPanelSafely());
        }
    }

    public void OnSettingsBackButtonClicked()
    {
        Debug.Log("Settings Back Button Clicked");
        
        if (settingsPanel != null)
        {
            SettingsPanel settingsScript = settingsPanel.GetComponent<SettingsPanel>();
            if (settingsScript != null)
                settingsScript.HideSettings();
        }

        if (menuRoot != null)
            menuRoot.SetActive(true);
    }

    public void OnCreditsBackButtonClicked()
    {
        Debug.Log("Credits Back Button Clicked");
        
        if (creditsPanel != null)
        {
            CreditsPanel creditsScript = creditsPanel.GetComponent<CreditsPanel>();
            if (creditsScript != null)
                creditsScript.HideCredits();
        }

        if (menuRoot != null)
            menuRoot.SetActive(true);
    }

    public void OnExitButtonClicked()
    {
        Debug.Log("Exit Button Clicked");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnClickCredits()
    {
    AudioManager.Instance?.PlayClickSFX();
 
    // Sembunyikan main menu panel
    menuRoot.SetActive(false);
 
    // Tampilkan credits TANPA tombol Play Again (fromEnding = false)
    CreditsScroller.Instance?.ShowCredits(fromEnding: false);
    }

    // ════════════════════════════════════════════════════════════════
    // SAFE COROUTINES - Wait for panels to fully activate
    // ════════════════════════════════════════════════════════════════

    private IEnumerator ShowCreditsPanelSafely()
    {
        // Wait for panel to fully activate
        yield return null;
        yield return null;

        // Now safe to play credits
        if (creditsPanel != null)
        {
            CreditsPanel creditsScript = creditsPanel.GetComponent<CreditsPanel>();
            if (creditsScript != null)
            {
                creditsScript.PrepareCredits();
                creditsScript.PlayCredits(); // ✅ Called from safe context
            }
        }
    }

    private IEnumerator ShowSettingsPanelSafely()
    {
        // Wait for panel to fully activate
        yield return null;
        yield return null;

        // Now safe to play settings
        if (settingsPanel != null)
        {
            SettingsPanel settingsScript = settingsPanel.GetComponent<SettingsPanel>();
            if (settingsScript != null)
            {
                settingsScript.PrepareSettings();
                settingsScript.PlaySettings(); // ✅ Called from safe context
            }
        }
    }

    // ════════════════════════════════════════════════════════════════
    // FADE ANIMATIONS
    // ════════════════════════════════════════════════════════════════

    private IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;

        fadeImage.gameObject.SetActive(true);
        Color fadeColor = fadeImage.color;
        fadeColor.a = 1f;
        fadeImage.color = fadeColor;

        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeInDuration);
            fadeImage.color = fadeColor;
            yield return null;
        }

        fadeColor.a = 0f;
        fadeImage.color = fadeColor;
        fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        if (fadeImage == null) yield break;

        fadeImage.gameObject.SetActive(true);
        Color fadeColor = fadeImage.color;
        fadeColor.a = 0f;
        fadeImage.color = fadeColor;

        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeOutDuration);
            fadeImage.color = fadeColor;
            yield return null;
        }

        fadeColor.a = 1f;
        fadeImage.color = fadeColor;

        SceneManager.LoadScene(sceneName);
    }
}