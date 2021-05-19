using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AI;

public class RoomFirstDunGen : RandomWalkGen
{
    [SerializeField]
    private int minRoomWidth = 4;
    [SerializeField]
    private int minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;

    [SerializeField]
    [Range(0, 10)]
    private int offset = 1; //offset so the rooms dont connect

    [SerializeField]
    private bool randomwalkRooms = false;

    //Enemies
    [SerializeField]
    private GameObject BasicEnemy;

   // public NavMeshSurface2d navMeshSurfaces;


    public Vector2Int StartingPos = Vector2Int.zero; 

    protected override void RunProcedualGeneration()
    {
        CreateRooms();
    }   

    public void CreateRooms()
    {
        var roomList = ProcedualGenAlgo.BinarySpacePartitioning(new BoundsInt((Vector3Int)startpos, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        if (randomwalkRooms)
        {
            floor = CreateRoomsUsingRandomWalk(roomList);        // Create Rooms using RandomWalk
        }
        else
        {
            floor = CreateSimpleRooms(roomList);                  //create rooms using BSP
        }

        List<Vector2Int> roomcenters = new List<Vector2Int>();
        foreach (var room in roomList)
        {
            roomcenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomcenters);  //connect rooms using corridors
        floor.UnionWith(corridors);

        StartingPos = FindStartingPos(floor);
        //print(StartingPos);

        tileMapVisualizer.PaintFloorTiles(floor);             //paint floor
        WallGenerator.CreateWalls(floor, tileMapVisualizer);  //paint wall
        CreateEndingPos(floor);  //paint portal
    }

    private void SpawnEnemy(HashSet<Vector2Int> floor)
    {
        int EnemyCounterLimit = floor.Count / 50; //for each 25 tiles we allow 1 enemy to spawn

        List<Vector2Int> enemiestoSpawn = floor.OrderBy(x => Guid.NewGuid()).Take(EnemyCounterLimit).ToList();  //sort by random order,then take the number we need

        foreach (var Pos in enemiestoSpawn)
        {
            Instantiate(BasicEnemy, new Vector3(Pos.x, Pos.y, 0), Quaternion.identity);
        }
    }

    private Vector2Int FindStartingPos(HashSet<Vector2Int> floor)
    {
        Vector2Int closest = Vector2Int.zero;

        int i = Random.Range(0, 10);
        foreach (var floorpos in floor)  //loop to find closest
        {
            if (i < 5)
            {
                float dist = float.MaxValue;
                float currdist = Vector2.Distance(floorpos, Vector2Int.zero);
                if (currdist < dist)
                {
                    dist = currdist;
                    closest = floorpos;
                }
            }
            else
            {
                float dist = float.MinValue;
                float currdist = Vector2.Distance(floorpos, Vector2Int.zero);
                if (currdist > dist)
                {
                    dist = currdist;
                    closest = floorpos;
                }
            }
        }
        return closest;
    }

    private void CreateEndingPos(HashSet<Vector2Int> floor)
    {
        Vector2Int furthest = Vector2Int.zero;
        float dist = float.MinValue;

        foreach (var floorpos in floor)  //loop to find closest
        {
            float currdist = Vector2.Distance(floorpos, StartingPos);
            if (currdist > dist)
            {
                dist = currdist;
                furthest = floorpos;
            }
        }
        tileMapVisualizer.PaintSinglePortalTile(furthest);
    }

    private HashSet<Vector2Int> CreateRoomsUsingRandomWalk(List<BoundsInt> roomList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        HashSet<Vector2Int> tempfloor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomList.Count; i++)
        {
            var roomBounds = roomList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));   //get center
            var roomFloor = RunRandomWalk(randomwalkParam, roomCenter);   //create using randomwalk

            foreach (var pos in roomFloor)
            {
                tempfloor.Clear();
                if(pos.x >= (roomBounds.xMin + offset) && pos.x <= (roomBounds.xMax - offset) && pos.y >= (roomBounds.yMin - offset) && pos.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(pos);  //if the floor is within the room bounds size then add it
                    tempfloor.Add(pos);
                }

                //navMeshSurfaces.UpdateNavMesh(navMeshSurfaces.navMeshData);
                SpawnEnemy(tempfloor);
            }

        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomcenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currRoomCenter = roomcenters[Random.Range(0, roomcenters.Count)];   //get random curr room center
        roomcenters.Remove(currRoomCenter);                                     // remove from list

        while (roomcenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currRoomCenter, roomcenters);// find closest room center to curr
            roomcenters.Remove(closest);                                         // remove closest
            HashSet<Vector2Int> newCorridor = CreateCorridor(currRoomCenter, closest); // create corridor
            currRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currRoomCenter, Vector2Int dest)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var pos = currRoomCenter;
        corridor.Add(pos);

        while (pos.y != dest.y)  //while not at same y coord
        {
            if(dest.y > pos.y)
            {
                pos += Vector2Int.up;   //go UP
            }
            else if(dest.y < pos.y)
            {
                pos += Vector2Int.down;//go DOWN
            }
            corridor.Add(pos);
        }
        while (pos.x != dest.x)//while not at same x coord
        {
            if (dest.x > pos.x)
            {
                pos += Vector2Int.right;//go RIGHT
            }
            else if (dest.x < pos.x)
            {
                pos += Vector2Int.left; //go LEFT
            }
            corridor.Add(pos);
        }

        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currRoomCenter, List<Vector2Int> roomcenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float dist = float.MaxValue;

        foreach (var roompos in roomcenters)  //loop to find closest
        {
            float currdist = Vector2.Distance(roompos, currRoomCenter);
            if(currdist < dist)
            {
                dist = currdist;
                closest = roompos;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        HashSet<Vector2Int> tempfloor = new HashSet<Vector2Int>();

        foreach (var room in roomList)   //for each room
        {
            tempfloor.Clear();
            for (int col = offset; col < room.size.x - offset; col++) //offset so rooms dont connect
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int pos = (Vector2Int)room.min + new Vector2Int(col, row); //add floorpos into floor hashset
                    floor.Add(pos);
                    tempfloor.Add(pos);
                }
            }

            //navMeshSurfaces.UpdateNavMesh(navMeshSurfaces.navMeshData);
            SpawnEnemy(tempfloor);
        }
        return floor;
    }
}
