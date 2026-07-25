using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("— Interact Hint —")]
    public GameObject      hintRoot;       // drag Hint_Root (parent panel)
    public TextMeshProUGUI hintText;       // drag Text_Hint (TMP text)

    [Header("— Inventory —")]
    public GameObject      inventoryPanel;
    public TextMeshProUGUI inventoryBadge;

    [Header("— Buttons —")]
    public Button pauseButton;
    public Button inventoryButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Sembunyikan hint saat start
        if (hintRoot) hintRoot.SetActive(false);

        // Sembunyikan inventory saat start
        if (inventoryPanel) inventoryPanel.SetActive(false);

        // Badge inventory
        if (inventoryBadge) inventoryBadge.gameObject.SetActive(false);

        // Setup button listeners
        if (pauseButton)
            pauseButton.onClick.AddListener(
                () => PauseMenuManager.Instance?.TogglePause());

        if (inventoryButton)
            inventoryButton.onClick.AddListener(ToggleInventory);
    }

    // ── Dipanggil oleh PlayerController setiap frame ──
    public void ShowInteractHint(bool show, string text = "")
    {
        if (hintRoot == null) return;

        hintRoot.SetActive(show);

        if (show && hintText != null && text != "")
            hintText.text = text;
    }

    // ── Inventory ──
    public void ToggleInventory()
    {
        if (inventoryPanel == null) return;
        bool nowOpen = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(nowOpen);

        // Lock player saat inventory terbuka
        var player = FindFirstObjectByType<PlayerController>();
        if (player) player.CanMove = !nowOpen;
    }

    public void RefreshInventoryBadge(int count)
    {
        if (inventoryBadge == null) return;
        inventoryBadge.gameObject.SetActive(count > 0);
        inventoryBadge.text = count.ToString();
    }
}