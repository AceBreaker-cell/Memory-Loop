using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Loop 1 — "Deja Vu"
/// Suasana suram, jam berhenti, dialog Ibu terasa terlalu rapi.
/// </summary>
public class Loop1Manager : MonoBehaviour
{
    public static Loop1Manager Instance;

    // ── Story flags ──────────────────────────────────────────
    [Header("— Story State —")]
    public bool openingMonologueDone = false;
    public bool metMomOutside        = false;
    public bool enteredHouse         = false;
    public bool inspectedClock       = false;
    public bool talkedAtKitchen      = false;
    public bool goneToBed            = false;

    // ── NPC ──────────────────────────────────────────────────
    [Header("— NPC References —")]
    public GameObject ibuOutside;   // drag "Ibu" di Hierarchy
    public GameObject ibuKitchen;   // drag "Ibu_Kitchen"

    // ── Door ─────────────────────────────────────────────────
    [Header("— Door —")]
    public GameObject doorTriggerEnter;

    // ── Atmosphere ───────────────────────────────────────────
    [Header("— Atmosphere —")]
    public LoopAtmosphere atmosphere; // drag object LoopAtmosphere

    // ── Cutscene / Scene ─────────────────────────────────────
    [Header("— Next Scene —")]
    public string loop2Scene = "Loop2Scene";
    public Sprite[] sleepCutsceneImages; // gambar cutscene tidur Loop 1

    // ─────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        Time.timeScale = 1f;

        // Setup NPC
        if (ibuOutside)       ibuOutside.SetActive(true);
        if (ibuKitchen)       ibuKitchen.SetActive(false);
        if (doorTriggerEnter) doorTriggerEnter.SetActive(false);

        // Aktifkan atmosfer suram Loop 1
        atmosphere?.ActivateLoop1Atmosphere();

        // Lock player selama fade in
        var player = FindFirstObjectByType<PlayerController>();
        if (player) player.CanMove = false;

