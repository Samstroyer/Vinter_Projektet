using System;
using Raylib_cs;

public class StartMenu
{
    //Variabler och klasser
    int recWidth, recHeight;
    (int, int) screenSize;
    (Rectangle, string)[] buttons = new (Rectangle, string)[3] { (new Rectangle(), "Play"), (new Rectangle(), "Create Map"), (new Rectangle(), "Settings") };
    MasterControl mc = new MasterControl();

    public StartMenu((int, int) actualSize)
    {
        //Eftersom det är olika filer (min enda gissning på problemet) går det inte att köra Raylib.GetScreenWidth() utan att få 0 ALLTID! Måste därför importera det i en variabel
        screenSize = actualSize;
        recWidth = 400;
        recHeight = 100;

        //Counters för alla knappar i "Start Menyn" - matten blir (rectangle.y = i*iOffset + startY)
        int iOffset = 150;
        int startY = 300;

        //Ge alla rektanglar en position med for() loop
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

        //Rita knapparna och 
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
                        mc.SetMenu("play");
                        break;
                    case 1:
                        Console.WriteLine("set menu to 'creator'");
                        mc.SetMenu("creator");
                        break;
                    case 2:
                        Console.WriteLine("set menu to 'settings'");
                        mc.SetMenu("settings");
                        break;
                }
            }
        }
    }
}