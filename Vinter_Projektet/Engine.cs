using System;
using Raylib_cs;
using System.Timers;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;


// Game-klockan är taget från https://docs.microsoft.com/en-us/dotnet/api/system.timers.timer.interval?view=net-6.0 ganska direkt
// Jag måste ha en timer som är reliable så inte ett snabbare system får 2x så snabbt pengar jämfört med ett annat system.

class Game
{
    private Timer clock;
    private BigInteger moneyPerInterval = 0;
    Tile[,] map;
    Player p;
    TextureHandler th;
    bool grid = false;
    int[] pixelsPerTile = new int[10] { 1, 2, 4, 8, 16, 20, 32, 40, 50, 80 };

    public Game(Tile[,] m)
    {
        map = m;
        // Create a timer and set a two second interval.
        clock = new System.Timers.Timer();
        clock.Interval = 1000;

        // Hook up the Elapsed event for the timer. 
        clock.Elapsed += OnTimedEvent;

        // Have the timer fire repeated events (true is the default)
        clock.AutoReset = true;

        // Start the timer
        clock.Enabled = true;
    }

    public void Run()
    {
        p = new Player(false);
        th = new TextureHandler(pixelsPerTile);
        int indexer = 6;
        int currPixelSize = pixelsPerTile[indexer];


        //Only need a start position as the pixelsPerTile will tell me the stop position
        //The "size" arguments will be from the Tile[,] which is 1000x1000
        Vector2 cameraStart = new Vector2(450, 450);


        while (!Raylib.WindowShouldClose())
        {
            int visibleTiles = 0;

            //Movement on the map
            int modifier = currPixelSize == pixelsPerTile[0] || currPixelSize == pixelsPerTile[1] || currPixelSize == pixelsPerTile[2] ? 5 : 1;
            (cameraStart, indexer, visibleTiles, currPixelSize) = Camera(modifier, cameraStart, indexer, currPixelSize, visibleTiles);
            Keybinds(currPixelSize);

            Rendering(visibleTiles, cameraStart, currPixelSize);

        }
    }

    private void Rendering(int visibleTiles, Vector2 cameraStart, int currPixelSize)
    {
        Vector2 cameraStop;

        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.PURPLE);

        //CameraStop måste updateras innan också, annars så kommer man rendera tiles som inte existerar...
        cameraStop.X = cameraStart.X + visibleTiles;
        cameraStop.Y = cameraStart.Y + visibleTiles;
        for (int x = (int)cameraStart.X; x < cameraStop.X; x++)
        {
            for (var y = (int)cameraStart.Y; y < cameraStop.Y; y++)
            {
                RenderChunk((x - (int)cameraStart.X, y - (int)cameraStart.Y), map[x, y], currPixelSize, grid, th);
            }
        }



        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
        {
            Vector2 mouseCords = Raylib.GetMousePosition();

            //Det finns redan lagg problem ibland, speciellt på skoldatorn. Så för att motverka det:
            //Gå igenom "x" först (i), hitta vilket x och sen kolla den raden.
            //Annars skulle man brute-force'a sig igenom en större grid 
            for (var i = 0; i < Raylib.GetScreenWidth(); i += currPixelSize)
            {
                if (mouseCords.X > i && mouseCords.X < i + currPixelSize)
                {
                    int arrX = (int)cameraStart.X + i / currPixelSize;

                    for (var j = 0; j < Raylib.GetScreenHeight(); j += currPixelSize)
                    {
                        if (mouseCords.Y > j && mouseCords.Y < j + currPixelSize)
                        {
                            int arrY = (int)cameraStart.Y + j / currPixelSize;
                            map[arrX, arrY].buildingName = p.ChangeTileTypeToSelectedItem();
                            break;
                        }
                    }
                    break;
                }
            }
        }


        //Rendera selected item ovanför allt ->
        p.DisplaySelectedItem();