        // Fade in lambat sinematik, lalu monolog pembuka
        CutscenePlayer.Instance?.FadeInGameplay(2.2f, () =>
        {
            StartCoroutine(PlayOpeningMonologue());
        });
    }

    // ════════════════════════════════════════════════════════
    //  PEMBUKA — Monolog Mono saat spawn
    // ════════════════════════════════════════════════════════
    private IEnumerator PlayOpeningMonologue()
    {
        yield return new WaitForSeconds(0.4f);

        var lines = new string[]
        {
            "Perasaan… aku sudah mengalami ini.",
            "Hari ini terasa sama seperti kemarin.",
            "Tapi mungkin aku hanya terlalu lelah.",
        };

        DialogueManager.Instance?.StartMonologue("Mono", lines, () =>
        {
            openingMonologueDone = true;
            // Player bisa gerak setelah monolog selesai
            var p = FindFirstObjectByType<PlayerController>();
            if (p) p.CanMove = true;
        });
    }

    // ════════════════════════════════════════════════════════
    //  IBU DI LUAR — dialog SAMA PERSIS Loop 0 (terasa aneh)
    // ════════════════════════════════════════════════════════
    public void TriggerOutsideDialogue()
    {
        if (metMomOutside) return;

        // Dialog SAMA PERSIS dengan Loop 0 — ini yang membuat player sadar
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
                text    = "Ibu kangen sekali sama kamu. Kamu sehat, Nak?"
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "...",
                choices = new string[]
                {
                    "Iya Bu, aku baik-baik saja.",
                    "Bu, apakah ini… sudah pernah terjadi?"
                },
                onChoiceSelected = (idx) =>
                {
                    if (idx == 1)
                    {
                        // Pilihan kritis — Mono mulai sadar
                        StartCoroutine(IbuReactionAneh());
                        return;
                    }
                    Debug.Log("[Loop1] Pilihan luar: " + idx);
                }
            },
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "Syukurlah… Ibu sudah masak kesukaan kamu. Masuk dulu yuk, sudah sore."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Iya Bu, maaf sudah lama tidak pulang."
            },
        };

        DialogueManager.Instance?.StartDialogue(lines, () =>
        {
            metMomOutside = true;
            if (doorTriggerEnter) doorTriggerEnter.SetActive(true);
        });
    }

    // Reaksi Ibu yang sedikit aneh jika Mono tanya
    private IEnumerator IbuReactionAneh()
    {
        yield return new WaitForSeconds(0.3f);

        // Ibu diam sebentar (glitch kecil)
        atmosphere?.TriggerMicroGlitch();

        yield return new WaitForSeconds(0.8f);

        var lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "Syukurlah… Ibu sudah masak kesukaan kamu. Masuk dulu yuk, sudah sore."
            },
        };

        // Ibu menjawab seolah tidak mendengar pertanyaan Mono
        DialogueManager.Instance?.StartDialogue(lines, () =>
        {
            metMomOutside = true;
            if (doorTriggerEnter) doorTriggerEnter.SetActive(true);
        });
    }

    // ════════════════════════════════════════════════════════
    //  MASUK RUMAH
    // ════════════════════════════════════════════════════════
    public void OnEnteredHouse()
    {
        if (enteredHouse) return;
        enteredHouse = true;

        if (ibuOutside)  ibuOutside.SetActive(false);
        if (ibuKitchen)  ibuKitchen.SetActive(true);

        // Sedikit tingkatkan intensitas atmosfer saat masuk
        atmosphere?.IntensifyAtmosphere();

        Debug.Log("[Loop1] Masuk rumah.");
    }

    // ════════════════════════════════════════════════════════
    //  JAM — dipanggil oleh ClockAnomaly saat selesai inspect
    // ════════════════════════════════════════════════════════
    public void OnClockInspected()
    {
        inspectedClock = true;
        Debug.Log("[Loop1] Jam sudah di-inspect.");
    }

    // ════════════════════════════════════════════════════════
    //  DAPUR — dialog Ibu aneh, sedikit repetitif
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
                // Kata-kata ini SAMA PERSIS dengan Loop 0 — terasa terlalu rapi
                text    = "Makanya sering pulang. Rumah ini selalu terbuka untukmu."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Iya Bu… enak sekali masakan ibu."
            },
            new DialogueLine
            {
                speaker = "Ibu",
                // Ibu mengulang kalimat yang sama
                text    = "Makanya sering pulang. Rumah ini selalu terbuka untukmu."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "...",
                choices = new string[]
                {
                    "Bu, kamu bilang itu dua kali.",
                    "Iya Bu, aku tahu.",
                    "Ada yang aneh dengan hari ini."
                },
                onChoiceSelected = (idx) =>
                {
                    Debug.Log($"[Loop1] Kitchen choice: {idx}");
                    // idx 0 = sadar, idx 1 = deny, idx 2 = awareness
                }
            },
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "Istirahat sana, Nak. Sudah malam. Kamar kamu masih sama seperti dulu."
            },
        };

        DialogueManager.Instance?.StartDialogue(lines, () =>
        {
            talkedAtKitchen = true;
        });
    }

    // ════════════════════════════════════════════════════════
    //  KASUR / TIDUR
    // ════════════════════════════════════════════════════════
    public void TriggerSleep()
    {
        if (!talkedAtKitchen)
        {
            DialogueManager.Instance?.StartMonologue("Mono",
                new string[] { "Sepertinya ibu masih menunggu di dapur…" });
            return;
        }
        if (goneToBed) return;

        var lines = new DialogueLine[]
        {
            new DialogueLine { speaker = "Mono", text = "Hari ini… terasa seperti kemarin." },
            new DialogueLine { speaker = "Mono", text = "Jam di ruang keluarga berhenti." },
            new DialogueLine { speaker = "Mono", text = "Dan ibu… mengulang kalimat yang sama." },
            new DialogueLine { speaker = "Mono", text = "Mungkin aku terlalu lelah. Aku harus tidur." },
        };

        DialogueManager.Instance?.StartDialogue(lines, () =>
        {
            goneToBed = true;

            // Auto-save sebelum pindah scene
            CheckpointManager.Instance?.SaveCheckpoint(1);

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
                openFadeDuration  = 0.9f,
                closeFadeDuration = 1.1f
            };

            // Langsung load Loop2Scene setelah cutscene
            CutscenePlayer.Instance?.PlayCutsceneThenLoadScene(data, loop2Scene);
        }
        else
        {
            // Tidak ada gambar, langsung load
            StartCoroutine(LoadLoop2Direct());
        }
    }

    private IEnumerator LoadLoop2Direct()
    {
        // Simple fade to black lalu load
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene(loop2Scene);
    }

    // ════════════════════════════════════════════════════════
    //  ROOM EDGE NOTIFY
    // ════════════════════════════════════════════════════════
    public void OnEnteredKitchen()  => Debug.Log("[Loop1] Masuk dapur.");
    public void OnEnteredBedroom()  => Debug.Log("[Loop1] Masuk kamar.");
}