using System;
using Raylib_cs;

//Skapa ett 800x800 fönster som heter "The project!"
(int, int) screenSize = (800, 800);

//Skapa en MasterControl klass i Master.cs 
MasterControl mc;

//Initierar fönstret
Setup();

//Start av spelets logik
mc.LogicLoop();


void Setup()
{
    Raylib.InitWindow(screenSize.Item1, screenSize.Item2, "The project!");
    mc = new MasterControl();
}

public class MasterControl
{
    //Variablar
    string menu;

    //Klasser
    StartMenu sm;

    public MasterControl()
    {
        menu = "start";
        sm = new StartMenu();
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