using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDunGen : MonoBehaviour
{
    [SerializeField]
    protected TileMapVisualizer tileMapVisualizer = null;

    [SerializeField]
    protected Vector2Int startpos =  Vector2Int.zero;

    public void GenerateDungeon()
    {
        tileMapVisualizer.Clear();
        RunProcedualGeneration();
    }

    protected abstract void RunProcedualGeneration();
}
