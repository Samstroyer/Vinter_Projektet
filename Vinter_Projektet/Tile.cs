using System;
using Raylib_cs;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

public enum BuildingType{
    Building_Type_Sawmill,
}

class Tile
{
    public string type;
    public string buildingName;   // ?
    public float alpha;           // 32 bitar
    public double richness;       // 64 bitar

    public Tile(string alt, int light, double assignedRichness)
    {
        type = alt;
        alpha = light / 255f;
        richness = assignedRichness;
    }

    public void SetBuilding(string buildingType)
    {
        buildingName = buildingType;
    }
}
