using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomWalkGen : AbstractDunGen
{
    [SerializeField]
    protected RandomWalkData randomwalkParam;

    protected override void RunProcedualGeneration()    
    {
        HashSet<Vector2Int> floorpos = RunRandomWalk(randomwalkParam, startpos);
        tileMapVisualizer.Clear();  //Clear prev 
        tileMapVisualizer.PaintFloorTiles(floorpos); //Paint tiles
        WallGenerator.CreateWalls(floorpos, tileMapVisualizer); //Paint Walls
    }
        
    protected HashSet<Vector2Int> RunRandomWalk(RandomWalkData param, Vector2Int pos)
    {
        var currpos = pos;
        HashSet<Vector2Int> floorpos = new HashSet<Vector2Int>();
        for (int i = 0; i < param.iterations; i++)  // loop through iterations
        {
            var path = ProcedualGenAlgo.RandomWalk(currpos, param.walklength);
            floorpos.UnionWith(path);
            if (param.startrandomlyEachIteration)
                currpos = floorpos.ElementAt(Random.Range(0, floorpos.Count));
        }
        return floorpos;
    }
}
