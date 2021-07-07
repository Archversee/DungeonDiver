﻿using System;
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
    [SerializeField]
    private GameObject RangedEnemy;
    //Consumables
    public GameObject HealthPot;
    public GameObject TreasureChest;

    public GameObject gameController;

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
        CreateAesthetics(floor);
        //print(StartingPos);

        tileMapVisualizer.PaintFloorTiles(floor);             //paint floor
        WallGenerator.CreateWalls(floor, tileMapVisualizer);  //paint wall
        CreateEndingPos(floor);  //paint portal
    }

    private void SpawnEnemy(HashSet<Vector2Int> floor)
    {
        int tilenum = 50 - gameController.GetComponent<GameController>().levelCount;
        float offset = 0.5f;
        if (tilenum <= 30)
        {
            tilenum = 30; //clamp the least num of tiles to be 30
        }    

        int EnemyCounterLimit = floor.Count / tilenum; //for each 50 tiles we allow 1 enemy to spawn / as you go deeper into the level the amt of tiles needed decreases

        List<Vector2Int> enemiestoSpawn = floor.OrderBy(x => Guid.NewGuid()).Take(EnemyCounterLimit).ToList();  //sort by random order,then take the number we need

        foreach (var Pos in enemiestoSpawn)
        {
            int enemytospawn = Random.Range(0, 3);
            if (enemytospawn == 0) //mix both
            {
                float randnum = Random.Range(0, 1);
                if (randnum <= 0.5f)
                {
                    Instantiate(BasicEnemy, new Vector3(Pos.x + offset, Pos.y + offset, 0), Quaternion.identity);
                }
                else 
                {
                    Instantiate(RangedEnemy, new Vector3(Pos.x + offset, Pos.y + offset, 0), Quaternion.identity);
                }
            }
            else if (enemytospawn == 1) //only melee
            {
                Instantiate(BasicEnemy, new Vector3(Pos.x + offset, Pos.y + offset, 0), Quaternion.identity);
            }
            else if(enemytospawn == 2) // onyl ranged
            {
                Instantiate(RangedEnemy, new Vector3(Pos.x + offset, Pos.y + offset, 0), Quaternion.identity);
            }
        }
    }

    private void SpawnConsumables(HashSet<Vector2Int> floor)
    {
        int HealthCounterLimit = floor.Count / 140; //for each 140 tiles we allow 1 enemy to spawn
        int ChestCounterLimit = floor.Count / 180; //for each 180 tiles we allow 1 enemy to spawn

        List<Vector2Int> HealthPotstoSpawn = floor.OrderBy(x => Guid.NewGuid()).Take(HealthCounterLimit).ToList();  //sort by random order,then take the number we need
        List<Vector2Int> ChesttoSpawn = floor.OrderBy(x => Guid.NewGuid()).Take(ChestCounterLimit).ToList();  //sort by random order,then take the number we need
        float offset = 0.5f;

        foreach (var Pos in HealthPotstoSpawn)
        {
            Instantiate(HealthPot, new Vector3(Pos.x + offset, Pos.y + offset, 0), Quaternion.identity);
        }
        foreach (var Pos in ChesttoSpawn)
        {
            float randnum = Random.Range(0, 1);
            if (randnum < 0.3)
            {
                Instantiate(TreasureChest, new Vector3(Pos.x + offset, Pos.y + offset, 0), Quaternion.identity);
            }
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
    private void CreateAesthetics(HashSet<Vector2Int> floor)
    {
        HashSet<Vector2Int> bones = new HashSet<Vector2Int>();
        int NumberB4nextBone = 0;

        foreach (var floorpos in floor)  
        {
            NumberB4nextBone++;
            if (NumberB4nextBone > 50)
            {
                int randnum = Random.Range(0, 10);
                if(randnum > 5)
                {
                    bones.Add(floorpos);
                    NumberB4nextBone = 0;
                }
            }
        }
        tileMapVisualizer.PaintBoneTiles(bones);
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
        float randnum = Random.Range(0f, 10f);
        bool widecorridors = false;
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var pos = currRoomCenter;
        corridor.Add(pos);

        if(randnum >= 5)
        {
            widecorridors = true;
        }

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
            if (widecorridors)
            {
                corridor.Add(pos + Vector2Int.right);
                corridor.Add(pos + Vector2Int.left);
            }
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
            if (widecorridors)
            {
                corridor.Add(pos + Vector2Int.up);
                corridor.Add(pos + Vector2Int.down);
            }
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

        int shoproomint = Random.Range(0, roomList.Count);
        //Debug.Log(shoproomint);

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

            if(room == roomList[shoproomint]) //if room designated to be shop
            {
                tileMapVisualizer.PaintSingleShopTile((Vector2Int)Vector3Int.RoundToInt(room.center));
            }
            else
            {
                int randnum = Random.Range(0, 100);
                if (randnum >= 70) //30% chance for special room
                {
                    if (randnum >= 70 && randnum < 80) // 10% for treasure room (no enemies only chest)
                    {
                        SpawnTreasureRoom(room);
                    }
                    else if(randnum >= 80 && randnum < 90) // 10% for lava room (lava burns player)
                    {
                        SpawnLavaRoom(tempfloor);
                    }
                    else if (randnum >= 90) // 10% for Mud room (Mud slows player)
                    {
                        SpawnMudRoom(tempfloor);
                        SpawnEnemy(tempfloor);
                    }
                    SpawnConsumables(tempfloor);
                }
                else //normal room
                {
                    //navMeshSurfaces.UpdateNavMesh(navMeshSurfaces.navMeshData);
                    SpawnEnemy(tempfloor);
                    SpawnConsumables(tempfloor);
                }
            }
        }
        return floor;
    }

    private void SpawnMudRoom(HashSet<Vector2Int> floor)
    {
        int MudCounterLimit = floor.Count / 2; //for each 5 tiles we allow 1 lava to spawn

        List<Vector2Int> MudtoSpawn = floor.OrderBy(x => Guid.NewGuid()).Take(MudCounterLimit).ToList();  //sort by random order,then take the number we need

        HashSet<Vector2Int> mud = new HashSet<Vector2Int>();
        foreach (var Pos in MudtoSpawn)
        {
            mud.Add(Pos);
        }
        tileMapVisualizer.PaintMudTiles(mud);
    }

    private void SpawnLavaRoom(HashSet<Vector2Int> floor)
    {
        int LavaCounterLimit = floor.Count / 5; //for each 5 tiles we allow 1 lava to spawn

        List<Vector2Int> LavatoSpawn = floor.OrderBy(x => Guid.NewGuid()).Take(LavaCounterLimit).ToList();  //sort by random order,then take the number we need
        
        HashSet<Vector2Int> lava = new HashSet<Vector2Int>();
        foreach (var Pos in LavatoSpawn)
        {
            lava.Add(Pos);
        }
        tileMapVisualizer.PaintLavaTiles(lava);
    }

    private void SpawnTreasureRoom(BoundsInt room)
    {

        List<Vector2Int> ChesttoSpawn = new List<Vector2Int>();

        ChesttoSpawn.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        ChesttoSpawn.Add((Vector2Int)Vector3Int.RoundToInt(new Vector3(room.center.x + 1, room.center.y, 0)));
        ChesttoSpawn.Add((Vector2Int)Vector3Int.RoundToInt(new Vector3(room.center.x - 1, room.center.y, 0)));

        float offset = 0.5f;

        foreach (var Pos in ChesttoSpawn)
        {
            float randnum = Random.Range(0, 1);
            if (randnum < 0.3)
            {
                Instantiate(TreasureChest, new Vector3(Pos.x + offset, Pos.y + offset, 0), Quaternion.identity);
                //m_consumableManager.RegisterConsumable(treasurec);
            }
        }
    }
}
