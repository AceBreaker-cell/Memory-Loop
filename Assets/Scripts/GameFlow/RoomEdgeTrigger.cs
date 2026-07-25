using UnityEngine;

public class RoomEdgeTrigger : MonoBehaviour
{
    [Header("— Room Tujuan —")]
    public GameObject roomToEnable;
    public GameObject roomToDisable;

    [Header("— Spawn Point —")]
    public Transform spawnPoint;

    [Header("— Camera Bounds —")]
    public float camMinX;
    public float camMaxX;
    public float camFixedY;

    [Header("— Notify —")]
    public bool notifyEnterHouse   = false;
    public bool notifyEnterKitchen = false;
    public bool notifyEnterBedroom = false;

    private bool _triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;

        _triggered = true;

        // Kumpulkan semua data SEBELUM object ini mungkin ke-disable
        var data = new RoomTransitionManager.TransitionData
        {
            spawnPos         = spawnPoint != null ? spawnPoint.position : transform.position,
            roomToEnable     = this.roomToEnable,
            roomToDisable    = this.roomToDisable,
            camMinX          = this.camMinX,
            camMaxX          = this.camMaxX,
            camFixedY        = this.camFixedY,
            notifyEnterHouse   = this.notifyEnterHouse,
            notifyEnterKitchen = this.notifyEnterKitchen,
            notifyEnterBedroom = this.notifyEnterBedroom,
            onDone           = () => _triggered = false
        };

        // Serahkan ke RoomTransitionManager yang tidak akan ke-disable
        RoomTransitionManager.Instance?.StartEdgeTransition(data);
    }
}