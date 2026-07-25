using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("— Panel —")]
    public GameObject dialoguePanel;

    [Header("— Speaker —")]
    public TextMeshProUGUI speakerNameText;
    public Image           speakerPortraitLeft;
    public Image           speakerPortraitRight;

    [Header("— Dialogue Text —")]
    public TextMeshProUGUI dialogueText;

    [Header("— Choices —")]
    public GameObject      choicesPanel;
    public Button[]        choiceButtons;
    public TextMeshProUGUI[] choiceLabels;

    [Header("— Portraits —")]
    public Sprite portraitMono;
    public Sprite portraitIbu;

    [Header("— Typewriter —")]
    public float typeSpeed = 0.035f;

    // State
    public bool IsOpen { get; private set; }
    private bool _skipType;
    private int  _choiceResult = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        dialoguePanel.SetActive(false);
        choicesPanel.SetActive(false);
    }

    // ── Public ────────────────────────────────

    /// Jalankan array DialogueLine
    public void StartDialogue(DialogueLine[] lines, Action onDone = null)
    {
        if (IsOpen) return;
        StartCoroutine(RunDialogue(lines, onDone));
    }

    /// Jalankan monologue sederhana (string[])
    public void StartMonologue(string speaker, string[] lines, Action onDone = null)
    {
        if (IsOpen) return;
        var dl = new DialogueLine[lines.Length];
        for (int i = 0; i < lines.Length; i++)
            dl[i] = new DialogueLine { speaker = speaker, text = lines[i] };
        StartCoroutine(RunDialogue(dl, onDone));
    }

    // ── Coroutines ────────────────────────────

    private IEnumerator RunDialogue(DialogueLine[] lines, Action onDone)
    {
        IsOpen = true;
        SetPlayerMove(false);
        dialoguePanel.SetActive(true);

        foreach (var line in lines)
        {
            UpdatePortrait(line.speaker);
            speakerNameText.text = line.speaker;
            choicesPanel.SetActive(false);

            yield return StartCoroutine(Typewrite(line.text));

            if (line.choices != null && line.choices.Length > 0)
            {
                yield return StartCoroutine(WaitChoice(line.choices));
                line.onChoiceSelected?.Invoke(_choiceResult);
                _choiceResult = -1;
            }
            else
            {
                yield return WaitForAdvanceKey();
            }
        }

        dialoguePanel.SetActive(false);
        IsOpen = false;
        SetPlayerMove(true);
        onDone?.Invoke();
    }

    private IEnumerator Typewrite(string fullText)
    {
        _skipType = false;
        dialogueText.text = "";
        foreach (char c in fullText)
        {
            if (_skipType) { dialogueText.text = fullText; break; }
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    private IEnumerator WaitForAdvanceKey()
    {
        // Kalau masih ngetik, klik pertama = skip
        if (dialogueText.text.Length < GetCurrentLine())
        {
            yield return new WaitUntil(() => PressedAdvance());
            _skipType = true;
            yield return new WaitUntil(() => !PressedAdvance());
        }

        // Tunggu key up dulu biar tidak langsung skip
        yield return new WaitUntil(() => !PressedAdvance());
        yield return new WaitUntil(() =>  PressedAdvance());
    }

    private int GetCurrentLine() =>
        dialogueText.text?.Length ?? 0;

    private bool PressedAdvance() =>
        Input.GetKey(KeyCode.E)      ||
        Input.GetKey(KeyCode.Space)  ||
        Input.GetKey(KeyCode.Return);

    private IEnumerator WaitChoice(string[] choices)
    {
        _choiceResult = -1;
        choicesPanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            bool show = i < choices.Length;
            choiceButtons[i].gameObject.SetActive(show);
            if (!show) continue;
            int idx = i;
            choiceLabels[i].text = choices[i];
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => _choiceResult = idx);
        }

        yield return new WaitUntil(() => _choiceResult >= 0);
        choicesPanel.SetActive(false);
    }

    private void UpdatePortrait(string speaker)
    {
        speakerPortraitLeft.gameObject.SetActive(false);
        speakerPortraitRight.gameObject.SetActive(false);

        switch (speaker)
        {
            case "Mono":
                speakerPortraitLeft.sprite = portraitMono;
                speakerPortraitLeft.gameObject.SetActive(portraitMono != null);
                break;
            case "Ibu":
                speakerPortraitRight.sprite = portraitIbu;
                speakerPortraitRight.gameObject.SetActive(portraitIbu != null);
                break;
        }
    }

    private void SetPlayerMove(bool canMove)
    {
        var p = FindFirstObjectByType<PlayerController>();
        if (p) p.CanMove = canMove;
    }
}

// ── Data ─────────────────────────────────────
[System.Serializable]
public class DialogueLine
{
    public string   speaker;
    [TextArea(2, 3)]
    public string   text;
    public string[] choices;
    public Action<int> onChoiceSelected;
}