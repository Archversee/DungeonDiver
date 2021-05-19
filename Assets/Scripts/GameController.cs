using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameController : MonoBehaviour
{
    public RoomFirstDunGen roomFirstDunGen;
    [SerializeField]
    protected TileMapVisualizer tileMapVisualizer = null;
    EnemyManager m_EnemyManager = EnemyManager.Singleton;

    private NavMeshSurface2d navMeshSurfaces;

    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        navMeshSurfaces = FindObjectOfType<NavMeshSurface2d>();
        navMeshSurfaces.BuildNavMesh();
        //InvokeRepeating("doubleUpdateNavMesh", 0f, 1.0f);

        tileMapVisualizer.Clear();
        roomFirstDunGen.CreateRooms();

        Vector3 temppos = player.position;
        temppos.x = roomFirstDunGen.StartingPos.x +1;
        temppos.y = roomFirstDunGen.StartingPos.y + 1;
        player.position = temppos;
        doubleUpdateNavMesh();
    }
    void doubleUpdateNavMesh()
    {
        navMeshSurfaces.UpdateNavMesh(navMeshSurfaces.navMeshData);
        navMeshSurfaces.UpdateNavMesh(navMeshSurfaces.navMeshData);
    }

    public void NewMap()
    {
        tileMapVisualizer.Clear();
        m_EnemyManager.ClearList();
        roomFirstDunGen.CreateRooms();
        doubleUpdateNavMesh();

        Vector3 temppos = player.position;
        temppos.x = roomFirstDunGen.StartingPos.x + 1;
        temppos.y = roomFirstDunGen.StartingPos.y + 1;
        player.position = temppos;
    }
}
