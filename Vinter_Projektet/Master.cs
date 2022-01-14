using System;
using Raylib_cs;

//Variablar
string menu = "start";
(int, int) screenSize = (800, 800);

//Klasser (kommer efteråt då jag behöver t.ex screenSize)
StartMenu sm = new StartMenu(screenSize);

//Start av spelets logik
Setup();
LogicLoop();

void Setup()
{
    //Skapa ett 800x800 fönster som heter "The project!"
    Raylib.InitWindow(screenSize.Item1, screenSize.Item2, "The project!");
}

void LogicLoop()
{
    while (!Raylib.WindowShouldClose())
    {
        switch (menu)
        {
            case "start":
                sm.Display();
                break;
            default:
                break;
        }
    }
}

void SetMenu(string newMenu)
{
    //Ändrar värdet på "menu" vilket visar vilken meny man är i. T.ex från "start" till "map"
    menu = newMenu;
}