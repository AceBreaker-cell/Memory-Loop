using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Loop 2 — "Rumah Mengingat"
/// Foto keluarga rusak. Puzzle foto. Ruangan baru terbuka.
/// Warna lebih pudar, ibu mulai mengulang kalimat.
/// </summary>
public class Loop2Manager : MonoBehaviour
{
    public static Loop2Manager Instance;

    // ── Story flags ──────────────────────────────────────────
    [Header("— Story State —")]
    public bool openingMonologueDone  = false;
    public bool metMomOutside         = false;
    public bool enteredHouse          = false;
    public bool brokenPhotoDiscovered = false;
    public bool puzzleComplete        = false;
    public bool talkedAtKitchen       = false;
    public bool goneToBed             = false;

    // ── NPC ──────────────────────────────────────────────────
    [Header("— NPC References —")]
    public GameObject ibuOutside;
    public GameObject ibuKitchen;

    // ── Door ─────────────────────────────────────────────────
    [Header("— Doors —")]
    public GameObject doorTriggerEnter;
    public GameObject brokenPhotoObj;     // Object foto rusak di dinding (bukan PhotoPiece)

    // ── Atmosphere ───────────────────────────────────────────
    [Header("— Atmosphere —")]
    public LoopAtmosphere atmosphere;

    // ── Scene ────────────────────────────────────────────────
    [Header("— Next Scene —")]
    public string loop3Scene = "Loop3Scene";
    public Sprite[] sleepCutsceneImages;

    // ─────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        Time.timeScale = 1f;

        if (ibuOutside)       ibuOutside.SetActive(true);
        if (ibuKitchen)       ibuKitchen.SetActive(false);
        if (doorTriggerEnter) doorTriggerEnter.SetActive(false);

        // Atmosfer Loop 2 lebih intens dari Loop 1
        atmosphere?.ActivateLoop2Atmosphere();

        var player = FindFirstObjectByType<PlayerController>();
        if (player) player.CanMove = false;

