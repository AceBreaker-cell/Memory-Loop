using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Interact Hint")]
    public GameObject interactHintObject;
    public TextMeshProUGUI interactHintText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowInteractHint(bool show, string text = "[E] Lihat / [Space] Bicara")
    {
        interactHintObject.SetActive(show);
        if (interactHintText) interactHintText.text = text;
    }
}