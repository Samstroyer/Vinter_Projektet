using System;
using Raylib_cs;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

class MapManager
{
    public MapManager(bool loadFromSave)
    {
        if (loadFromSave)
        {

        }
        else
        {

        }
    }

    public Tile[,] GenerateMap()
    {
        Random ran = new Random();
        int pregenAlternatives = Directory.GetFiles(@"SaveData\Pregens").Count();
        string selectedImage = ran.Next(pregenAlternatives).ToString() + ".png";
        Tile[,] mapTiles = new Tile[1000, 1000];

        Image noiseImage = Raylib.LoadImage(@$"SaveData\Pregens\{selectedImage}");

        //Generatorn : bild till karta : ALLA RGB ÄR SAMMA! (Gray scale 0-255)
        for (int x = 0; x < noiseImage.width; x++)
        {
            for (int y = 0; y < noiseImage.height; y++)
            {
                //Vi får pixelns ljusstyrka och av det ska vi generera "terrain"
                Color colors = Raylib.GetImageColor(noiseImage, x, y);
                int brightness = (colors.r + colors.g + colors.b) / 3;

                //Den här delen skulle vara i en separat funktion - men tror jag gillar det mer grupperat i ett.
                //4 for loops i varandra också, not too bad.... (it is really bad)
                for (int xx = 0; xx < 4; xx++)
                {
                    for (int yy = 0; yy < 4; yy++)
                    {
                        int usedX = xx + (x * 4);
                        int usedY = yy + (y * 4);

                        mapTiles[xx + x * 4, yy + y * 4] = GenerateTile(brightness);
                    }
                }
            }
        }

        Raylib.UnloadImage(noiseImage);

        return mapTiles;
    }


    //Index:
    //wT = water tile : <65
    //fT = flat tile : 66 - 135
    //fT = forrest tile : 136-200
    //sT = stone tile : >200
    private Tile GenerateTile(int light)
    {
        string alt;

        if (light < 65)
        {
            alt = "water";
        }
        else if (light < 135)
        {
            alt = "flat";
        }
        else if (light < 200)
        {
            alt = "forrest";
        }
        else
        {
            alt = "stone";
        }

        return new Tile(alt, light);
    }
}