using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Loop 3 — "Retakan"
/// Ibu diam terlalu lama. Gerakan kaku. Dialog bercabang menentukan ending.
/// Saturasi turun drastis. Crack effect muncul.
/// </summary>
public class Loop3Manager : MonoBehaviour
{
    public static Loop3Manager Instance;

    [Header("— Story State —")]
    public bool openingDone        = false;
    public bool metMomOutside      = false;
    public bool enteredHouse       = false;
    public bool crack1Shown        = false;
    public bool talkedAtKitchen    = false;
    public bool goneToBed          = false;

    [Header("— NPC —")]
    public GameObject ibuOutside;
    public GameObject ibuKitchen;

    [Header("— Door —")]
    public GameObject doorTriggerEnter;

    [Header("— References —")]
    public LoopAtmosphere atmosphere;
    public CrackEffect    crackEffect;

    [Header("— Scene —")]
    public string finalLoopScene = "FinalLoopScene";
    public Sprite[] sleepCutsceneImages;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        Time.timeScale = 1f;

        // Load emotion flags dari loop sebelumnya
        EmotionFlagSystem.Instance?.LoadFlags();

        if (ibuOutside)       ibuOutside.SetActive(true);
        if (ibuKitchen)       ibuKitchen.SetActive(false);
        if (doorTriggerEnter) doorTriggerEnter.SetActive(false);

        // Atmosfer Loop 3: sangat suram
        atmosphere?.ActivateLoop3Atmosphere();

        var player = FindFirstObjectByType<PlayerController>();
        if (player) player.CanMove = false;

