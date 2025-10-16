using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float spawnInterval = 0.9f; // 会由 GameManager 根据难度覆盖
    public float spawnY = -1.6f;
    float timer;

    void Start()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Spawn();
            timer = spawnInterval;
        }
    }

    void Spawn()
    {
        Vector3 spawnPos = new Vector3(12f, spawnY, 0f);
        var obj = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
        var mover = obj.GetComponent<ObstacleMover>();
        if (mover != null) mover.speed = GameManager.Instance.CurrentObstacleSpeed;
    }
}
