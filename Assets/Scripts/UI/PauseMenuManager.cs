using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance;

    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    [Header("Settings Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private bool _isPaused;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;
        pausePanel.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0f : 1f;

        var player = FindFirstObjectByType<PlayerController>();
        if (player) player.CanMove = !_isPaused;
    }

    public void OnResumeClick()
    {
        _isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        var player = FindFirstObjectByType<PlayerController>();
        if (player) player.CanMove = true;
    }

    public void OnSettingsClick()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);

        if (AudioManager.Instance != null)
        {
            musicSlider.value = AudioManager.Instance.GetMusicVolume();
            sfxSlider.value   = AudioManager.Instance.GetSFXVolume();
        }
    }

    public void OnSettingsBack()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void OnQuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void OnMusicChanged(float v) => AudioManager.Instance?.SetMusicVolume(v);
    public void OnSFXChanged(float v)   => AudioManager.Instance?.SetSFXVolume(v);
}