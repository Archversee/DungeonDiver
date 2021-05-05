using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floortilemap , walltilemap;
        
    [SerializeField]
    private TileBase floortile , wallTop;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorpos)
    {
        PaintTiles(floorpos, floortilemap, floortile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> pos, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in pos)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int pos)
    {
        var tilepos = tilemap.WorldToCell((Vector3Int)pos);   //Set pos
        tilemap.SetTile(tilepos, tile);     //set tile
    }

    internal void PaintSingleBasicWall(Vector2Int pos)
    {
        PaintSingleTile(walltilemap, wallTop, pos); 
    }

    public void Clear() //Clear all tiles
    {
        floortilemap.ClearAllTiles();
        walltilemap.ClearAllTiles();
    }
}
