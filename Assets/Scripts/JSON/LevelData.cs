using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Grid { public int x; public int y; public string type; }

[Serializable]
public class JSONVars
{
    [SerializeField]
    private int stage;
    [SerializeField]
    private string goalType;
    [SerializeField]
    private int goalScore;
    [SerializeField]
    private List<Grid> grids;

    public int Stage => stage;
    public string GoalType => goalType;
    public int GoalScore => goalScore;
    public List<Grid> Grids => grids;
}
