using System;
using System.Collections.Generic;


[Serializable]
public class Grid { public int x; public int y; public string type; }

[Serializable]
public class JSONVars
{
    public int stage;
    public string goalType;
    public int goalScore;
    public List<Grid> grids;
}
