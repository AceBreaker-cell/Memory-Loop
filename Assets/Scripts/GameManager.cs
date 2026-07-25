using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Scene State")]
    public int currentLoop = 0; // 0 = normal, 1 = deja vu, dll.

    [Header("Story Progression")]
    public bool hasMetMomAtDoor = false;
    public bool hasEatenDinner  = false;
    public bool hasGoneToSleep  = false;

    [Header("References")]
    public Image fadeOverlay;
    public string nextLoopScene = "Loop1Scene";

    // Dialogue data untuk pertemuan di depan pintu
    private DialogueLine[] _doorDialogue;
    private DialogueLine[] _dinnerDialogue;
    private DialogueLine[] _bedDialogue;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        BuildDialogues();
        // Trigger dialog ibu di depan pintu setelah fade in
        StartCoroutine(OpeningSequence());
    }

    private void BuildDialogues()
    {
        // ── Dialog Depan Pintu ──
        _doorDialogue = new DialogueLine[]
        {
            new DialogueLine { speaker = "Ibu", text = "Mono!" },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "...",
                choices = new[] { "Ibu?", "Siapa itu?" },
                onChoiceSelected = (idx) => {
                    // Nanti bisa pengaruhi emotion flag
                    Debug.Log($"Player pilih: {idx}");
                }
            },
            new DialogueLine { speaker = "Ibu", text = "..." },
        };

        // ── Dialog Makan Malam ──
        _dinnerDialogue = new DialogueLine[]
        {
            new DialogueLine { speaker = "Ibu", text = "Makanya sering pulang, Nak. Rumah selalu terbuka untukmu." },
            new DialogueLine { speaker = "Mono", text = "Iya Bu... Enak sekali masakan ibu." },
            new DialogueLine { speaker = "Ibu", text = "Ibu masak ini sudah dari siang. Ingat, ini kesukaan kamu waktu kecil." },
            new DialogueLine
            {
                speaker = "Mono",
                text    = "...",
                choices = new[] { "Aku kangen suasana ini, Bu.", "Aku cuma lelah, Bu.", "Rasanya aku pernah mengalami ini." },
                onChoiceSelected = (idx) => {
                    Debug.Log($"Dinner choice: {idx}");
                }
            },
            new DialogueLine { speaker = "Ibu", text = "Istirahat dulu sana, Nak. Sudah malam." },
        };

        // ── Dialog Kasur / Tidur ──
        _bedDialogue = new DialogueLine[]
        {
            new DialogueLine { speaker = "Mono", text = "Kasur ini... masih sama seperti dulu." },
            new DialogueLine { speaker = "Mono", text = "Sepertinya ada yang aneh, tetapi apa ya?" },
            new DialogueLine { speaker = "Mono", text = "..." },
            new DialogueLine { speaker = "Mono", text = "(Mono pejamkan mata...)" },
        };
    }

    // ─────────────────────────────────────────
    //  ALUR UTAMA OPENING
    // ─────────────────────────────────────────

    private IEnumerator OpeningSequence()
    {
        // Fade in scene
        yield return StartCoroutine(FadeScene(1f, 0f, 1.2f));

        // Player tidak bisa gerak dulu
        PlayerController player = FindFirstObjectByType<PlayerController>();

        yield return new WaitForSeconds(0.5f);

        if (!hasMetMomAtDoor)
        {
            // Kunci gerak saat dialog
            DialogueManager.Instance.StartDialogue(_doorDialogue, () =>
            {
                hasMetMomAtDoor = true;
                // Pintu terbuka: enable room transition
                FindFirstObjectByType<RoomTransition>()?.gameObject.SetActive(true);
            });
        }
    }

    // ─────────────────────────────────────────
    //  DIPANGGIL OLEH KitchenTrigger
    // ─────────────────────────────────────────
    public void TriggerKitchenDialogue()
    {
        if (hasEatenDinner) return;
        DialogueManager.Instance.StartDialogue(_dinnerDialogue, () =>
        {
            hasEatenDinner = true;
        });
    }

    // ─────────────────────────────────────────
    //  DIPANGGIL OLEH BedTrigger / BedObj
    // ─────────────────────────────────────────
    public void TriggerBedInteraction()
    {
        if (!hasEatenDinner) return; // harus makan dulu
        if (hasGoneToSleep)  return;

        DialogueManager.Instance.StartDialogue(_bedDialogue, () =>
        {
            hasGoneToSleep = true;
            StartCoroutine(TransitionToLoop1());
        });
    }

    private IEnumerator TransitionToLoop1()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeScene(0f, 1f, 1.5f));
        SceneManager.LoadScene(nextLoopScene);
    }

    // ─────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────

    private IEnumerator FadeScene(float from, float to, float duration)
    {
        if (!fadeOverlay) yield break;
        fadeOverlay.gameObject.SetActive(true);
        Color c = Color.black;
        c.a = from;
        fadeOverlay.color = c;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            fadeOverlay.color = c;
            yield return null;
        }
        c.a = to;
        fadeOverlay.color = c;
        if (to == 0f) fadeOverlay.gameObject.SetActive(false);
    }
}