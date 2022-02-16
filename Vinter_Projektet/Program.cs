using System;
using Raylib_cs;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace VinterProjektet
{
    class Program
    {
        static void Main(string[] args)
        {
            Setup();
            GameLoop();
        }

        static void Setup()
        {
            Raylib.InitWindow(800, 800, "The project!");
            Raylib.SetTargetFPS(60);
        }

        static void GameLoop()
        {
            string menu = "start";
            while (!Raylib.WindowShouldClose())
            {
                switch (menu)
                {
                    case "start":
                        menu = DisplayStartScreen();
                        break;
                    case "creator":
                        CreateMap();
                        break;

                        //default:
                        //menu = DisplayStartScreen();
                        //break;
                }
            }
        }

        static void CreateMap()
        {
            //Index:
            //wT = water tile : <65
            //fT = flat tile : 66 - 135
            //fT = forrest tile : 136-200
            //sT = stone tile : >200

            string selectedImage = "";
            Random ran = new Random();

            if (Directory.Exists(@"SaveData\Pregens"))
            {
                int pregenAlternatives = Directory.GetFiles(@"SaveData\Pregens").Count();
                selectedImage = ran.Next(pregenAlternatives).ToString() + ".png";
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

                StartGame(mapTiles);

                // Visar bilden som används på skärmen (DEBUGGING MEST)
                // Raylib.BeginDrawing();
                // Raylib.ClearBackground(Color.BLACK);
                // for (int i = 0; i < mapTiles.GetLength(0); i++)
                // {
                //     for (int j = 0; j < mapTiles.GetLength(1); j++)
                //     {
                //         Raylib.DrawRectangle(Convert.ToInt32(i * 0.8), Convert.ToInt32(j * 0.8), 1, 1, Raylib.ColorAlpha(Color.WHITE, mapTiles[i, j].alpha));
                //     }
                // }
                // Raylib.EndDrawing();
            }
            else
            {
                Console.WriteLine("Error loading, no saves directory!");
                Raylib.CloseWindow();
            }
        }

        static void StartGame(Tile[,] map)
        {
            Player p = new Player();
            List<(Texture2D texture, string name)> textures = p.AvailableTileTextures();
            int[] pixelsPerTile = new int[10] { 1, 2, 4, 8, 16, 20, 32, 40, 50, 80 };
            int indexer = 6;
            int currPixelSize = pixelsPerTile[indexer];
            bool grid = false;

            //Only need a start position as the pixelsPerTile will tell me the stop position
            //The "size" arguments will be from the Tile[,] which is 1000x1000
            Vector2 cameraStart = new Vector2(450, 450);

            bool playing = true;

            while (playing && !Raylib.WindowShouldClose())
            {
                int visibleTiles;
                Vector2 cameraStop;

                //Movement on the map
                int modifier = currPixelSize == pixelsPerTile[0] || currPixelSize == pixelsPerTile[1] || currPixelSize == pixelsPerTile[2] ? 5 : 1;
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

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.PURPLE);

                //CameraStop måste updateras innan också, annars så kommer man rendera tiles som inte existerar...
                cameraStop.X = cameraStart.X + visibleTiles;
                cameraStop.Y = cameraStart.Y + visibleTiles;
                for (int x = (int)cameraStart.X; x < cameraStop.X; x++)
                {
                    for (var y = (int)cameraStart.Y; y < cameraStop.Y; y++)
                    {
                        RenderChunk((x - (int)cameraStart.X, y - (int)cameraStart.Y), map[x, y], currPixelSize, grid);
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
                                    map[arrX, arrY].building = p.ChangeTileTypeToSelectedItem();
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
        }

        static async void RenderChunk((int x, int y) cords, Tile t, int size, bool grid)
        {
            int g = grid ? 1 : 0;
            int sizedX = cords.x * size;
            int sizedY = cords.y * size;
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

            if (t.building.name != "" && t.building.name != null && displaySize > 1)
            {
                // HUR MAN SKA GÖRA DET I RÄTT ORDNING SEPARAT
                Image tempImg = Raylib.LoadImageFromTexture(t.building.texture);
                Raylib.ImageResize(ref tempImg, displaySize, displaySize);
                Texture2D dispTexture = Raylib.LoadTextureFromImage(tempImg);
                Raylib.DrawTexture(dispTexture, sizedX, sizedY, Color.WHITE);

                // Raylib.UnloadImage(tempImg);
                // Raylib.UnloadTexture(dispTexture);
            }
        }

        static Tile GenerateTile(int light)
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


        static string DisplayStartScreen()
        {
            int recWidth = 400;
            int recHeight = 100;
            (Rectangle, string, string)[] buttons = new (Rectangle, string, string)[3] { (new Rectangle(), "Load", "play"), (new Rectangle(), "New", "creator"), (new Rectangle(), "Settings", "settings") };
            string nextMenu = "";

            int iOffset = 150;
            int startY = 300;
            for (int i = 0; i < buttons.Length; i++)
            {
                int x = Raylib.GetScreenWidth() / 2 - recWidth / 2;
                int y = i * iOffset + startY;
                buttons[i].Item1 = new Rectangle(x, y, recWidth, recHeight);
            }

            while (nextMenu == "" && !Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);

                for (int i = 0; i < buttons.Length; i++)
                {
                    Rectangle tempRec = buttons[i].Item1;

                    Raylib.DrawRectangleRec(tempRec, Color.GRAY);
                    Raylib.DrawRectangleLinesEx(tempRec, 5, Color.BLACK);
                    Raylib.DrawText(buttons[i].Item2, (int)tempRec.x + 30, (int)tempRec.y + 30, 40, Color.ORANGE);
                    nextMenu = Click(buttons);
                }

                Raylib.EndDrawing();
            }

            return nextMenu;
        }

        static string Click((Rectangle, string, string)[] buttons)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                //Kan ha den utanför loopen men om man klickar t.ex (100, 100) men utanför loopen var man (0, 0) så får man att man klickade på (0, 0). Speedrunner issues be like...
                Vector2 mouseCords = Raylib.GetMousePosition();

                Rectangle tempRec = buttons[i].Item1;
                (int xStart, int xStop, int yStart, int yStop) buttonHitbox = ((int)tempRec.x, (int)tempRec.x + (int)tempRec.width, (int)tempRec.y, (int)tempRec.y + (int)tempRec.height);

                if (mouseCords.X > buttonHitbox.xStart && mouseCords.X < buttonHitbox.xStop && mouseCords.Y > buttonHitbox.yStart && mouseCords.Y < buttonHitbox.yStop)
                {
                    Raylib.DrawRectangleRec(tempRec, Color.DARKGRAY);
                    Raylib.DrawRectangleLinesEx(tempRec, 5, Color.DARKBROWN);
                    Raylib.DrawText(buttons[i].Item2, (int)tempRec.x + 30, (int)tempRec.y + 30, 40, Color.GOLD);

                    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                    {
                        Console.WriteLine($"Returning {buttons[i].Item3}");
                        return buttons[i].Item3;
                    }
                }
            }
            return "";
        }
    }



    class Tile
    {
        public string type;
        public float alpha;
        public (Texture2D texture, string name) building;

        public Tile(string alt, int light)
        {
            type = alt;
            alpha = light / 255f;
        }

        public void SetBuilding((Texture2D, string) buildingType)
        {
            building = buildingType;
        }
    }
}