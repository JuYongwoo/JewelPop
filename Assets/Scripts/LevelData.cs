using System;
using System.Collections.Generic;

[Serializable]
public class LevelGoal { public string type; public int color; public int count; }

[Serializable]
public class CellDef { public int r; public int c; public string type; public int color; }
// r=row(y), c=col(x)

[Serializable]
public class LevelDef
{
    public int id;
    public string layout;   // "odd-r" ±«¿Â
    public int width;
    public int height;
    public int moves;
    public List<LevelGoal> goals;
    public List<CellDef> cells;
}
