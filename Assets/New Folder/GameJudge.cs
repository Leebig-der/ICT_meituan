using UnityEngine;
using TMPro;

public class GameJudge : MonoBehaviour
{
    [Header("Players")]
    public BlowToExplodeTap player1;
    public BlowToExplodeTap player2;

    [Header("Win UI")]
    public GameObject winPanel;         // 胜利提示框面板
    public TextMeshProUGUI winText;     // 面板里的文字

    private bool finished = false;
    private string winner = "None";     // "P1" / "P2" / "None"

    void Awake()
    {
        // 运行时确保面板是隐藏的
        if (winPanel != null) winPanel.SetActive(false);
    }

    /// <summary>
    /// 被玩家脚本回调：谁先爆了
    /// </summary>
    public void OnPlayerExploded(BlowToExplodeTap who)
    {
        if (finished) return;
        finished = true;

        if (who == player1) winner = "P1";
        else if (who == player2) winner = "P2";
        else winner = "None";

        string msg = (winner == "P1") ? "P1 Victory!" : "P2 Victory!";
        if (winText != null) winText.text = msg;
        if (winPanel != null) winPanel.SetActive(true);

        // 暂停
        Time.timeScale = 0f;
        Debug.Log($"Winner: {winner}");
    }

    /// <summary>
    /// 重置整局：恢复气球、隐藏面板、清空胜负、恢复时间
    /// </summary>
    [ContextMenu("🔁 Reset Game")]
    public void ResetGame()
    {
        finished = false;
        winner = "None";

        if (player1 != null) player1.ResetBalloon();
        if (player2 != null) player2.ResetBalloon();

        if (winPanel != null) winPanel.SetActive(false);

        // 恢复运行
        Time.timeScale = 1f;

        Debug.Log("Game reset.");
    }

    /// <summary>
    /// 开始一局（相当于 Reset 并开跑）
    /// </summary>
    [ContextMenu("▶ Start Game")]
    public void StartGame()
    {
        ResetGame();
        Debug.Log("Game started.");
    }

    /// <summary>
    /// 获取胜利者："P1" / "P2" / "None"
    /// </summary>
    public string GetWinner()
    {
        return winner;
    }

    /// <summary>
    /// 统一设置两个玩家的难度（给菜单/外部系统调用）
    /// </summary>
    public void SetDifficultyAll(string level)
    {
        if (player1 != null) player1.SetDifficultyByName(level);
        if (player2 != null) player2.SetDifficultyByName(level);
        Debug.Log($"Difficulty set to {level}");
    }

    // ======== 👇 方便调试的右键快捷功能 ========

    [ContextMenu("⚙ Set Difficulty (Easy)")]
    public void SetDifficultyEasy()
    {
        SetDifficultyAll("easy");
    }

    [ContextMenu("⚙ Set Difficulty (Medium)")]
    public void SetDifficultyMedium()
    {
        SetDifficultyAll("medium");
    }

    [ContextMenu("⚙ Set Difficulty (Hard)")]
    public void SetDifficultyHard()
    {
        SetDifficultyAll("hard");
    }
}
