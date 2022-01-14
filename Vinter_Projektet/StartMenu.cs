using System;
using Raylib_cs;

public class StartMenu
{
    Rectangle[] recs = new Rectangle[3];
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
        for (int i = 0; i < recs.Length; i++)
        {
            int x = screenSize.Item1 / 2 - recWidth / 2;
            int y = i * iOffset + startY;
            recs[i] = new Rectangle(x, y, recWidth, recHeight);
        }
    }

    public void Display()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.WHITE);
        foreach (Rectangle rec in recs)
        {
            Raylib.DrawRectangleRec(rec, Color.GRAY);
        }
        Raylib.EndDrawing();
    }
}