using UnityEngine;

public class GameInterface : MonoBehaviour
{
    public GameJudge judge;

    // 方便外部（菜单/场馆串联）直接调用

    /// <summary>
    /// 设置难度（"easy"/"medium"/"hard"）
    /// </summary>
    public void SetDifficulty(string level)
    {
        if (judge != null) judge.SetDifficultyAll(level);
    }

    /// <summary>
    /// 开始/重开一局
    /// </summary>
    public void StartMatch()
    {
        if (judge != null) judge.StartGame();
    }

    /// <summary>
    /// 读取谁赢了（"P1"/"P2"/"None"）
    /// </summary>
    public string ReadWinner()
    {
        return (judge != null) ? judge.GetWinner() : "None";
    }

    /// <summary>
    /// 替换按键（例如：P1->"a" / P2->"l"）
    /// </summary>
    public void SetKeys(string p1, string p2)
    {
        if (judge == null) return;
        if (judge.player1 != null && !string.IsNullOrEmpty(p1)) judge.player1.SetKey(p1);
        if (judge.player2 != null && !string.IsNullOrEmpty(p2)) judge.player2.SetKey(p2);
    }

    /// <summary>
    /// 设置困难模式的漏气速度（按键等效/秒）
    /// </summary>
    public void SetHardLeak(float pressesPerSecond)
    {
        if (judge == null) return;
        if (judge.player1 != null) judge.player1.SetHardLeak(pressesPerSecond);
        if (judge.player2 != null) judge.player2.SetHardLeak(pressesPerSecond);
    }

    /// <summary>
    /// 读取两个玩家的进度（0~1）
    /// </summary>
    public Vector2 ReadProgress()
    {
        if (judge == null) return Vector2.zero;
        float p1 = (judge.player1 != null) ? judge.player1.GetProgress() : 0f;
        float p2 = (judge.player2 != null) ? judge.player2.GetProgress() : 0f;
        return new Vector2(p1, p2);
    }
}
