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
    public double richness;

    public Tile(string alt, int light, double richness)
    {
        type = alt;
        alpha = light / 255f;
    }

    public void SetBuilding(string buildingType)
    {
        buildingName = buildingType;
    }
}