﻿using System;
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
        }

        static void GameLoop()
        {
            string menu = "creator";
            while (!Raylib.WindowShouldClose())
            {
                switch (menu)
                {
                    case "start":
                        menu = DisplayStartScreen();
                        break;
                    case "creator":
                        CreateMap();
                        menu = Creator();
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
            Tile[,] map = new Tile[1000, 1000];


            if (Directory.Exists(@"SaveData\Pregens"))
            {
                int pregenAlternatives = Directory.GetFiles(@"SaveData\Pregens").Count();
                selectedImage = ran.Next(pregenAlternatives).ToString() + ".png";
                Image noiseImage = Raylib.LoadImage(@$"SaveData\Pregens\{selectedImage}");
                noiseImage = Raylib.LoadImage(@$"SaveData\Pregens\12.png");

                //Generatorn : bild till karta
                Color color = Raylib.GetImageColor(noiseImage, 0, 0);

                Raylib.BeginDrawing();
                Raylib.EndDrawing();

                Raylib.UnloadImage(noiseImage);
            }
            else
            {
                Console.WriteLine("Error loading, no saves directory!");
                Raylib.CloseWindow();
            }
        }

        static string Creator()
        {
            int[] pixelsPerTile = new int[13] { 2, 5, 8, 10, 16, 20, 25, 32, 40, 50, 80, 100, 160 };



            int currPixels = pixelsPerTile[6];
            bool playing = true;

            while (playing && !Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);
                Raylib.DrawRectangle(200, 200, 200, 200, Color.ORANGE);
                Raylib.EndDrawing();
            }

            return "menu";
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
        Vector2 cords = new Vector2();
        public string type;

        Tile(int x, int y)
        {
            cords.X = x;
            cords.Y = y;
        }
    }
}