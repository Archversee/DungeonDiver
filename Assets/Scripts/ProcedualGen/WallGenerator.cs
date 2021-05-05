using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorpos, TileMapVisualizer tileMapVisualizer)
    {
        var basicWallPositions = FindWallsinDir(floorpos, Direction2D.cardinalDirectionsList);
        foreach (var position in basicWallPositions)
        { tileMapVisualizer.PaintSingleBasicWall(position);
           
        }
    }

    private static HashSet<Vector2Int> FindWallsinDir(HashSet<Vector2Int> floorpos, List<Vector2Int> dirList)
    {
        HashSet<Vector2Int> wallpos = new HashSet<Vector2Int>();
        foreach (var positions in floorpos)  //for each floor tile
        {
            foreach (var dir in dirList)  // get each dir
            {
                var neighbourPos = positions + dir;      //get neighbour
                if(floorpos.Contains(neighbourPos) == false)  //if neighbour ! a floor add to wall hashset
                {
                    wallpos.Add(neighbourPos);
                }
            }
        }
        return wallpos;
    }
}
