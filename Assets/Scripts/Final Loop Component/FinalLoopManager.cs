using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Final Loop — "Kebenaran"
/// Rumah retak parah. Lampu berkedip. Ibu menjadi bayangan/memori.
/// Puzzle terakhir: dialog kejujuran. Lalu reveal cutscene + ending.
/// </summary>
public class FinalLoopManager : MonoBehaviour
{
    public static FinalLoopManager Instance;

    [Header("— Story State —")]
    public bool openingDone       = false;
    public bool enteredHouse      = false;
    public bool ibuRevealDone     = false;
    public bool finalDialogueDone = false;
    public bool goneToBed         = false;

    [Header("— NPC —")]
    public GameObject ibuOutside;
    public GameObject ibuKitchen;
    // Ibu "bayangan" — versi transparan/ghostly untuk Final Loop
    public GameObject ibuGhost;

    [Header("— Door —")]
    public GameObject doorTriggerEnter;

    [Header("— Effects —")]
    public LoopAtmosphere atmosphere;
    public CrackEffect    crackEffect;
    public FlickerLight   flickerLight; // script untuk lampu berkedip

    [Header("— Reveal Cutscene —")]
    [Tooltip("Gambar-gambar cutscene saat kebenaran terungkap (dari kamu)")]
    public Sprite[] revealCutsceneImages;
    [Tooltip("Durasi tiap gambar reveal (detik)")]
    public float revealImageDuration = 4f;

    [Header("— Ending Cutscenes —")]
    [Tooltip("Gambar cutscene Ending A — Acceptance")]
    public Sprite[] endingACutsceneImages;
    [Tooltip("Gambar cutscene Ending B — Denial")]
    public Sprite[] endingBCutsceneImages;
    [Tooltip("Gambar cutscene Ending C — Regret")]
    public Sprite[] endingCCutsceneImages;
    [Tooltip("Gambar cutscene Secret Ending")]
    public Sprite[] endingSecretCutsceneImages;

    [Header("— Scene Names —")]
    public string endingAScene      = "Ending_Acceptance";
    public string endingBScene      = "Ending_Denial";
    public string endingCScene      = "Ending_Regret";
    public string endingSecretScene = "Ending_Secret";

    // ── Inventory check ──────────────────────────────────────
    [Header("— Secret Ending Requirement —")]
    [Tooltip("Jumlah memory item minimum untuk secret ending")]
    public int secretEndingMinItems = 5;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        Time.timeScale = 1f;

        // Load emotion flags dari semua loop sebelumnya
        EmotionFlagSystem.Instance?.LoadFlags();

        // Setup NPC awal — ibu ada di luar (terakhir kali)
        if (ibuOutside)       ibuOutside.SetActive(true);
        if (ibuKitchen)       ibuKitchen.SetActive(false);
        if (ibuGhost)         ibuGhost.SetActive(false);
        if (doorTriggerEnter) doorTriggerEnter.SetActive(false);

        // Atmosfer paling intens — hampir grayscale penuh
        atmosphere?.ActivateLoop3Atmosphere();

        // Crack langsung level 3 dari awal
        crackEffect?.ShowCrackLevel3();

        // Mulai lampu berkedip
        flickerLight?.StartFlicker();

        var player = FindFirstObjectByType<PlayerController>();
        if (player) player.CanMove = false;

