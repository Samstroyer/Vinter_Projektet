using System;
using Raylib_cs;
using System.IO;
using System.Linq;
using System.Numerics;

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

                                //Om man körde javascript skulle det finnas en cool arrowfunction här... RIP
                                mapTiles[xx + x * 4, yy + y * 4] = GenerateTile(brightness);
                            }
                        }
                    }
                }
                Raylib.UnloadImage(noiseImage);


                StartGame(mapTiles);

                // shows image on the screen - useless RN
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
            int[] pixelsPerTile = new int[10] { 1, 2, 4, 8, 16, 20, 32, 40, 50, 80 };
            int indexer = 6;
            int currPixelSize = pixelsPerTile[indexer];

            Vector2 cameraPos = new Vector2(0, 0);

            //Only need a start position as the pixelsPerTile will tell me the stop position
            //The "size" arguments will be from the Tile[,] which is 1000x1000
            Vector2 cameraStart = new Vector2(450, 450);

            bool playing = true;

            while (playing && !Raylib.WindowShouldClose())
            {
                int visibleTiles = 800 / currPixelSize;
                Vector2 cameraStop;

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.PURPLE);

                //Movement on the map
                int modifier = currPixelSize == 1 || currPixelSize == 2 ? 5 : 1;
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

            RenderStart:
                cameraStop.X = cameraStart.X + visibleTiles;
                cameraStop.Y = cameraStart.Y + visibleTiles;

                for (int x = (int)cameraStart.X; x < cameraStop.X; x++)
                {
                    for (var y = (int)cameraStart.Y; y < cameraStop.Y; y++)
                    {
                        if ((x < 0 || x > 999) || (y < 0 || y > 999))
                        {
                            Console.WriteLine($"Don't zoom while drinking! Will not load tile [{x}, {y}]");
                            cameraStart = FixRender(currPixelSize, cameraStart);
                            Raylib.ClearBackground(Color.PURPLE);
                            goto RenderStart;
                        }
                        else
                        {
                            RenderChunk((x - (int)cameraStart.X, y - (int)cameraStart.Y), map[x, y], currPixelSize);
                            TileInteraction((x - (int)cameraStart.X, y - (int)cameraStart.Y), currPixelSize);
                        }
                    }
                }

                Raylib.EndDrawing();




            }
        }

        static void TileInteraction((int x, int y) cords, int size)
        {
            Vector2 mp = Raylib.GetMousePosition();
            if(cords.x < mp.X) {

            }
        }

        static Vector2 FixRender(int currPixelSize, Vector2 cameraStart)
        {
            int maxIndexX = (int)cameraStart.X + 800 / currPixelSize;
            int maxIndexY = (int)cameraStart.Y + 800 / currPixelSize;

            if (maxIndexX >= 1000)
            {
                cameraStart.X = 999 - 800 / currPixelSize;
            }
            if (maxIndexY >= 1000)
            {
                cameraStart.Y = 999 - 800 / currPixelSize;
            }

            return cameraStart;
        }

        static void RenderChunk((int x, int y) cords, Tile t, int size)
        {
            switch (t.type)
            {
                case "water":
                    Raylib.DrawRectangle(cords.x * size, cords.y * size, size, size, Color.BLUE);
                    break;
                case "flat":
                    Raylib.DrawRectangle(cords.x * size, cords.y * size, size, size, Color.ORANGE);
                    break;
                case "forrest":
                    Raylib.DrawRectangle(cords.x * size, cords.y * size, size, size, Color.GREEN);
                    break;
                case "stone":
                    Raylib.DrawRectangle(cords.x * size, cords.y * size, size, size, Color.GRAY);
                    break;
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
            (Rectangle, string, string)[] buttons = new (Rectangle, string, string)[3] { (new Rectangle(), "Play", "play"), (new Rectangle(), "Create Map", "creator"), (new Rectangle(), "Settings", "settings") };
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

        public Tile(string alt, int light)
        {
            type = alt;
            alpha = light / 255f;
        }
    }
}