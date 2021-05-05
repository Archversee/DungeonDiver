using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDunGen : RandomWalkGen
{
    [SerializeField]
    private int corridorLength = 14, corridorCount = 5;
    [SerializeField]
    private float roomPercent = 0.8f;

    protected override void RunProcedualGeneration()
    {
        CorridorFirstGen();
    }

    private void CorridorFirstGen()
    {
        HashSet<Vector2Int> floorpos = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPos = new HashSet<Vector2Int>();

        CreateCorridors(floorpos, potentialRoomPos);   //Create corridors first

        HashSet<Vector2Int> roomPos = CreateRooms(potentialRoomPos);  //Create rooms
        List<Vector2Int> deadEnds = FindAllDeadEnds(floorpos); //find deadends

        CreateRoomsAtDeadEnd(deadEnds, roomPos); //create deadEnd rooms

        floorpos.UnionWith(roomPos);  //add roompos to floorpos

        //tileMapVisualizer.Clear();  //Clear prev 
        tileMapVisualizer.PaintFloorTiles(floorpos);
        WallGenerator.CreateWalls(floorpos, tileMapVisualizer);
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomPos)
    {
        foreach (var pos in deadEnds)
        {
            if(roomPos.Contains(pos) == false) //if pos is not a roompos already
            {
                var room = RunRandomWalk(randomwalkParam, pos);
                roomPos.UnionWith(room);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorpos)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        foreach (var pos in floorpos)
        {
            int neighboursCount = 0;
            foreach (var dir in Direction2D.cardinalDirectionsList)  //check all dir
            {
                if (floorpos.Contains(pos + dir))
                {
                    neighboursCount++;
                }
            }
            if (neighboursCount == 1)  //if only has 1 neighbour
            {
                deadEnds.Add(pos);
            }
        }
        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPos)
    {
        HashSet<Vector2Int> roomPos = new HashSet<Vector2Int>();
        int roomtoCreateCount = Mathf.RoundToInt(potentialRoomPos.Count * roomPercent); //Get count of rooms using percent

        List<Vector2Int> roomstoCreate = potentialRoomPos.OrderBy(x => Guid.NewGuid()).Take(roomtoCreateCount).ToList();  //sort by random order,then take the number we need

        foreach (var Pos in roomstoCreate)
        {
            var roomFloor = RunRandomWalk(randomwalkParam, Pos);  //Create rooms 
            roomPos.UnionWith(roomFloor);
        }

        return roomPos;
    }

    private void CreateCorridors(HashSet<Vector2Int> floorpos, HashSet<Vector2Int> potentialRoomPos)
    {
        var currpos = startpos;
        potentialRoomPos.Add(currpos);
        for (int i = 0; i < corridorCount; i++)  // loop through count
        {
            var corridor = ProcedualGenAlgo.RandomWalkCorridor(currpos, corridorLength);
            currpos = corridor[corridor.Count - 1];    //set last corr pos as starting point to connect
            potentialRoomPos.Add(currpos);             //set last area of corridor to be potential room
            floorpos.UnionWith(corridor);
        }
    }
}
