
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float jumpForce = 9.5f;
    public float CurrentSpeed { get; private set; } = 6f;

    Rigidbody2D rb;
    int jumpCount = 0;
    const int maxJump = 2;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetMoveSpeed(float s) => CurrentSpeed = s;

    public void ResetPlayer(Vector3 pos, float gravity)
    {
        transform.position = pos;
        rb.velocity = Vector2.zero;
        rb.gravityScale = gravity;
        jumpCount = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space))
            TryJump();

        // variable jump height
        if (rb.velocity.y > 0 && (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space)))
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

        if (transform.position.y < -6f)
            FindObjectOfType<GameManager>().GameOver();
    }

    void TryJump()
    {
        if (jumpCount < maxJump)
        {
            float force = jumpCount == 0 ? jumpForce : jumpForce * 0.85f;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            jumpCount++;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.collider.isTrigger) jumpCount = 0;
        if (other.collider.GetComponent<Obstacle>() != null)
            FindObjectOfType<GameManager>().GameOver();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        var coin = col.GetComponent<Coin>();
        if (coin != null)
        {
            FindObjectOfType<GameManager>().AddCoin(1);
            Destroy(coin.gameObject);
        }
    }
}
