using System;
using Raylib_cs;

public class StartMenu
{
    //     Rectangle[] recs = new Rectangle[3];
    //     string[] buttonText = new string[3] { "Play", "Create Map", "Settings" };
    (Rectangle, string)[] buttons = new (Rectangle, string)[3] { (new Rectangle(), "Play"), (new Rectangle(), "Create Map"), (new Rectangle(), "Settings") };
    int recWidth, recHeight;
    (int, int) screenSize;

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

    public void Display()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.WHITE);

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
}