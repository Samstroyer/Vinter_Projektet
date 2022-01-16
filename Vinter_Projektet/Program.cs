using System;
using Raylib_cs;

namespace Quiz
{
    class Program
    {
        static void Main(string[] args)
        {
            Setup();
            GameLoop("start");
        }

        static void Setup()
        {
            //Fixar ett 800x800 fönster med namnet "The project!"
            Raylib.InitWindow(800, 800, "The project!");
        }

        static void GameLoop(string menu)
        {
            while (!Raylib.WindowShouldClose())
            {
                switch (menu)
                {
                    case "start":
                        break;
                    default:
                        break;
                }
            }
        }

        public class StartMenu
        {
            //Variabler + alla knappar (knapp och text till knappen)
            int recWidth = 400;
            int recHeight = 100;
            (Rectangle, string)[] buttons = new (Rectangle, string)[3] { (new Rectangle(), "Play"), (new Rectangle(), "Create Map"), (new Rectangle(), "Settings") };

            public StartMenu()
            {
                //Ge alla rektanglar en position med for() loop
                int iOffset = 150;
                int startY = 300;
                for (int i = 0; i < buttons.Length; i++)
                {
                    int x = screenSize.Item1 / 2 - recWidth / 2;
                    int y = i * iOffset + startY;
                    buttons[i].Item1 = new Rectangle(x, y, recWidth, recHeight);
                }
            }

            public void DisplayStartScreen()
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);

                //Kolla om användaren klickar någonstans
                Click();

                //Rita knapparna och skriv texten
                for (int i = 0; i < buttons.Length; i++)
                {
                    Rectangle tempRec = buttons[i].Item1;
                    Raylib.DrawRectangleRec(tempRec, Color.GRAY);
                    Raylib.DrawRectangleLinesEx(tempRec, 5, Color.BLACK);
                    Raylib.DrawText(buttons[i].Item2, (int)tempRec.x + 30, (int)tempRec.y + 30, 40, Color.BLUE);
                }
                Raylib.EndDrawing();
            }

            public void DisplaySettingsScreen()
            {

            }

            private void Click()
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    Rectangle tempRec = buttons[i].Item1;
                    (int xStart, int xStop, int yStart, int yStop) buttonHitbox = ((int)tempRec.x, (int)tempRec.x + (int)tempRec.width, (int)tempRec.y, (int)tempRec.y + (int)tempRec.height);

                    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                    {
                        switch (i)
                        {
                            case 0:
                                Console.WriteLine("set menu to 'play'");
                                SetMenu("play");
                                break;
                            case 1:
                                Console.WriteLine("set menu to 'creator'");
                                SetMenu("creator");
                                break;
                            case 2:
                                Console.WriteLine("set menu to 'settings'");
                                SetMenu("settings");
                                break;
                        }
                    }
                }
            }
        }
    }
}