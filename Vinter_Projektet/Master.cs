using System;
using Raylib_cs;

//Skapa en MasterControl klass i Master.cs 
MasterControl mc = new MasterControl();

//Start av spelets logik
mc.Setup();
mc.LogicLoop();


public class MasterControl
{
    //Variablar
    string menu = "start";
    public (int, int) screenSize = (800, 800);

    //Klasser
    StartMenu sm = new StartMenu(screenSize);

    public void Setup()
    {
        //Skapa ett 800x800 fönster som heter "The project!"
        Raylib.InitWindow(screenSize.Item1, screenSize.Item2, "The project!");
    }

    public void LogicLoop()
    {
        while (!Raylib.WindowShouldClose())
        {
            switch (menu)
            {
                case "start":
                    sm.DisplayStartScreen();
                    break;
                default:
                    break;
            }
        }
    }
    
    public void SetMenu(string newMenu)
    {
        //Ändrar värdet på "menu" vilket visar vilken meny man är i. T.ex från "start" till "map"
        menu = newMenu;
    }
}