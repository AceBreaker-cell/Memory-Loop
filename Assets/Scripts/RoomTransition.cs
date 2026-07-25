using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoomTransition : MonoBehaviour, IInteractable
{
    [Header("Target Area")]
    public Transform spawnPoint;  // posisi Mono setelah transisi
    public GameObject areaToEnable;   // ruangan tujuan
    public GameObject areaToDisable;  // ruangan asal

    [Header("Fade")]
    public Image fadeOverlay;
    public float fadeDuration = 0.4f;

    [Header("Trigger Prompt")]
    public string hintText = "[E] Masuk";

    public void Interact()
    {
        StartCoroutine(DoTransition());
    }

    public string GetHintText()
    {
        return hintText;
    }

    private IEnumerator DoTransition()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player) player.CanMove = false;

        // Fade out
        yield return StartCoroutine(Fade(0f, 1f));

        // Pindah area
        areaToDisable.SetActive(false);
        areaToEnable.SetActive(true);

        // Teleport player
        if (player && spawnPoint)
            player.transform.position = spawnPoint.position;

        yield return null; // 1 frame
        // Fade in
        yield return StartCoroutine(Fade(1f, 0f));

        if (player) player.CanMove = true;
    }

    private IEnumerator Fade(float from, float to)
    {
        if (!fadeOverlay) yield break;
        fadeOverlay.gameObject.SetActive(true);
        Color c = fadeOverlay.color;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / fadeDuration);
            fadeOverlay.color = c;
            yield return null;
        }
        c.a = to;
        fadeOverlay.color = c;
        if (to == 0f) fadeOverlay.gameObject.SetActive(false);
    }
}