using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameController : MonoBehaviour
{
    public RoomFirstDunGen roomFirstDunGen;
    [SerializeField]
    protected TileMapVisualizer tileMapVisualizer = null;
    EnemyManager m_EnemyManager = EnemyManager.Singleton;
    ConsumableManager m_consumableManager = ConsumableManager.Singleton;

    GameObject[] Consumables;
    GameObject[] Deletables;

    private NavMeshSurface2d navMeshSurfaces;

    public Transform player;

    public float Score; //1
    public Text ScoreText;
    public int levelCount; //2
    public Text levelText;

    public Transform MenuUI;
    public Transform SaveScoreUI;

    // Start is called before the first frame update
    void Start()
    {
        navMeshSurfaces = FindObjectOfType<NavMeshSurface2d>();
        navMeshSurfaces.BuildNavMesh();
        //InvokeRepeating("doubleUpdateNavMesh", 0f, 1.0f);

        tileMapVisualizer.Clear();
        m_EnemyManager.ClearList();
        roomFirstDunGen.CreateRooms();

        Vector3 temppos = player.position;
        temppos.x = roomFirstDunGen.StartingPos.x +1;
        temppos.y = roomFirstDunGen.StartingPos.y + 1;
        player.position = temppos;
        doubleUpdateNavMesh();
        Score = 0f;
        levelCount = 1;

        string path = Application.persistentDataPath + "/player.data";
        if (File.Exists(path))
        {
            MenuUI.gameObject.SetActive(true);
        }
    }
    void doubleUpdateNavMesh()
    {
        navMeshSurfaces.UpdateNavMesh(navMeshSurfaces.navMeshData);
        navMeshSurfaces.UpdateNavMesh(navMeshSurfaces.navMeshData);
    }

    public void FixedUpdate()
    {
        ScoreText.text = "Score: " + Score.ToString();
        levelText.text = "Level: " + levelCount.ToString();
    }

    private void Update()
    {
        if (player.GetComponent<Health>().currhealth <= 0)
        {
            SaveScoreUI.gameObject.SetActive(true);
            SaveScoreUI.Find("Score").GetComponent<Text>().text = ScoreText.text;
            //ChangeSceneLoseScreen();
        }
    }

    public void ChangeSceneLoseScreen()
    {
        SceneManager.LoadScene(1);
    }

    public void NewMap()
    {
        levelCount++;
        tileMapVisualizer.Clear();
        m_EnemyManager.ClearList();
        Consumables = GameObject.FindGameObjectsWithTag("Consumable");
        Deletables = GameObject.FindGameObjectsWithTag("Environmentals");
        foreach (GameObject consum in Consumables)
        {
            Destroy(consum);
        }
        foreach (GameObject delete in Deletables)
        {
            Destroy(delete);
        }

        roomFirstDunGen.CreateRooms();
        doubleUpdateNavMesh();
        Score += 5f;


        Vector3 temppos = player.position;
        temppos.x = roomFirstDunGen.StartingPos.x + 1;
        temppos.y = roomFirstDunGen.StartingPos.y + 1;
        player.position = temppos;

        //auto save every new level
        SaveSystem.SavePlayer(player.GetComponent<playerMovement>(), this);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        Score = data.score;
        levelCount = data.levelcount - 1;

        player.GetComponent<playerMovement>().speed = data.speed;
        player.GetComponent<playerMovement>().attackrange = data.attackrange;
        player.GetComponent<playerMovement>().attackdamage = data.attackdamage;
        player.GetComponent<playerMovement>().attackcooldown = data.attackcooldown;
        player.GetComponent<playerMovement>().critrate = data.critrate;
        player.GetComponent<playerMovement>().critMultiplier = data.critMultiplier;
        player.GetComponent<playerMovement>().Coins = data.Coins;
        NewMap();
        MenuUI.gameObject.SetActive(false);
        player.GetComponent<playerMovement>().UpdateCurrency();
    }

    public void closeLoadMenu()
    {
        MenuUI.gameObject.SetActive(false);
        SaveSystem.SavePlayer(player.GetComponent<playerMovement>(), this);
    }
}
