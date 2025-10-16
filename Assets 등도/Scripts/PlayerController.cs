using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public KeyCode jumpKey = KeyCode.W;    // 在 Inspector 可改为 UpArrow 等
    public int playerId = 1;               // 在 Inspector 分别设为 1/2
    public float jumpForce = 10f;
    public bool isAlive = true;

    Rigidbody2D rb;
    Collider2D col;
    Vector2 groundCheckOffset = new Vector2(0, -0.51f);
    float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!isAlive) return;

        if (Input.GetKeyDown(jumpKey) && IsGrounded())
        {
            DoJump();
        }
    }

    public void Jump() // for UI Button
    {
        if (!isAlive) return;
        if (IsGrounded())
            DoJump();
    }

    void DoJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    bool IsGrounded()
    {
        Vector2 pos = (Vector2)transform.position + groundCheckOffset;
        return Physics2D.OverlapCircle(pos, groundCheckRadius, groundLayer) != null;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAlive) return;

        if (collision.collider.CompareTag("Obstacle"))
        {
            isAlive = false;
            GameManager.Instance.PlayerHit(playerId);
        }
    }
}