        CutscenePlayer.Instance?.FadeInGameplay(2.8f, () =>
        {
            StartCoroutine(PlayOpening());
        });
    }

    // ════════════════════════════════════════════════════════
    //  OPENING — retakan pertama muncul
    // ════════════════════════════════════════════════════════

    private IEnumerator PlayOpening()
    {
        yield return new WaitForSeconds(0.5f);

        // Crack kecil muncul saat opening
        crackEffect?.ShowCrackLevel1();

        yield return new WaitForSeconds(0.8f);

        var lines = new string[]
        {
            "...",
            "Lagi.",
            "Mengapa selalu lagi?",
            "Ada yang retak di sini. Aku bisa merasakannya.",
        };

        DialogueManager.Instance?.StartMonologue("Mono", lines, () =>
        {
            openingDone = true;
            var p = FindFirstObjectByType<PlayerController>();
            if (p) p.CanMove = true;
        });
    }

    // ════════════════════════════════════════════════════════
    //  IBU DI LUAR — sangat aneh, hampir seperti robot
    // ════════════════════════════════════════════════════════

    public void TriggerOutsideDialogue()
    {
        if (metMomOutside) return;

        StartCoroutine(OutsideDialogueWithGlitch());
    }

    private IEnumerator OutsideDialogueWithGlitch()
    {
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
                // Kata-kata persis sama — kali ini terasa robotik
                text    = "Ibu kangen sekali sama kamu. Kamu sehat, Nak?"
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "...",
                choices = new string[]
                {
                    "Bu, berhenti. Ada yang tidak beres.",   // Acceptance
                    "Iya Bu, aku sehat.",                    // Denial
                    "Maaf Bu... maaf aku jarang pulang.",    // Regret
                },
                onChoiceSelected = idx =>
                {
                    switch (idx)
                    {
                        case 0: EmotionFlagSystem.Instance?.AddAcceptance(2); break;
                        case 1: EmotionFlagSystem.Instance?.AddDenial(2);     break;
                        case 2: EmotionFlagSystem.Instance?.AddRegret(2);     break;
                    }
                }
            },
            new DialogueLine
            {
                speaker = "Ibu",
                // Ibu tidak responsif — langsung lanjut
                text    = "..."
            },
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "Masuk sana, sudah sore."
            },
        };

        DialogueManager.Instance?.StartDialogue(lines, () =>
        {
            metMomOutside = true;

            // Crack flash saat dialog selesai
            crackEffect?.FlashCrack(0.7f, 0.5f);

            if (doorTriggerEnter) doorTriggerEnter.SetActive(true);
        });

        // Glitch di tengah dialog
        yield return new WaitForSeconds(5f);
        atmosphere?.TriggerMicroGlitch();
        crackEffect?.FlashCrack(0.4f, 0.2f);
    }

    // ════════════════════════════════════════════════════════
    //  MASUK RUMAH
    // ════════════════════════════════════════════════════════

    public void OnEnteredHouse()
    {
        if (enteredHouse) return;
        enteredHouse = true;

        if (ibuOutside) ibuOutside.SetActive(false);
        if (ibuKitchen) ibuKitchen.SetActive(true);

        atmosphere?.IntensifyAtmosphere();

        // Crack level 2 saat masuk rumah
        if (!crack1Shown)
        {
            crack1Shown = true;
            StartCoroutine(ShowCrack2WithDelay());
        }
    }

    private IEnumerator ShowCrack2WithDelay()
    {
        yield return new WaitForSeconds(1.5f);

        // Dialog singkat saat masuk rumah dan lihat kondisi dalam
        var lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Rumah ini... terasa lebih sempit dari biasanya."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Atau mungkin hanya aku yang semakin sesak."
            },
        };

        DialogueManager.Instance?.StartDialogue(lines, () =>
        {
            crackEffect?.ShowCrackLevel2();
        });
    }

    // ════════════════════════════════════════════════════════
    //  DAPUR — dialog bercabang terpenting, sangat mempengaruhi ending
    // ════════════════════════════════════════════════════════

    public void TriggerKitchenDialogue()
    {
        if (talkedAtKitchen) return;

        StartCoroutine(KitchenDialogueRoutine());
    }

    private IEnumerator KitchenDialogueRoutine()
    {
        // Ibu diam sebentar sebelum bicara — efek aneh
        var player = FindFirstObjectByType<PlayerController>();
        if (player) player.CanMove = false;

        yield return new WaitForSeconds(1.5f);

        atmosphere?.TriggerMicroGlitch();

        yield return new WaitForSeconds(0.5f);

        if (player) player.CanMove = true;

        var lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "Ayo duduk dulu, Nak."
            },
            new DialogueLine
            {
                speaker = "Ibu",
                // Ibu diam sangat lama di sini — lihat coroutine di bawah
                text    = "..."
            },
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "Makanya sering pulang. Rumah ini selalu terbuka untukmu."
            },
            // ── Dialog bercabang utama ──
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Bu... aku ingin bicara sesuatu.",
                choices = new string[]
                {
                    "Aku tahu ibu sudah tidak ada.",          // Acceptance +3
                    "Tidak ada apa-apa. Tidak usah dibahas.", // Denial +3
                    "Maaf Bu, aku tidak pernah pulang.",      // Regret +3
                },
                onChoiceSelected = idx =>
                {
                    switch (idx)
                    {
                        case 0:
                            EmotionFlagSystem.Instance?.AddAcceptance(3);
                            StartCoroutine(AcceptanceResponse());
                            break;
                        case 1:
                            EmotionFlagSystem.Instance?.AddDenial(3);
                            StartCoroutine(DenialResponse());
                            break;
                        case 2:
                            EmotionFlagSystem.Instance?.AddRegret(3);
                            StartCoroutine(RegretResponse());
                            break;
                    }
                }
            },
        };

        DialogueManager.Instance?.StartDialogue(lines, null);

        // Glitch saat Ibu diam (line ke-2)
        yield return new WaitForSeconds(6f);
        atmosphere?.TriggerFullGlitch();
        crackEffect?.FlashCrack(0.8f, 0.6f);
    }

    // ── Response berdasarkan pilihan ──

    private IEnumerator AcceptanceResponse()
    {
        yield return new WaitForSeconds(0.3f);

        // Ibu bereaksi berbeda — seolah hampir tersadar
        crackEffect?.FlashCrack(0.9f, 0.8f);
        atmosphere?.TriggerFullGlitch();

        yield return new WaitForSecondsRealtime(1f);

        var lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "..."
            },
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "Ibu tahu kamu sibuk, Nak."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Maaf Bu. Maaf aku terlambat menyadarinya."
            },
        };

        DialogueManager.Instance?.StartDialogue(lines, () => talkedAtKitchen = true);
    }

    private IEnumerator DenialResponse()
    {
        yield return new WaitForSeconds(0.3f);

        var lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "Iya, Nak. Istirahat sana."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "(Aku tidak sanggup mengatakannya.)"
            },
        };

        DialogueManager.Instance?.StartDialogue(lines, () => talkedAtKitchen = true);
    }

    private IEnumerator RegretResponse()
    {
        yield return new WaitForSeconds(0.3f);

        crackEffect?.FlashCrack(0.7f, 0.5f);

        var lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Seharusnya aku lebih sering pulang."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Seharusnya aku ada di sana waktu itu."
            },
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "Istirahat sana, Nak. Sudah malam."
            },
        };

        DialogueManager.Instance?.StartDialogue(lines, () => talkedAtKitchen = true);
    }

    // ════════════════════════════════════════════════════════
    //  TIDUR — crack level 3, transisi ke Final Loop
    // ════════════════════════════════════════════════════════

    public void TriggerSleep()
    {
        if (!talkedAtKitchen)
        {
            DialogueManager.Instance?.StartMonologue("Mono",
                new string[] { "Ibu masih menunggu di dapur..." });
            return;
        }
        if (goneToBed) return;

        StartCoroutine(SleepRoutine());
    }

    private IEnumerator SleepRoutine()
    {
        goneToBed = true;

        // Crack level 3 saat mau tidur — paling intens sebelum reveal
        crackEffect?.ShowCrackLevel3();
        atmosphere?.TriggerFullGlitch();

        yield return new WaitForSecondsRealtime(0.8f);

        var lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Aku sudah tidak bisa berpura-pura lagi."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Setiap kali aku tidur, hari yang sama terulang."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Ibu... apakah kamu masih di sana?"
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Atau ini hanya yang aku inginkan?"
            },
        };

        DialogueManager.Instance?.StartDialogue(lines, () =>
        {
            // Save emotion flags
            EmotionFlagSystem.Instance?.SaveFlags();
            CheckpointManager.Instance?.SaveCheckpoint(3);
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
                fadeDuration      = 0.8f,
                openFadeDuration  = 1.0f,
                closeFadeDuration = 1.5f
            };

            CutscenePlayer.Instance?.PlayCutsceneThenLoadScene(data, finalLoopScene);
        }
        else
        {
            StartCoroutine(LoadFinalDirect());
        }
    }

    private IEnumerator LoadFinalDirect()
    {
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(finalLoopScene);
    }

    // ════════════════════════════════════════════════════════
    //  ROOM NOTIFY
    // ════════════════════════════════════════════════════════

    public void OnEnteredKitchen() => Debug.Log("[Loop3] Masuk dapur.");
    public void OnEnteredBedroom() => Debug.Log("[Loop3] Masuk kamar.");
}