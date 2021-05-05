using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomWalkParam", menuName = "ProcedualGen/RandomWalkData")]
public class RandomWalkData : ScriptableObject
{
    public int iterations = 20, walklength = 20;
    public bool startrandomlyEachIteration = true;
}
