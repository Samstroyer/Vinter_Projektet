using System;
using Raylib_cs;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

class Tile
{
    public string type;
    public float alpha;
    public string buildingName;

    public Tile(string alt, int light)
    {
        type = alt;
        alpha = light / 255f;
    }

    public void SetBuilding(string buildingType)
    {
        buildingName = buildingType;
    }
}