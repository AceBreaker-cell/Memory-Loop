using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("— Audio Sources —")]
    public AudioSource musicSource;  // drag MenuMusicSource
    public AudioSource sfxSource;    // drag UISfxSource

    [Header("— BGM Clips —")]
    public AudioClip mainMenuMusic;
    public AudioClip newGameMusic;   // musik saat klik New Game

    [Header("— SFX Clips —")]
    public AudioClip hoverSFX;
    public AudioClip clickSFX;

    [Header("— Fade Settings —")]
    public float musicFadeInDuration  = 2f;
    public float musicFadeOutDuration = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        if (mainMenuMusic != null)
            StartCoroutine(PlayMusicFadeIn(mainMenuMusic));
    }

    // ── Fade in musik dari volume 0 ──
    public void PlayMainMenuMusic()
    {
        StartCoroutine(PlayMusicFadeIn(mainMenuMusic));
    }

    // ── Transisi musik (fade out lama → fade in baru) ──
    public void TransitionToMusic(AudioClip newClip)
    {
        if (newClip == null) return;
        StartCoroutine(CrossFade(newClip));
    }

    // ── SFX ──
    public void PlayHoverSFX() { if (hoverSFX) sfxSource.PlayOneShot(hoverSFX); }
    public void PlayClickSFX() { if (clickSFX) sfxSource.PlayOneShot(clickSFX); }

    // ── Dipanggil oleh Slider Settings ──
    public void SetMusicVolume(float val) => musicSource.volume = val;
    public void SetSFXVolume(float val)   => sfxSource.volume   = val;
    public float GetMusicVolume()         => musicSource.volume;
    public float GetSFXVolume()           => sfxSource.volume;

    // ════════════════════════════════
    //  COROUTINES
    // ════════════════════════════════
    private IEnumerator PlayMusicFadeIn(AudioClip clip)
    {
        musicSource.clip   = clip;
        musicSource.loop   = true;
        musicSource.volume = 0f;
        musicSource.Play();

        float t = 0f;
        while (t < musicFadeInDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, 1f, t / musicFadeInDuration);
            yield return null;
        }
        musicSource.volume = 1f;
    }

    private IEnumerator CrossFade(AudioClip newClip)
    {
        // Fade out
        float startVol = musicSource.volume;
        float t = 0f;
        while (t < musicFadeOutDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVol, 0f, t / musicFadeOutDuration);
            yield return null;
        }
        musicSource.Stop();

        // Ganti dan fade in
        yield return StartCoroutine(PlayMusicFadeIn(newClip));
    }
}