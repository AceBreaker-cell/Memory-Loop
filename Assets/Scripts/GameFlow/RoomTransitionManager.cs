using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoomTransitionManager : MonoBehaviour
{
    public static RoomTransitionManager Instance;

    [Header("Fade")]
    public Image fadeOverlay;
    public float fadeDuration = 0.35f;

    public class TransitionData
    {
        public Vector3    spawnPos;
        public GameObject roomToEnable;
        public GameObject roomToDisable;
        public float      camMinX;
        public float      camMaxX;
        public float      camFixedY;
        public bool       notifyEnterHouse;
        public bool       notifyEnterKitchen;
        public bool       notifyEnterBedroom;
        public Action     onDone;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartEdgeTransition(TransitionData data)
        => StartCoroutine(DoEdgeTransition(data));

    public void Transition(
        Vector3 spawnPos, GameObject areaOn, GameObject areaOff,
        float camMin, float camMax, float camY, bool notifyEnter = false)
    {
        var data = new TransitionData
        {
            spawnPos         = spawnPos,
            roomToEnable     = areaOn,
            roomToDisable    = areaOff,
            camMinX          = camMin,
            camMaxX          = camMax,
            camFixedY        = camY,
            notifyEnterHouse = notifyEnter,
            onDone           = null
        };
        StartCoroutine(DoEdgeTransition(data));
    }

    private IEnumerator DoEdgeTransition(TransitionData data)
    {
        Time.timeScale = 1f;

        var player = FindFirstObjectByType<PlayerController>();
        var cam    = Camera.main?.GetComponent<CameraFollow>();
        var rb     = player?.GetComponent<Rigidbody2D>();

        if (player) player.CanMove = false;
        if (rb)     rb.linearVelocity = Vector2.zero;

        yield return StartCoroutine(Fade(0f, 1f));

        if (data.roomToDisable) data.roomToDisable.SetActive(false);
        if (data.roomToEnable)  data.roomToEnable.SetActive(true);

        if (player != null)
        {
            if (rb) rb.linearVelocity = Vector2.zero;
            player.transform.position = data.spawnPos;
            if (rb) rb.linearVelocity = Vector2.zero;
        }

        if (cam != null)
        {
            cam.SetBounds(data.camMinX, data.camMaxX);
            cam.SetFixedY(data.camFixedY);
            cam.SnapToTarget();
        }

        // Notify semua manager yang aktif
        NotifyManagers(data);

        yield return null;
        yield return StartCoroutine(Fade(1f, 0f));

        if (player)
        {
            player.CanMove = true;
            if (rb) rb.linearVelocity = Vector2.zero;
        }

        data.onDone?.Invoke();
    }

    private void NotifyManagers(TransitionData data)
    {
        if (data.notifyEnterHouse)
        {
            GameFlowManager.Instance?.OnEnteredHouse();
            Loop1Manager.Instance?.OnEnteredHouse();
            Loop2Manager.Instance?.OnEnteredHouse();
            Loop3Manager.Instance?.OnEnteredHouse();
        }
        if (data.notifyEnterKitchen)
        {
            GameFlowManager.Instance?.OnEnteredKitchen();
            Loop1Manager.Instance?.OnEnteredKitchen();
            Loop2Manager.Instance?.OnEnteredKitchen();
            Loop3Manager.Instance?.OnEnteredKitchen();
        }
        if (data.notifyEnterBedroom)
        {
            GameFlowManager.Instance?.OnEnteredBedroom();
            Loop1Manager.Instance?.OnEnteredBedroom();
            Loop2Manager.Instance?.OnEnteredBedroom();
            Loop3Manager.Instance?.OnEnteredBedroom();
        }
    }

    private IEnumerator Fade(float from, float to)
    {
        if (!fadeOverlay) yield break;
        fadeOverlay.gameObject.SetActive(true);
        var c = fadeOverlay.color;
        c.a = from; fadeOverlay.color = c;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / fadeDuration));
            fadeOverlay.color = c;
            yield return null;
        }
        c.a = to; fadeOverlay.color = c;
        if (to <= 0f) fadeOverlay.gameObject.SetActive(false);
    }
}