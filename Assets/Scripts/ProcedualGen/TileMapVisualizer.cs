using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TileMapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floortilemap , walltilemap, portaltilemap , shoptilemap, bonestilemap, lavatilemap, mudtilemap;
        
    [SerializeField]
    private TileBase floortile , wallTop , portalTile, shopTile, bonesTile, bonesTile2, lavaTile , mudTile;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorpos)
    {
        PaintTiles(floorpos, floortilemap, floortile);
    }
    public void PaintBoneTiles(IEnumerable<Vector2Int> bonepos)
    {
        PaintMultipleTiles(bonepos, bonestilemap, bonesTile, bonesTile2);
    }

    public void PaintLavaTiles(IEnumerable<Vector2Int> lavapos)
    {
        PaintTiles(lavapos, lavatilemap, lavaTile);
    }

    public void PaintMudTiles(IEnumerable<Vector2Int> mudpos)
    {
        PaintTiles(mudpos, mudtilemap, mudTile);
    }

    public void PaintSinglePortalTile(Vector2Int pos)
    {
        PaintSingleTile(portaltilemap, portalTile, pos);
    }

    public void PaintSingleShopTile(Vector2Int pos)
    {
        PaintSingleTile(shoptilemap, shopTile, pos);
    }

    private void PaintTiles(IEnumerable<Vector2Int> pos, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in pos)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintMultipleTiles(IEnumerable<Vector2Int> pos, Tilemap tilemap, TileBase tile, TileBase tile2)
    {
        foreach (var position in pos)
        {
            int randomnum = Random.Range(0, 10);
            if(randomnum > 5)
            {
               PaintSingleTile(tilemap, tile, position);
            }
            else
            {
                PaintSingleTile(tilemap, tile2, position);
            }
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
        portaltilemap.ClearAllTiles();
        shoptilemap.ClearAllTiles();
        bonestilemap.ClearAllTiles();
        lavatilemap.ClearAllTiles();
        mudtilemap.ClearAllTiles();
    }
}
