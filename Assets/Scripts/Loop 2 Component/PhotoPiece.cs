using UnityEngine;

/// <summary>
/// Satu potongan foto keluarga yang tersebar di seluruh rumah.
/// Saat player interact → masuk ke inventory + progress puzzle bertambah.
/// </summary>
public class PhotoPiece : MonoBehaviour, IInteractable
{
    [Header("— Identity —")]
    public int    pieceIndex = 0;      // 0-4, urutan potongan foto
    public string pieceName  = "Potongan Foto";
    public string hintText   = "[E] Ambil potongan foto";

    [Header("— Inspect Lines (saat pertama ditemukan) —")]
    [TextArea(2, 4)]
    public string[] inspectLines;

    [Header("— Sprites —")]
    public Sprite pieceSprite;         // Gambar potongan ini

    [Header("— State —")]
    public bool isCollected = false;

    [Header("— Visual —")]
    public SpriteRenderer glowEffect;  // Optional: efek glow di sekitar foto
    public GameObject     floatAnim;   // Optional: animasi mengambang

    // ─────────────────────────────────────────────────────────

    private void Start()
    {
        // Animasi glow/float jika ada
        if (glowEffect)
        {
            StartCoroutine(GlowPulse());
        }
    }

    // ════════════════════════════════════════════════════════
    //  IInteractable
    // ════════════════════════════════════════════════════════

    public string GetHintText() => isCollected ? "" : hintText;

    public void Interact()
    {
        if (isCollected) return;

        // Tampilkan inspect lines dulu
        if (inspectLines != null && inspectLines.Length > 0)
        {
            InspectManager.Instance?.OpenInspectWithLines(inspectLines, OnInspectClosed);
        }
        else
        {
            OnInspectClosed();
        }
    }

    // ════════════════════════════════════════════════════════
    //  SETELAH INSPECT DITUTUP
    // ════════════════════════════════════════════════════════

    private void OnInspectClosed()
    {
        // Notify puzzle manager
        PhotoPuzzleManager.Instance?.CollectPiece(this);

        // Sembunyikan object ini dari scene
        gameObject.SetActive(false);
    }

    // ════════════════════════════════════════════════════════
    //  GLOW PULSE ANIMATION
    // ════════════════════════════════════════════════════════

    private System.Collections.IEnumerator GlowPulse()
    {
        if (!glowEffect) yield break;

        float t = 0f;
        while (!isCollected)
        {
            t += Time.deltaTime * 2f;
            float alpha = (Mathf.Sin(t) + 1f) * 0.5f * 0.4f + 0.1f; // 0.1 - 0.5
            var c = glowEffect.color;
            c.a = alpha;
            glowEffect.color = c;
            yield return null;
        }
    }
}