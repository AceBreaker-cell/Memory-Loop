using UnityEngine;

/// <summary>
/// Sistem checkpoint sederhana menggunakan PlayerPrefs.
/// Menyimpan loop yang terakhir dicapai player.
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private const string KEY_LOOP = "checkpoint_loop";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ════════════════════════════════════════════════════════
    //  SAVE
    // ════════════════════════════════════════════════════════

    /// Simpan checkpoint pada loop ke-N
    public void SaveCheckpoint(int loopNumber)
    {
        PlayerPrefs.SetInt(KEY_LOOP, loopNumber);
        PlayerPrefs.Save();
        Debug.Log($"[Checkpoint] Disimpan: Loop {loopNumber}");
    }

    // ════════════════════════════════════════════════════════
    //  LOAD
    // ════════════════════════════════════════════════════════

    /// Ambil loop terakhir yang tersimpan (0 jika belum ada)
    public int LoadLastLoop()
    {
        return PlayerPrefs.GetInt(KEY_LOOP, 0);
    }

    /// Apakah ada checkpoint tersimpan?
    public bool HasCheckpoint()
    {
        return PlayerPrefs.HasKey(KEY_LOOP);
    }

    // ════════════════════════════════════════════════════════
    //  CLEAR
    // ════════════════════════════════════════════════════════

    public void ClearCheckpoint()
    {
        PlayerPrefs.DeleteKey(KEY_LOOP);
        PlayerPrefs.Save();
        Debug.Log("[Checkpoint] Dihapus.");
    }

    /// Nama scene yang sesuai dengan loop number
    public string GetSceneNameForLoop(int loop)
    {
        return loop switch
        {
            0 => "Loop0Scene",
            1 => "Loop1Scene",
            2 => "Loop2Scene",
            3 => "Loop3Scene",
            _ => "Loop0Scene"
        };
    }
}