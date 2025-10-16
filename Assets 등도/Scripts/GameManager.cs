using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty difficulty = Difficulty.Normal;

    [Header("Difficulty values")]
    public float easySpeed = 6f;
    public float normalSpeed = 9f;
    public float hardSpeed = 12f;
    public float easySpawn = 1.2f;
    public float normalSpawn = 0.8f;
    public float hardSpawn = 0.5f;
    public int easyReward = 10;
    public int normalReward = 30;
    public int hardReward = 100;

    public float roundDuration = 30f;

    [HideInInspector] public float CurrentObstacleSpeed = 9f;
    public ObstacleSpawner spawner;
    public UIManager uiManager;

    int p1Score = 0;
    int p2Score = 0;

    bool gameRunning = false;
    float timer = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ApplyDifficulty();
        uiManager.UpdateTotalScore(p1Score + p2Score);
        uiManager.UpdateDifficultyText(difficulty.ToString());
    }

    void ApplyDifficulty()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                CurrentObstacleSpeed = easySpeed;
                spawner.spawnInterval = easySpawn;
                break;
            case Difficulty.Normal:
                CurrentObstacleSpeed = normalSpeed;
                spawner.spawnInterval = normalSpawn;
                break;
            case Difficulty.Hard:
                CurrentObstacleSpeed = hardSpeed;
                spawner.spawnInterval = hardSpawn;
                break;
        }
    }

    public void StartRound()
    {
        if (gameRunning) return;
        // 重置玩家存活状态（若需要）
        var p1 = GameObject.FindWithTag("Player1")?.GetComponent<PlayerController>();
        var p2 = GameObject.FindWithTag("Player2")?.GetComponent<PlayerController>();
        if (p1 != null) p1.isAlive = true;
        if (p2 != null) p2.isAlive = true;

        timer = roundDuration;
        gameRunning = true;
        StartCoroutine(RoundTimer());
    }

    IEnumerator RoundTimer()
    {
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            uiManager.UpdateTimer(Mathf.CeilToInt(timer));
            yield return null;
        }
        EndRound();
    }

    void EndRound()
    {
        gameRunning = false;
        var p1 = GameObject.FindWithTag("Player1")?.GetComponent<PlayerController>();
        var p2 = GameObject.FindWithTag("Player2")?.GetComponent<PlayerController>();

        int reward = GetRewardByDifficulty();
        if (p1 != null && p1.isAlive) { p1Score += reward; }
        if (p2 != null && p2.isAlive) { p2Score += reward; }

        uiManager.ShowResult($"Round End! P1:+{(p1!=null && p1.isAlive?reward:0)} P2:+{(p2!=null && p2.isAlive?reward:0)}");
        uiManager.UpdateScores(p1Score, p2Score);
        uiManager.UpdateTotalScore(p1Score + p2Score);
    }

    int GetRewardByDifficulty()
    {
        switch (difficulty)
        {
            case Difficulty.Easy: return easyReward;
            case Difficulty.Normal: return normalReward;
            case Difficulty.Hard: return hardReward;
        }
        return normalReward;
    }

    public void PlayerHit(int playerId)
    {
        uiManager.ShowHit(playerId);
        var p1 = GameObject.FindWithTag("Player1")?.GetComponent<PlayerController>();
        var p2 = GameObject.FindWithTag("Player2")?.GetComponent<PlayerController>();
        if ((p1 == null || !p1.isAlive) && (p2 == null || !p2.isAlive))
        {
            StopAllCoroutines();
            EndRound();
        }
    }
}
