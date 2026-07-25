using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Interaction")]
    public float     interactRadius   = 1.5f;
    public LayerMask interactableLayer;

    public bool CanMove { get; set; } = true;

    private Rigidbody2D  _rb;
    private Animator     _anim;
    private float        _inputX;
    private bool         _facingRight  = true;
    private bool         _hasSpeedParam;

    private IInteractable _nearest;
    private string        _nearestHint = "";

    private void Awake()
    {
        _rb   = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        _rb.gravityScale = 0f;
        _rb.constraints  = RigidbodyConstraints2D.FreezePositionY
                         | RigidbodyConstraints2D.FreezeRotation;

        foreach (var p in _anim.parameters)
            if (p.name == "Speed") { _hasSpeedParam = true; break; }

        if (!_hasSpeedParam)
            Debug.LogWarning("[Player] Parameter 'Speed' tidak ada di Animator!");
    }

    private void Update()
    {
        if (!CanMove)
        {
            _rb.linearVelocity = Vector2.zero;
            if (_hasSpeedParam) _anim.SetFloat("Speed", 0f);
            return;
        }

        HandleMovement();
        DetectInteractable();
        HandleInteractInput();
    }

    private void FixedUpdate()
    {
        if (!CanMove) { _rb.linearVelocity = Vector2.zero; return; }
        _rb.linearVelocity = new Vector2(_inputX * moveSpeed, 0f);
    }

    // ─────────────────────────────────────────
    private void HandleMovement()
    {
        _inputX = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)  || Input.GetKey(KeyCode.A)) _inputX = -1f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) _inputX =  1f;

        if (_hasSpeedParam)
            _anim.SetFloat("Speed", Mathf.Abs(_inputX));

        if (_inputX > 0f && !_facingRight) Flip();
        if (_inputX < 0f &&  _facingRight) Flip();
    }

    private void DetectInteractable()
    {
        // Gunakan OverlapCircleAll — detect SEMUA collider di radius
        var hits = Physics2D.OverlapCircleAll(
            transform.position, interactRadius, interactableLayer);

        _nearest    = null;
        _nearestHint = "";
        float minDist = float.MaxValue;

        foreach (var h in hits)
        {
            // Cari IInteractable di object ini DAN parent-nya
            var iv = h.GetComponentInParent<IInteractable>();
            if (iv == null) iv = h.GetComponent<IInteractable>();

            if (iv == null)
            {
                Debug.Log($"[Player] '{h.name}' terdeteksi tapi tidak punya IInteractable");
                continue;
            }

            float d = Vector2.Distance(transform.position, h.transform.position);
            if (d < minDist)
            {
                minDist      = d;
                _nearest     = iv;
                _nearestHint = iv.GetHintText();
            }
        }

        // Update hint UI
        if (_nearest != null)
        {
            HUDManager.Instance?.ShowInteractHint(true, _nearestHint);
        }
        else
        {
            HUDManager.Instance?.ShowInteractHint(false);
        }
    }

    private void HandleInteractInput()
    {
        if (_nearest == null) return;
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"[Player] Interact dengan: {_nearest}");
            _nearest.Interact();
        }

        // DEBUG SEMENTARA — hapus setelah fix
if (Input.GetKeyDown(KeyCode.E))
{
    Debug.Log($"[E pressed] _nearest = {(_nearest != null ? _nearest.ToString() : "NULL")}");
    Debug.Log($"[E pressed] CanMove = {CanMove}");
    
    // Force detect ulang
    var allHits = Physics2D.OverlapCircleAll(transform.position, interactRadius);
    Debug.Log($"[E pressed] Total collider di radius (semua layer): {allHits.Length}");
    foreach (var h in allHits)
        Debug.Log($"  - {h.name} | Layer: {LayerMask.LayerToName(h.gameObject.layer)}");
}
    }

    private void Flip()
    {
        _facingRight = !_facingRight;
        var s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}