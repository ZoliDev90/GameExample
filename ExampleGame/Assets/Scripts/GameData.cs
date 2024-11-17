using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public float TimeElapsed;
    public int Score;
    public int Turns;
    public int Hits;
    public int Rows;
    public int Columns;
    public List<Vector3> CardPositions;
}
