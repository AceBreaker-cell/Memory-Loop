using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Speed")]
    public float smoothSpeed = 8f;

    [Header("Bounds")]
    public float minX = -50f;
    public float maxX =  50f;

    [Header("Fixed Y")]
    public float fixedY = 0f;

    private bool _initialized = false;

    private void Start()
    {
        // Auto-set dari posisi kamera saat start
        if (!_initialized)
        {
            fixedY = transform.position.y;
            _initialized = true;
        }

        if (target != null) SnapToTarget();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        float tx = Mathf.Clamp(target.position.x, minX, maxX);
        Vector3 goal = new Vector3(tx, fixedY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, goal,
                                           smoothSpeed * Time.deltaTime);
    }

    public void SetBounds(float min, float max)
    {
        minX = min;
        maxX = max;
    }

    public void SetFixedY(float y)
    {
        fixedY = y;
    }

    public void SnapToTarget()
    {
        if (target == null) return;
        float tx = Mathf.Clamp(target.position.x, minX, maxX);
        transform.position = new Vector3(tx, fixedY, transform.position.z);
    }
}