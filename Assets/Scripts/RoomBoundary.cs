using UnityEngine;

public class RoomBoundary : MonoBehaviour
{
    [Header("Batas Kamera untuk Room Ini")]
    public float cameraMinX;
    public float cameraMaxX;
    public float cameraFixedY;

    [Header("BGM Room Ini (opsional)")]
    public AudioClip roomBGM;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Update batas kamera
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam) cam.SetBounds(cameraMinX, cameraMaxX);

        // Ganti BGM jika ada
        if (roomBGM)
            AudioManager.Instance?.TransitionToMusic(roomBGM);
    }
}