        CutscenePlayer.Instance?.FadeInGameplay(2.5f, () =>
        {
            StartCoroutine(PlayOpeningMonologue());
        });
    }

    // ════════════════════════════════════════════════════════
    //  PEMBUKA
    // ════════════════════════════════════════════════════════

    private IEnumerator PlayOpeningMonologue()
    {
        yield return new WaitForSeconds(0.4f);

        var lines = new string[]
        {
            "Lagi.",
            "Aku terbangun di depan rumah ini… lagi.",
            "Sesuatu di sini… berubah.",
        };

        DialogueManager.Instance?.StartMonologue("Mono", lines, () =>
        {
            openingMonologueDone = true;
            var p = FindFirstObjectByType<PlayerController>();
            if (p) p.CanMove = true;
        });
    }

    // ════════════════════════════════════════════════════════
    //  IBU DI LUAR — sama persis, tapi dengan glitch tambahan
    // ════════════════════════════════════════════════════════

    public void TriggerOutsideDialogue()
    {
        if (metMomOutside) return;

        var lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "Mono! Akhirnya kamu pulang juga, Nak..."
            },
            new DialogueLine
            {
                speaker = "Ibu",
                // Loop 2: Ibu mengulang kalimat yang sama persis lagi
                text    = "Ibu kangen sekali sama kamu. Kamu sehat, Nak?"
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "...",
                choices = new string[]
                {
                    "Bu, ini sudah ketiga kalinya.",
                    "Iya Bu, aku baik.",
                    "Ada yang aneh, Bu."
                },
                onChoiceSelected = idx =>
                {
                    if (idx == 0 || idx == 2)
                        StartCoroutine(IbuGlitchReaction());
                }
            },
            new DialogueLine
            {
                speaker = "Ibu",
                // Ibu tidak merespons, langsung lanjut
                text    = "Masuk sana, sudah sore."
            },
        };

        DialogueManager.Instance?.StartDialogue(lines, () =>
        {
            metMomOutside = true;
            if (doorTriggerEnter) doorTriggerEnter.SetActive(true);
        });
    }

    private IEnumerator IbuGlitchReaction()
    {
        // Ibu freeze sebentar + glitch
        atmosphere?.TriggerMicroGlitch();
        yield return new WaitForSecondsRealtime(0.6f);
        atmosphere?.TriggerMicroGlitch();
    }

    // ════════════════════════════════════════════════════════
    //  MASUK RUMAH — foto rusak langsung ketahuan
    // ════════════════════════════════════════════════════════

    public void OnEnteredHouse()
    {
        if (enteredHouse) return;
        enteredHouse = true;

        if (ibuOutside) ibuOutside.SetActive(false);
        if (ibuKitchen) ibuKitchen.SetActive(true);

        atmosphere?.IntensifyAtmosphere();

        // Sedikit delay lalu trigger dialog foto rusak
        StartCoroutine(TriggerBrokenPhotoDiscovery());
    }

    private IEnumerator TriggerBrokenPhotoDiscovery()
    {
        if (brokenPhotoDiscovered) yield break;

        yield return new WaitForSeconds(1.2f);

        brokenPhotoDiscovered = true;

        // Glitch saat lihat foto rusak
        atmosphere?.TriggerMicroGlitch();

        yield return new WaitForSecondsRealtime(0.3f);

        var lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = "Mono",
                text    = "..."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Foto keluarga ini… ada yang rusak."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Potongannya tersebar. Aku harus mengumpulkannya."
            },
        };

        DialogueManager.Instance?.StartDialogue(lines, () =>
        {
            // Aktifkan progress bar puzzle
            PhotoPuzzleManager.Instance?.gameObject.SetActive(true);
            var root = PhotoPuzzleManager.Instance?.progressBarRoot;
            if (root) root.SetActive(true);
        });
    }

    // ════════════════════════════════════════════════════════
    //  DAPUR — Ibu mulai sangat aneh
    // ════════════════════════════════════════════════════════

    public void TriggerKitchenDialogue()
    {
        if (talkedAtKitchen) return;

        var lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "Ayo duduk dulu, Nak. Makanannya sudah siap dari tadi."
            },
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "Makanya sering pulang. Rumah ini selalu terbuka untukmu."
            },
            new DialogueLine
            {
                speaker = "Ibu",
                // Kalimat diulang persis — terasa sangat aneh di Loop 2
                text    = "Makanya sering pulang. Rumah ini selalu terbuka untukmu."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Bu…"
            },
            new DialogueLine
            {
                speaker = "Ibu",
                // Ibu diam lama, lalu menjawab tidak nyambung
                text    = "..."
            },
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "Istirahat sana, Nak. Sudah malam."
            },
        };

        // Glitch saat Ibu diam
        StartCoroutine(KitchenDialogueWithGlitch(lines));
    }

    private IEnumerator KitchenDialogueWithGlitch(DialogueLine[] lines)
    {
        DialogueManager.Instance?.StartDialogue(lines, null);

        // Tunggu sampai sampai ke "..." Ibu (line ke-5)
        yield return new WaitForSeconds(8f);
        atmosphere?.TriggerMicroGlitch();
        yield return new WaitForSeconds(0.5f);
        atmosphere?.TriggerMicroGlitch();

        // Setelah semua dialog selesai
        yield return new WaitUntil(() => !DialogueManager.Instance.IsOpen);
        talkedAtKitchen = true;
    }

    // ════════════════════════════════════════════════════════
    //  TIDUR
    // ════════════════════════════════════════════════════════

    public void TriggerSleep()
    {
        if (!talkedAtKitchen)
        {
            DialogueManager.Instance?.StartMonologue("Mono",
                new string[] { "Sepertinya ibu masih menunggu di dapur..." });
            return;
        }
        if (goneToBed) return;

        var lines = new DialogueLine[]
        {
            new DialogueLine { speaker = "Mono", text = "Foto itu… aku sudah mengumpulkannya." },
            new DialogueLine { speaker = "Mono", text = "Tapi ibu… ada yang tidak beres." },
            new DialogueLine { speaker = "Mono", text = "Kalimat yang sama. Gerakan yang sama." },
            new DialogueLine { speaker = "Mono", text = "Ini… bukan ibu yang kukenal." },
        };

        DialogueManager.Instance?.StartDialogue(lines, () =>
        {
            goneToBed = true;
            CheckpointManager.Instance?.SaveCheckpoint(2);
            StartSleepCutscene();
        });
    }

    private void StartSleepCutscene()
    {
        if (sleepCutsceneImages != null && sleepCutsceneImages.Length > 0)
        {
            var frames = new CutsceneFrame[sleepCutsceneImages.Length];
            for (int i = 0; i < sleepCutsceneImages.Length; i++)
                frames[i] = new CutsceneFrame { image = sleepCutsceneImages[i], duration = 3f };

            var data = new CutsceneData
            {
                frames            = frames,
                defaultDuration   = 3f,
                fadeDuration      = 0.7f,
                openFadeDuration  = 1.0f,
                closeFadeDuration = 1.2f
            };

            CutscenePlayer.Instance?.PlayCutsceneThenLoadScene(data, loop3Scene);
        }
        else
        {
            StartCoroutine(LoadLoop3Direct());
        }
    }

    private IEnumerator LoadLoop3Direct()
    {
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(loop3Scene);
    }

    // ════════════════════════════════════════════════════════
    //  ROOM EDGE NOTIFY
    // ════════════════════════════════════════════════════════

    public void OnEnteredKitchen()  => Debug.Log("[Loop2] Masuk dapur.");
    public void OnEnteredBedroom()  => Debug.Log("[Loop2] Masuk kamar.");
}