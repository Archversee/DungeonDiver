using UnityEngine.UI;
using LootLocker.Requests;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderBoardController : MonoBehaviour
{
    public InputField MemberID;
    public int PlayerScore;
    public int ID;
    public GameController gameController;
    int MaxScores = 8;
    public Text[] Entries;

    private void Start()
    {
        LootLockerSDKManager.StartSession("Player", (response) =>
        {
            if(response.success)
            {
                //Debug.Log("Success");
            }    
            else
            {
               // Debug.Log("Failed");
            }
        });
    }

    public void ShowScores()
    {
        LootLockerSDKManager.GetScoreList(ID, MaxScores, (response) =>
        {
            if (response.success)
            {
                LootLocker.Requests.LootLockerLeaderboardMember[] scores = response.items;

                for (int i = 0; i < scores.Length; i++)
                {
                    Entries[i].text = (scores[i].rank + ".   " + scores[i].member_id + " - " + scores[i].score);
                }

                if (scores.Length < MaxScores)
                {
                    for (int i = scores.Length; i < MaxScores; i++)
                    {
                        Entries[i].text = (i + 1).ToString() + ".   none";
                    }
                }
            }
            else
            {
                //Debug.Log("Failed");
            }
        });
    }

    public void SubmitScore()
    {
        PlayerScore = (int)gameController.Score;
        LootLockerSDKManager.SubmitScore(MemberID.text, PlayerScore, ID, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Success");
                ChangeSceneLoseScreen();
            }
            else
            {
                Debug.Log("Failed");
            }
        });
    }
    public void ChangeSceneLoseScreen()
    {
        SceneManager.LoadScene(1);
    }
}
