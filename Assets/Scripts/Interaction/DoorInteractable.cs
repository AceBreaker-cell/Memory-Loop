using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [Header("— Target —")]
    public Transform  spawnPoint;
    public GameObject areaToEnable;
    public GameObject areaToDisable;

    [Header("— Camera Bounds —")]
    public float camMinX;
    public float camMaxX;
    public float camFixedY;

    [Header("— Notify GameFlow —")]
    public bool notifyEnterHouse = false;

    [Header("— Hint —")]
    public string hintText = "[E] Masuk";

    public string GetHintText() => hintText;

    public void Interact()
    {
        Debug.Log("[Door] Interact() dipanggil!");

        if (RoomTransitionManager.Instance == null)
        {
            Debug.LogError("[Door] RoomTransitionManager.Instance adalah NULL!");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("[Door] spawnPoint belum diisi!");
            return;
        }

        if (areaToEnable == null)
        {
            Debug.LogError("[Door] areaToEnable belum diisi!");
            return;
        }

        Debug.Log("[Door] Semua field OK, memanggil Transition...");

        RoomTransitionManager.Instance.Transition(
            spawnPoint.position,
            areaToEnable,
            areaToDisable,
            camMinX,
            camMaxX,
            camFixedY,
            notifyEnterHouse
        );
    }
}