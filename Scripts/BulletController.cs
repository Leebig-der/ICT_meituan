using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 10f;

    private GameManager gm;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        if (gm == null)
            Debug.LogWarning("[BulletController] 没找到 GameManager，命中将不加分。");
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        if (transform.position.y > 10f) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Target")) return;

        int addScore = 0;
        var name = collision.gameObject.name;
        if (name.Contains("Red")) addScore = 5;
        else if (name.Contains("Yellow")) addScore = 3;
        else if (name.Contains("Green")) addScore = 1;

        gm?.AddScore(addScore);

        Destroy(collision.gameObject);
        Destroy(gameObject);
    }
}
