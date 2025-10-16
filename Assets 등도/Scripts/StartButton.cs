using UnityEngine;

public class StartButton : MonoBehaviour
{
    public void OnStartButtonPressed()
    {
        GameManager.Instance.StartRound();
    }
}
