using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public RoomFirstDunGen roomFirstDunGen;
    [SerializeField]
    protected TileMapVisualizer tileMapVisualizer = null;
    EnemyManager m_EnemyManager = EnemyManager.Singleton;
    ConsumableManager m_consumableManager = ConsumableManager.Singleton;

    GameObject[] Consumables;

private NavMeshSurface2d navMeshSurfaces;

    public Transform player;

    public float Score;
    public Text ScoreText;

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
    }
    void doubleUpdateNavMesh()
    {
        navMeshSurfaces.UpdateNavMesh(navMeshSurfaces.navMeshData);
        navMeshSurfaces.UpdateNavMesh(navMeshSurfaces.navMeshData);
    }

    public void FixedUpdate()
    {
        ScoreText.text = "Score: " + Score.ToString();
    }

    public void NewMap()
    {
        tileMapVisualizer.Clear();
        m_EnemyManager.ClearList();
        Consumables = GameObject.FindGameObjectsWithTag("Consumable");
        foreach (GameObject consum in Consumables)
        {
            Destroy(consum);
        }

        roomFirstDunGen.CreateRooms();
        doubleUpdateNavMesh();
        Score += 5f;


        Vector3 temppos = player.position;
        temppos.x = roomFirstDunGen.StartingPos.x + 1;
        temppos.y = roomFirstDunGen.StartingPos.y + 1;
        player.position = temppos;
    }
}
