using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Mengelola puzzle foto keluarga di Loop 2.
/// Player harus mengumpulkan 5 potongan foto yang tersebar di seluruh rumah.
/// Saat semua terkumpul, foto menyatu dan ruangan baru terbuka.
/// </summary>
public class PhotoPuzzleManager : MonoBehaviour
{
    public static PhotoPuzzleManager Instance;

    // ── Progress ──────────────────────────────────────────────
    [Header("— Progress —")]
    public int totalPieces   = 5;
    public int foundPieces   = 0;
    public bool puzzleComplete = false;

    // ── UI Progress Bar ───────────────────────────────────────
    [Header("— UI Progress Bar —")]
    public GameObject progressBarRoot;      // Panel container progress bar
    public Image      progressBarFill;      // Image yang di-fill (fill amount 0-1)
    public TextMeshProUGUI progressText;    // "2/5 Potongan Foto"
    public TextMeshProUGUI progressHint;    // "Cari potongan foto yang tersebar..."

    // ── Sprite Foto ───────────────────────────────────────────
    [Header("— Foto Sprites —")]
    public Sprite[] piecesSprite;           // Sprite tiap potongan (opsional, untuk preview)
    public Sprite   completedPhotoSprite;   // Foto lengkap setelah semua terkumpul

    // ── Completed Photo Reveal ────────────────────────────────
    [Header("— Reveal Panel —")]
    public GameObject revealPanel;          // Panel khusus saat foto lengkap
    public Image      revealImage;          // Gambar foto lengkap
    public TextMeshProUGUI revealText;      // Teks narasi saat foto lengkap

    // ── Unlock Ruangan ────────────────────────────────────────
    [Header("— Unlock Rooms —")]
    // Setiap beberapa foto terkumpul, satu ruangan/door terbuka
    public UnlockThreshold[] unlockThresholds;

    // ── SFX ───────────────────────────────────────────────────
    [Header("— Audio —")]
    public AudioSource sfxSource;
    public AudioClip   pickupSFX;           // Suara saat ambil foto
    public AudioClip   unlockSFX;           // Suara saat ruangan terbuka
    public AudioClip   completeSFX;         // Suara saat foto lengkap

    // ─────────────────────────────────────────────────────────

    [System.Serializable]
    public class UnlockThreshold
    {
        public int             piecesRequired;   // Berapa foto diperlukan
        public GameObject      doorToUnlock;     // Pintu/ruangan yang terbuka
        public string          unlockMessage;    // Pesan saat terbuka
    }

    // ─────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        // Sembunyikan progress bar saat awal
        if (progressBarRoot) progressBarRoot.SetActive(false);
        if (revealPanel)     revealPanel.SetActive(false);

        // Semua pintu terkunci di awal
        foreach (var t in unlockThresholds)
            if (t.doorToUnlock) t.doorToUnlock.SetActive(false);

        UpdateProgressUI();
    }

    // ════════════════════════════════════════════════════════
    //  PUBLIC — dipanggil oleh PhotoPiece saat player ambil
    // ════════════════════════════════════════════════════════

    public void CollectPiece(PhotoPiece piece)
    {
        if (puzzleComplete) return;
        if (piece.isCollected) return;

        piece.isCollected = true;
        foundPieces++;

        // Tampilkan progress bar
        if (!progressBarRoot.activeSelf)
            progressBarRoot.SetActive(true);

        // SFX pickup
        if (sfxSource && pickupSFX)
            sfxSource.PlayOneShot(pickupSFX);

        // Update UI
        UpdateProgressUI();

        // Cek unlock threshold
        CheckUnlocks();

        // Cek apakah selesai
        if (foundPieces >= totalPieces)
            StartCoroutine(OnPuzzleComplete());

        Debug.Log($"[Puzzle] Foto {foundPieces}/{totalPieces} terkumpul!");
    }

    // ════════════════════════════════════════════════════════
    //  UPDATE UI
    // ════════════════════════════════════════════════════════

    private void UpdateProgressUI()
    {
        if (progressBarFill)
            progressBarFill.fillAmount = (float)foundPieces / totalPieces;

        if (progressText)
            progressText.text = $"{foundPieces}/{totalPieces} Potongan Foto";

        if (progressHint)
        {
            if (foundPieces == 0)
                progressHint.text = "Ada yang aneh dengan foto keluarga itu...";
            else if (foundPieces < totalPieces)
                progressHint.text = "Terus cari potongan foto yang tersebar...";
            else
                progressHint.text = "Foto keluarga sudah lengkap!";
        }
    }

    // ════════════════════════════════════════════════════════
    //  CEK UNLOCK
    // ════════════════════════════════════════════════════════

    private void CheckUnlocks()
    {
        foreach (var t in unlockThresholds)
        {
            if (foundPieces >= t.piecesRequired && t.doorToUnlock != null
                && !t.doorToUnlock.activeSelf)
            {
                StartCoroutine(UnlockRoom(t));
            }
        }
    }

    private IEnumerator UnlockRoom(UnlockThreshold threshold)
    {
        // SFX unlock
        if (sfxSource && unlockSFX)
            sfxSource.PlayOneShot(unlockSFX);

        // Tampilkan notifikasi
        if (!string.IsNullOrEmpty(threshold.unlockMessage))
        {
            var notifLines = new string[] { threshold.unlockMessage };
            DialogueManager.Instance?.StartMonologue("Mono", notifLines);

            // Tunggu dialog selesai
            yield return new WaitUntil(() => !DialogueManager.Instance.IsOpen);
        }

        // Aktifkan pintu/ruangan
        threshold.doorToUnlock.SetActive(true);

        // Efek visual glitch kecil
        var atm = FindFirstObjectByType<LoopAtmosphere>();
        atm?.TriggerMicroGlitch();

        yield return null;
    }

    // ════════════════════════════════════════════════════════
    //  PUZZLE COMPLETE
    // ════════════════════════════════════════════════════════

    private IEnumerator OnPuzzleComplete()
    {
        puzzleComplete = true;

        // SFX
        if (sfxSource && completeSFX)
            sfxSource.PlayOneShot(completeSFX);

        // Efek glitch lebih intens
        var atm = FindFirstObjectByType<LoopAtmosphere>();
        atm?.TriggerFullGlitch();

        yield return new WaitForSecondsRealtime(0.5f);

        // Tampilkan foto lengkap
        if (revealPanel)
        {
            if (revealImage && completedPhotoSprite)
                revealImage.sprite = completedPhotoSprite;

            if (revealText)
                revealText.text = "Ini… foto keluarga kami.\nTapi mengapa terasa begitu jauh?";

            revealPanel.SetActive(true);
        }

        // Dialog reveal
        var lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Foto ini… sudah lama tidak aku lihat."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Ibu terlihat sangat bahagia di sini."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Kenapa aku merasa… ini sudah lama sekali?"
            },
        };

        // Lock player
        var player = FindFirstObjectByType<PlayerController>();
        if (player) player.CanMove = false;

        // Tunggu sebentar lalu dialog
        yield return new WaitForSecondsRealtime(1f);

        if (revealPanel) revealPanel.SetActive(false);

        if (player) player.CanMove = true;

        DialogueManager.Instance?.StartDialogue(lines, () =>
        {
            // Tambahkan ke inventory
            Debug.Log("[Puzzle] Foto keluarga ditambahkan ke inventory!");
        });
    }
}