        // Fade in sangat lambat — sinematik
        CutscenePlayer.Instance?.FadeInGameplay(3.5f, () =>
        {
            StartCoroutine(PlayOpening());
        });
    }

    // ════════════════════════════════════════════════════════
    //  OPENING — paling dramatis
    // ════════════════════════════════════════════════════════

    private IEnumerator PlayOpening()
    {
        yield return new WaitForSeconds(0.8f);

        // Glitch besar saat opening
        atmosphere?.TriggerFullGlitch();
        crackEffect?.FlashCrack(1f, 0.5f);

        yield return new WaitForSecondsRealtime(0.6f);

        var lines = new string[]
        {
            "Rumah ini...",
            "Sudah tidak sama.",
            "Dan aku tahu mengapa.",
            "Aku hanya belum berani mengatakannya.",
        };

        DialogueManager.Instance?.StartMonologue("Mono", lines, () =>
        {
            openingDone = true;
            var p = FindFirstObjectByType<PlayerController>();
            if (p) p.CanMove = true;
        });
    }

    // ════════════════════════════════════════════════════════
    //  IBU DI LUAR — kali terakhir, sudah seperti bayangan
    // ════════════════════════════════════════════════════════

    public void TriggerOutsideDialogue()
    {
        if (ibuRevealDone) return;
        StartCoroutine(FinalOutsideDialogue());
    }

    private IEnumerator FinalOutsideDialogue()
    {
        // Glitch saat mendekati Ibu
        atmosphere?.TriggerFullGlitch();
        crackEffect?.FlashCrack(0.9f, 0.4f);

        yield return new WaitForSecondsRealtime(0.5f);

        var lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = "Ibu",
                // Suara Ibu terasa jauh dan echo
                text    = "Mono..."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Ibu..."
            },
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "Kamu sudah pulang."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "...",
                // PUZZLE TERAKHIR — pilihan yang menentukan ending final
                choices = new string[]
                {
                    "Aku pulang, Bu.",                    // Acceptance
                    "Ibu masih ada, kan?",                // Denial
                    "Maaf aku tidak ada waktu itu.",      // Regret
                    "Aku rindu. Tapi aku harus pergi.",   // Secret (hanya muncul jika semua item)
                },
                onChoiceSelected = idx =>
                {
                    switch (idx)
                    {
                        case 0: EmotionFlagSystem.Instance?.AddAcceptance(5); break;
                        case 1: EmotionFlagSystem.Instance?.AddDenial(5);     break;
                        case 2: EmotionFlagSystem.Instance?.AddRegret(5);     break;
                        case 3: EmotionFlagSystem.Instance?.AddAcceptance(10); break;
                    }
                    StartCoroutine(AfterFinalChoice());
                }
            },
        };

        DialogueManager.Instance?.StartDialogue(lines, null);
    }

    private IEnumerator AfterFinalChoice()
    {
        yield return new WaitUntil(() => !DialogueManager.Instance.IsOpen);
        yield return new WaitForSecondsRealtime(0.5f);

        // Ibu menghilang — fade out
        if (ibuOutside)
        {
            var sr = ibuOutside.GetComponent<SpriteRenderer>();
            if (sr) yield return StartCoroutine(FadeSprite(sr, 1f, 0f, 1.5f));
            ibuOutside.SetActive(false);
        }

        // Tampilkan Ibu Ghost (transparan)
        if (ibuGhost) ibuGhost.SetActive(true);

        yield return new WaitForSecondsRealtime(0.5f);

        // Glitch besar + crack flash
        atmosphere?.TriggerFullGlitch();
        crackEffect?.FlashCrack(1f, 0.8f);

        yield return new WaitForSecondsRealtime(0.3f);

        var lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Ibu..."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Aku tahu."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Aku sudah tahu sejak awal.",
            },
        };

        DialogueManager.Instance?.StartDialogue(lines, () =>
        {
            ibuRevealDone = true;
            if (doorTriggerEnter) doorTriggerEnter.SetActive(true);
        });
    }

    // ════════════════════════════════════════════════════════
    //  MASUK RUMAH TERAKHIR
    // ════════════════════════════════════════════════════════

    public void OnEnteredHouse()
    {
        if (enteredHouse) return;
        enteredHouse = true;

        // Ibu ghost di dapur
        if (ibuKitchen) ibuKitchen.SetActive(false);
        if (ibuGhost)   ibuGhost.SetActive(true);

        atmosphere?.IntensifyAtmosphere();
        StartCoroutine(InsideMonologue());
    }

    private IEnumerator InsideMonologue()
    {
        yield return new WaitForSeconds(1.5f);

        var lines = new string[]
        {
            "Rumah ini... sudah lama kosong.",
            "Tapi aku terus kembali ke sini.",
            "Karena di sinilah terakhir kali aku melihat Ibu.",
        };

        DialogueManager.Instance?.StartMonologue("Mono", lines, null);
    }

    // ════════════════════════════════════════════════════════
    //  KASUR — dialog terakhir + trigger reveal cutscene
    // ════════════════════════════════════════════════════════

    public void TriggerSleep()
    {
        if (!ibuRevealDone)
        {
            DialogueManager.Instance?.StartMonologue("Mono",
                new string[] { "Ada yang belum selesai di luar..." });
            return;
        }
        if (goneToBed) return;
        goneToBed = true;

        StartCoroutine(FinalSleepRoutine());
    }

    private IEnumerator FinalSleepRoutine()
{
    goneToBed = true;

    // Dialog penutup sebelum reveal
    var lines = new DialogueLine[]
    {
        new DialogueLine { speaker = "Mono", text = "Ibu pergi tiga bulan yang lalu." },
        new DialogueLine { speaker = "Mono", text = "Dan aku tidak ada di sana." },
        new DialogueLine { speaker = "Mono", text = "Karena aku terlalu sibuk." },
        new DialogueLine { speaker = "Mono", text = "Jadi aku kembali ke sini." },
        new DialogueLine { speaker = "Mono", text = "Lagi dan lagi." },
        new DialogueLine { speaker = "Mono", text = "Karena di sini... Ibu masih hidup." },
    };

    DialogueManager.Instance?.StartDialogue(lines, () =>
    {
        EmotionFlagSystem.Instance?.SaveFlags();
        CheckpointManager.Instance?.SaveCheckpoint(99);
        StartRevealCutscene();
    });

    yield break; // ← INI YANG PENTING! Harus ada yield agar valid IEnumerator
}

    // ════════════════════════════════════════════════════════
    //  REVEAL CUTSCENE → ENDING
    // ════════════════════════════════════════════════════════

    private void StartRevealCutscene()
    {
        if (revealCutsceneImages != null && revealCutsceneImages.Length > 0)
        {
            var frames = new CutsceneFrame[revealCutsceneImages.Length];
            for (int i = 0; i < revealCutsceneImages.Length; i++)
                frames[i] = new CutsceneFrame
                {
                    image    = revealCutsceneImages[i],
                    duration = revealImageDuration
                };

            var data = new CutsceneData
            {
                frames            = frames,
                defaultDuration   = revealImageDuration,
                fadeDuration      = 1.0f,
                openFadeDuration  = 1.5f,
                closeFadeDuration = 2.0f
            };

            // Setelah reveal cutscene selesai → langsung ending cutscene
            CutscenePlayer.Instance?.PlayCutscene(data, GoToEnding);
        }
        else
        {
            GoToEnding();
        }
    }

    private void GoToEnding()
    {
        // Tentukan ending
        bool hasAllItems = InventoryManager.Instance != null &&
                           InventoryManager.Instance.GetItems().Count >= secretEndingMinItems;

        var emotionSystem = EmotionFlagSystem.Instance;
        if (emotionSystem == null) { LoadEndingScene(endingAScene); return; }

        var ending      = emotionSystem.GetEnding(hasAllItems);
        Sprite[] images = GetEndingImages(ending);
        string   scene  = GetEndingScene(ending);

        if (images != null && images.Length > 0)
        {
            var frames = new CutsceneFrame[images.Length];
            for (int i = 0; i < images.Length; i++)
                frames[i] = new CutsceneFrame { image = images[i], duration = 4f };

            var data = new CutsceneData
            {
                frames            = frames,
                defaultDuration   = 4f,
                fadeDuration      = 1.2f,
                openFadeDuration  = 1.5f,
                closeFadeDuration = 2.5f
            };

            CutscenePlayer.Instance?.PlayCutsceneThenLoadScene(data, scene);
        }
        else
        {
            LoadEndingScene(scene);
        }
    }

    private Sprite[] GetEndingImages(EmotionFlagSystem.EndingType ending)
    {
        return ending switch
        {
            EmotionFlagSystem.EndingType.Acceptance => endingACutsceneImages,
            EmotionFlagSystem.EndingType.Denial     => endingBCutsceneImages,
            EmotionFlagSystem.EndingType.Regret     => endingCCutsceneImages,
            EmotionFlagSystem.EndingType.Secret     => endingSecretCutsceneImages,
            _                                       => endingACutsceneImages
        };
    }

    private string GetEndingScene(EmotionFlagSystem.EndingType ending)
    {
        return ending switch
        {
            EmotionFlagSystem.EndingType.Acceptance => endingAScene,
            EmotionFlagSystem.EndingType.Denial     => endingBScene,
            EmotionFlagSystem.EndingType.Regret     => endingCScene,
            EmotionFlagSystem.EndingType.Secret     => endingSecretScene,
            _                                       => endingAScene
        };
    }

    private void LoadEndingScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(sceneName);
    }

    // ════════════════════════════════════════════════════════
    //  HELPERS
    // ════════════════════════════════════════════════════════

    private IEnumerator FadeSprite(SpriteRenderer sr, float from, float to, float duration)
    {
        float elapsed = 0f;
        Color c = sr.color;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            sr.color = c;
            yield return null;
        }
        c.a = to;
        sr.color = c;
    }

    public void OnEnteredKitchen() { }
    public void OnEnteredBedroom() { }
}