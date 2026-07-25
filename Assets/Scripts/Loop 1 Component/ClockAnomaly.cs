using System.Collections;
using UnityEngine;

/// <summary>
/// Jam dinding yang berhenti di Loop 1.
/// Saat di-inspect: dialogue berbeda + efek glitch.
/// </summary>
public class ClockAnomaly : MonoBehaviour, IInteractable
{
    [Header("— Loop —")]
    [Tooltip("0 = normal (Loop 0), 1 = berhenti (Loop 1+)")]
    public int currentLoop = 1;

    [Header("— Clock Parts —")]
    [Tooltip("Transform jarum detik (opsional, untuk animasi)")]
    public Transform secondHand;
    [Tooltip("Transform jarum menit (opsional)")]
    public Transform minuteHand;
    [Tooltip("Transform jarum jam (opsional)")]
    public Transform hourHand;

    [Header("— Animator (jika pakai sprite animation) —")]
    public Animator clockAnimator;

    [Header("— Dialogue Loop 0 (Normal) —")]
    [TextArea(2, 3)]
    public string[] normalLines = {
        "Jam dinding tua milik Ibu.",
        "Jarumnya bergerak seperti biasa.",
    };

    [Header("— Dialogue Loop 1 (Berhenti) —")]
    [TextArea(2, 3)]
    public string[] stoppedLines = {
        "Jam dinding ini…",
        "Jarumnya berhenti.",
        "Atau mungkin hanya aku yang salah lihat?",
    };

    [Header("— Hint —")]
    public string hintText = "[E] Lihat Jam";

    // State
    private bool _hasBeenInspected = false;

    // ─────────────────────────────────────────────────────────

    private void Start()
    {
        if (currentLoop >= 1)
        {
            StopClockAnimation();
        }
    }

    // ════════════════════════════════════════════════════════
    //  IInteractable
    // ════════════════════════════════════════════════════════

    public string GetHintText() => hintText;

    public void Interact()
    {
        if (currentLoop >= 1)
            StartCoroutine(InspectAnomaly());
        else
            InspectNormal();
    }

    // ════════════════════════════════════════════════════════
    //  INSPECT
    // ════════════════════════════════════════════════════════

    private void InspectNormal()
    {
        InspectManager.Instance?.OpenInspectWithLines(normalLines);
    }

    private IEnumerator InspectAnomaly()
    {
        if (_hasBeenInspected)
        {
            // Sudah pernah inspect: hanya dialogue pendek
            var shortLines = new string[] { "Jarumnya masih berhenti." };
            InspectManager.Instance?.OpenInspectWithLines(shortLines);
            yield break;
        }

        _hasBeenInspected = true;

        // Efek glitch sebelum open inspect
        var atm = FindFirstObjectByType<LoopAtmosphere>();
        atm?.TriggerMicroGlitch();

        yield return new WaitForSecondsRealtime(0.3f);

        // Buka inspect dengan dialogue anomali
        InspectManager.Instance?.OpenInspectWithLines(stoppedLines, () =>
        {
            // Notify Loop1Manager
            Loop1Manager.Instance?.OnClockInspected();
        });
    }

    // ════════════════════════════════════════════════════════
    //  STOP ANIMATION
    // ════════════════════════════════════════════════════════

    private void StopClockAnimation()
    {
        // Hentikan Animator
        if (clockAnimator)
        {
            clockAnimator.enabled = false;
        }

        // Freeze jarum pada posisi terakhir
        // (jarum tidak bergerak lagi)
        if (secondHand) secondHand.rotation = Quaternion.Euler(0, 0, -90f); // arahkan ke 12
        if (minuteHand) minuteHand.rotation = Quaternion.Euler(0, 0, 0f);
        if (hourHand)   hourHand.rotation   = Quaternion.Euler(0, 0, 0f);
    }
}