using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public float speed = 8f;

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        if (transform.position.x < -20f) Destroy(gameObject);
    }
}
