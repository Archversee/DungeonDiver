using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gamelostscreen : MonoBehaviour
{
    public LeaderBoardController leaderBoard;
    // Start is called before the first frame update
    void Start()
    {
        leaderBoard.ShowScores();
    }

    public void ChangeSceneGameScreen()
    {
        SceneManager.LoadScene(1);
    }

    public void ChangeSceneMenuScreen()
    {
        SceneManager.LoadScene(0);
    }
}
