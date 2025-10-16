using UnityEngine;
using TMPro;                 
using System.Collections;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI p1ScoreText;
    public TextMeshProUGUI p2ScoreText;
    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI resultText;

    void Start()
    {
        if (resultText != null) resultText.text = "";
    }

    public void UpdateTimer(int secondsLeft)
    {
        if (timerText != null)
            timerText.text = $"Time: {secondsLeft}s";
    }

    public void UpdateScores(int p1, int p2)
    {
        if (p1ScoreText != null) p1ScoreText.text = $"P1: {p1}";
        if (p2ScoreText != null) p2ScoreText.text = $"P2: {p2}";
    }

    public void UpdateTotalScore(int total)
    {
        if (totalScoreText != null)
            totalScoreText.text = $"Total: {total}";
    }

    public void UpdateDifficultyText(string text)
    {
        if (difficultyText != null)
            difficultyText.text = text;
    }

    public void ShowResult(string text)
    {
        if (resultText != null)
        {
            resultText.text = text;
            StopAllCoroutines();
            StartCoroutine(ClearResult(3f));
        }
    }

    public void ShowHit(int playerId)
    {
        if (resultText != null)
        {
            resultText.text = $"Player {playerId} hit!";
            StopAllCoroutines();
            StartCoroutine(ClearResult(1.5f));
        }
    }

    IEnumerator ClearResult(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (resultText != null) resultText.text = "";
    }
}
