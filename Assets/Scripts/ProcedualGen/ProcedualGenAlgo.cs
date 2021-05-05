using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ProcedualGenAlgo
{
    public static HashSet<Vector2Int> RandomWalk(Vector2Int startingpos, int walklength)  // Random Walk Algorithm
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startingpos);
        var prevpos = startingpos;

        for (int i = 0; i < walklength; i++)
        {
            var newpos = prevpos + Direction2D.GetRandCardinalDir();  //Add new rand dir
            path.Add(newpos);                                         //add to path
            prevpos = newpos;                                         //set prev to new pos
        }
        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startingpos, int corridorlength)  // Random Walk In one dir Algorithm
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var dir = Direction2D.GetRandCardinalDir();                //Get rand dir
        var currpos = startingpos;
        corridor.Add(currpos);                                     //add starting

        for (int i = 0; i < corridorlength; i++)                   //add tiles in corridor dir
        {
            currpos += dir;
            corridor.Add(currpos);
        }
        return corridor;
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spacetoSplit, int minWidth, int minHeight)  // BSP Algorithm
    {
        Queue<BoundsInt> roomsqueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();

        roomsqueue.Enqueue(spacetoSplit);
        while(roomsqueue.Count > 0) //while we have rooms to split
        {
            var room = roomsqueue.Dequeue();
            if(room.size.y >= minHeight && room.size.x >= minWidth)  //can still be split or contain a room
            {
                if(Random.value < 0.5f)
                {
                    if(room.size.y >= minHeight * 2)   //if can be split horizontally
                    {
                        SplitHorizontally(minHeight, roomsqueue, room);
                    }
                    else if(room.size.x >= minWidth * 2)  //if can be split vertically
                    {
                        SplitVertically(minWidth, roomsqueue, room);
                    }
                    else if(room.size.x >= minWidth && room.size.y >= minHeight) //if area cant be split but can contain room
                    {
                        roomsList.Add(room);
                    }
                }
                else
                {
                    if (room.size.x >= minWidth * 2)  //if can be split vertically
                    {
                        SplitVertically(minWidth, roomsqueue, room);
                    }
                    else if (room.size.y >= minHeight * 2)   //if can be split horizontally
                    {
                        SplitHorizontally( minHeight, roomsqueue, room);
                    }
                    else if (room.size.x >= minWidth && room.size.y >= minHeight) //if area cant be split but can contain room
                    {
                        roomsList.Add(room);
                    }
                }
            }
        }
        return roomsList;
    }

    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsqueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);  //get random range between 1 and max -1
        BoundsInt firstroom = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));  //Position, Size
        BoundsInt secondroom = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z), new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));

        roomsqueue.Enqueue(firstroom); //requeue rooms
        roomsqueue.Enqueue(secondroom);
    }

    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsqueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);  //get random range between 1 and max -1
        BoundsInt firstroom = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));  //Position, Size
        BoundsInt secondroom = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z), new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));

        roomsqueue.Enqueue(firstroom); //requeue rooms
        roomsqueue.Enqueue(secondroom);
    }
}

public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0,1), //UP
        new Vector2Int(0,-1), //DOWN 
        new Vector2Int(-1,0), //LEFT
        new Vector2Int(1,0) //RIGHT
    };

    public static Vector2Int GetRandCardinalDir()
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}