        Raylib.EndDrawing();

    }

    private void Keybinds(int currPixelSize)
    {
        //Toggle grid
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_G))
        {
            grid = !grid;
        }
        //För om... ja alltså om du visar varje block som en pixel och tar bort en pixel ser man inget...
        if (currPixelSize == 1)
        {
            grid = false;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.KEY_P))
        {
            //Lika med inte sig själv är typ en toggle - True => False && False => True
            p.readyToPlace = !p.readyToPlace;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.KEY_COMMA))
        {
            p.SwitchSelectedItem(1);
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.KEY_PERIOD))
        {
            p.SwitchSelectedItem(-1);
        }

    }

    private (Vector2, int, int, int) Camera(int modifier, Vector2 cameraStart, int indexer, int currPixelSize, int visibleTiles)
    {
        if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
        {
            cameraStart.X -= 1 * modifier;
        }
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
        {
            cameraStart.X += 1 * modifier;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
        {
            cameraStart.Y -= 1 * modifier;
        }
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
        {
            cameraStart.Y += 1 * modifier;
        }

        //Scroll on the map
        indexer += Convert.ToInt32(Raylib.GetMouseWheelMove());
        if (indexer < 0)
        {
            indexer = pixelsPerTile.Length - 1;
        }
        else if (indexer > pixelsPerTile.Length - 1)
        {
            indexer = 0;
        }
        currPixelSize = pixelsPerTile[indexer];
        visibleTiles = 800 / currPixelSize;

        //Om man zoomar in, går nära kartans kant och sen zoomar ut så gör man en glitch :)
        //Det ska inte gå så jag lägger in säkerhetskod här! (Den gör så att den inte kan gå ArrayOutOfBound)
        if (cameraStart.X + visibleTiles >= 1000)
        {
            cameraStart.X = 1000 - visibleTiles;
        }
        else if (cameraStart.X < 0)
        {
            cameraStart.X = 0;
        }
        if (cameraStart.Y + visibleTiles >= 1000)
        {
            cameraStart.Y = 1000 - visibleTiles;
        }
        else if (cameraStart.Y < 0)
        {
            cameraStart.Y = 0;
        }

        //Det här var i tidigare versioner av spelet i "main" loopen, det var lite cluttered - så la det här istället.
        return (cameraStart, indexer, visibleTiles, currPixelSize);
    }

    static void RenderChunk((int x, int y) arrPos, Tile t, int size, bool grid, TextureHandler th)
    {
        int g = grid ? 1 : 0;
        int sizedX = arrPos.x * size;
        int sizedY = arrPos.y * size;
        int displaySize = size - g;
        switch (t.type)
        {
            case "water":
                Raylib.DrawRectangle(sizedX, sizedY, displaySize, displaySize, Color.BLUE);
                break;
            case "flat":
                Raylib.DrawRectangle(sizedX, sizedY, displaySize, displaySize, Color.ORANGE);
                break;
            case "forrest":
                Raylib.DrawRectangle(sizedX, sizedY, displaySize, displaySize, Color.GREEN);
                break;
            case "stone":
                Raylib.DrawRectangle(sizedX, sizedY, displaySize, displaySize, Color.GRAY);
                break;
            case "debug":
                Raylib.DrawRectangle(sizedX, sizedY, displaySize, displaySize, Color.RED);
                break;
        }

        if (t.buildingName != "" && t.buildingName != null && displaySize > 1)
        {
            th.RenderBuilding(sizedX, sizedY, size, t.buildingName);

            // HUR MAN SKA GÖRA DET I RÄTT ORDNING SEPARAT istället för th.RenderBuilding()
            // Image tempImg = Raylib.LoadImageFromTexture(t.building.texture);
            // Raylib.ImageResize(ref tempImg, displaySize, displaySize);
            // Texture2D dispTexture = Raylib.LoadTextureFromImage(tempImg);
            // Raylib.DrawTexture(dispTexture, sizedX, sizedY, Color.WHITE);
            // Raylib.UnloadImage(tempImg);
            // Raylib.UnloadTexture(dispTexture);
        }
    }

    public void ChangeMoneyPerInterval(int changer)
    {
        moneyPerInterval += changer;
    }

    private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
    {
        Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
    }
}