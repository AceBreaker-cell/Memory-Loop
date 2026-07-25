using UnityEngine;

/// <summary>
/// Sistem flag emosi tersembunyi.
/// Setiap pilihan dialog mempengaruhi nilai Denial, Regret, atau Acceptance.
/// Nilai tertinggi di akhir game menentukan ending mana yang didapat.
/// </summary>
public class EmotionFlagSystem : MonoBehaviour
{
    public static EmotionFlagSystem Instance;

    [Header("— Emotion Values (tersembunyi dari player) —")]
    public int denial     = 0;  // Mono menghindari kebenaran
    public int regret     = 0;  // Mono larut dalam penyesalan
    public int acceptance = 0;  // Mono mulai menerima

    [Header("— Debug (lihat nilai di Inspector saat play) —")]
    public bool showDebug = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // ════════════════════════════════════════════════════════
    //  ADD POINTS
    // ════════════════════════════════════════════════════════

    public void AddDenial(int amount = 1)
    {
        denial += amount;
        if (showDebug) Debug.Log($"[Emotion] Denial: {denial} | Regret: {regret} | Acceptance: {acceptance}");
    }

    public void AddRegret(int amount = 1)
    {
        regret += amount;
        if (showDebug) Debug.Log($"[Emotion] Denial: {denial} | Regret: {regret} | Acceptance: {acceptance}");
    }

    public void AddAcceptance(int amount = 1)
    {
        acceptance += amount;
        if (showDebug) Debug.Log($"[Emotion] Denial: {denial} | Regret: {regret} | Acceptance: {acceptance}");
    }

    // ════════════════════════════════════════════════════════
    //  DETERMINE ENDING
    // ════════════════════════════════════════════════════════

    public enum EndingType { Acceptance, Denial, Regret, Secret }

    /// Tentukan ending berdasarkan nilai flag tertinggi
    public EndingType GetEnding(bool hasAllMemoryItems)
    {
        // Secret ending: semua memory item terkumpul
        if (hasAllMemoryItems && acceptance >= 3)
            return EndingType.Secret;

        // Cari nilai tertinggi
        if (acceptance >= denial && acceptance >= regret)
            return EndingType.Acceptance;
        if (denial >= regret)
            return EndingType.Denial;

        return EndingType.Regret;
    }

    public string GetEndingSceneName(EndingType ending)
    {
        return ending switch
        {
            EndingType.Acceptance => "Ending_Acceptance",
            EndingType.Denial     => "Ending_Denial",
            EndingType.Regret     => "Ending_Regret",
            EndingType.Secret     => "Ending_Secret",
            _                     => "Ending_Acceptance"
        };
    }

    // ════════════════════════════════════════════════════════
    //  SAVE / LOAD (PlayerPrefs sederhana)
    // ════════════════════════════════════════════════════════

    public void SaveFlags()
    {
        PlayerPrefs.SetInt("emotion_denial",     denial);
        PlayerPrefs.SetInt("emotion_regret",     regret);
        PlayerPrefs.SetInt("emotion_acceptance", acceptance);
        PlayerPrefs.Save();
    }

    public void LoadFlags()
    {
        denial     = PlayerPrefs.GetInt("emotion_denial",     0);
        regret     = PlayerPrefs.GetInt("emotion_regret",     0);
        acceptance = PlayerPrefs.GetInt("emotion_acceptance", 0);
    }

    public void ResetFlags()
    {
        denial = regret = acceptance = 0;
        PlayerPrefs.DeleteKey("emotion_denial");
        PlayerPrefs.DeleteKey("emotion_regret");
        PlayerPrefs.DeleteKey("emotion_acceptance");
    }
}