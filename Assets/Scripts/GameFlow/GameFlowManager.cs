using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;

    [Header("— Story State —")]
    public bool metMomOutside   = false;
    public bool enteredHouse    = false;
    public bool talkedAtKitchen = false;
    public bool goneToBed       = false;

    [Header("— NPC References —")]
    public GameObject ibuOutside;       // drag object "Ibu" di Hierarchy
    public GameObject ibuKitchen;       // drag object "Ibu_Kitchen"

    [Header("— Door Reference —")]
    public GameObject doorTriggerEnter; // drag DoorTrigger_Enter

    [Header("— Next Scene —")]
    public string loop1Scene = "Loop1Scene";

    [Header("— Cutscene Makan —")]
    public Sprite[] dinnerCutsceneImages;

    [Header("— Cutscene Tidur —")]
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

        // Setup NPC awal
        if (ibuOutside)      ibuOutside.SetActive(true);
        if (ibuKitchen)      ibuKitchen.SetActive(false);
        if (doorTriggerEnter) doorTriggerEnter.SetActive(false);

        // FIX: Fade in awal yang lambat dan sinematik
        // Player dikunci selama fade in
        var player = FindFirstObjectByType<PlayerController>();
        if (player) player.CanMove = false;

        CutscenePlayer.Instance?.FadeInGameplay(2.0f, () =>
        {
            // Setelah fade in selesai, unlock player
            // Ibu sudah ada di luar, player bisa jalan mendekati ibu
            if (player) player.CanMove = true;
        });
    }

    // ════════════════════════════════════════════════════════
    //  IBU DI LUAR — dipanggil oleh NpcInteractable
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
                text    = "Ibu kangen sekali sama kamu. Kamu sehat, Nak?"
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "...",
                choices = new string[]
                {
                    "Iya Bu, aku baik-baik saja.",
                    "Maaf Bu, aku terlalu sibuk."
                },
                onChoiceSelected = (idx) =>
                {
                    Debug.Log("Pilihan luar: " + idx);
                }
            },
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "Syukurlah... Ibu sudah masak kesukaan kamu. Masuk dulu yuk, sudah sore."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Iya Bu, maaf sudah lama tidak pulang."
            },
        };

        DialogueManager.Instance?.StartDialogue(lines, OnOutsideDialogueDone);
    }

    private void OnOutsideDialogueDone()
    {
        metMomOutside = true;
        if (doorTriggerEnter) doorTriggerEnter.SetActive(true);
        Debug.Log("Dialog ibu selesai. Pintu masuk aktif.");
    }

    // ════════════════════════════════════════════════════════
    //  MASUK RUMAH — dipanggil oleh DoorInteractable
    // ════════════════════════════════════════════════════════
    public void OnEnteredHouse()
    {
        if (enteredHouse) return;
        enteredHouse = true;

        if (ibuOutside)  ibuOutside.SetActive(false);
        if (ibuKitchen)  ibuKitchen.SetActive(true);

        Debug.Log("Masuk rumah. Ibu sekarang di dapur.");
    }

    // ════════════════════════════════════════════════════════
    //  IBU DI DAPUR — dipanggil oleh NpcInteractable
    // ════════════════════════════════════════════════════════
    public void TriggerKitchenDialogue()
    {
        if (talkedAtKitchen) return;

        if (dinnerCutsceneImages != null && dinnerCutsceneImages.Length > 0)
        {
            var frames = new CutsceneFrame[dinnerCutsceneImages.Length];
            for (int i = 0; i < dinnerCutsceneImages.Length; i++)
                frames[i] = new CutsceneFrame { image = dinnerCutsceneImages[i], duration = 3f };

            var data = new CutsceneData
            {
                frames            = frames,
                defaultDuration   = 3f,
                fadeDuration      = 0.5f,
                openFadeDuration  = 0.6f,
                closeFadeDuration = 0.8f
            };

            // FIX: PlayCutscene → selesai → fade in gameplay DULU → baru StartKitchenDialogue
            CutscenePlayer.Instance?.PlayCutscene(data, StartKitchenDialogue);
        }
        else
        {
            // Tidak ada gambar cutscene, langsung dialog
            StartKitchenDialogue();
        }
    }

    // FIX: Dialog dipanggil SETELAH fade in gameplay selesai (di dalam PlayCutscene onDone)
    private void StartKitchenDialogue()
    {
        var lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Alhamdullilah, kenyang bu. Masakkan ibu selalu yang terbaik."
            },
            new DialogueLine
            {
                speaker = "Ibu",
                text    = "Makanya sering pulang. Rumah ini selalu terbuka untukmu."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "Iya Bu... enak sekali masakan ibu."
            },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "...",
                choices = new string[]
                {
                    "Aku kangen suasana ini, Bu.",
                    "Aku cuma lelah, Bu.",
                    "Rasanya aku pernah mengalami ini..."
                },
                onChoiceSelected = idx => Debug.Log($"Dinner choice: {idx}")
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
    //  TIDUR — dipanggil oleh BedInteractable
    // ════════════════════════════════════════════════════════
    public void TriggerSleep()
    {
        if (!talkedAtKitchen)
        {
            DialogueManager.Instance?.StartMonologue("Mono",
                new string[] { "Sepertinya ibu sedang menunggu di dapur..." });
            return;
        }
        if (goneToBed) return;

        var lines = new DialogueLine[]
        {
            new DialogueLine { speaker = "Mono", text = "Suasana hari ini... sedikit aneh?" },
            new DialogueLine { speaker = "Mono", text = "Tapi tidak apa-apa, itu hanya perasaanku saja." },
            new DialogueLine { speaker = "Mono", text = "Mungkin aku terlalu lelah. Mungkin aku harus istirahat." },
        };

        DialogueManager.Instance?.StartDialogue(lines, () =>
        {
            goneToBed = true;
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
                fadeDuration      = 0.6f,
                openFadeDuration  = 0.8f,
                closeFadeDuration = 1.0f
            };

            // FIX: Langsung load scene setelah cutscene — tidak balik gameplay sama sekali
            CutscenePlayer.Instance?.PlayCutsceneThenLoadScene(data, loop1Scene);
        }
        else
        {
            // Tidak ada gambar, langsung title card lalu load
            StartCoroutine(LoadLoop1Direct());
        }
    }

    private IEnumerator LoadLoop1Direct()
    {
        // Fade to black
        var cp = CutscenePlayer.Instance;
        if (cp != null)
        {
            // Gunakan globalFade
            bool done = false;
            cp.FadeInGameplay(0f, () => done = true); // dummy, kita manual
        }

        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(loop1Scene);
    }

    // ════════════════════════════════════════════════════════
    //  NOTIFY DARI ROOM EDGE TRIGGER
    // ════════════════════════════════════════════════════════
    public void OnEnteredKitchen()
    {
        Debug.Log("[GameFlow] Masuk dapur.");
    }

    public void OnEnteredBedroom()
    {
        Debug.Log("[GameFlow] Masuk kamar.");
    }
}