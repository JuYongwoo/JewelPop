using System;
using System.Collections.Generic;


[Serializable]
public class Grid { public int x; public int y; public string type; public int color; }
// r=row(y), c=col(x)

[Serializable]
public class JSONVars
{
    public string layout;   // "odd-r" ±«¿Â
    public List<Grid> grids;
}
