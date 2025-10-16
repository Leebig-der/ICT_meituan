
using UnityEngine;
public class Obstacle : MonoBehaviour
{
    float speed;
    public void Init(float s) { speed = s; }
    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
        if (transform.position.x < -12f) Destroy(gameObject);
    }
}
