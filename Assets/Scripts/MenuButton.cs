using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("Hover Scale")]
    public float hoverScale    = 1.1f;
    public float normalScale   = 1.0f;
    public float animDuration  = 0.12f;

    private Coroutine _anim;

    // ── Mouse masuk tombol ──
    public void OnPointerEnter(PointerEventData e)
    {
        AudioManager.Instance?.PlayHoverSFX();
        ScaleTo(hoverScale);
    }

    // ── Mouse keluar tombol ──
    public void OnPointerExit(PointerEventData e)
    {
        ScaleTo(normalScale);
    }

    // ── Mouse klik tombol ──
    public void OnPointerClick(PointerEventData e)
    {
        AudioManager.Instance?.PlayClickSFX();
    }

    private void ScaleTo(float target)
    {
        if (_anim != null) StopCoroutine(_anim);
        _anim = StartCoroutine(ScaleCoroutine(target));
    }

    private IEnumerator ScaleCoroutine(float targetScale)
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale   = Vector3.one * targetScale;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animDuration;
            // Ease out
            t = 1f - (1f - t) * (1f - t);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        transform.localScale = endScale;
    }
}