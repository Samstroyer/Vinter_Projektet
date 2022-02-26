using System;
using System.Numerics;
using Raylib_cs;
using System.IO;
using System.Collections.Generic;

class Player
{
    int selectedItem = 0;
    BigInteger money = 0;
    int moneyPerInterval = 0;
    public bool readyToPlace = false;
    List<(Texture2D texture, string name)> baseTextures = new List<(Texture2D texture, string name)>();

    List<(string type, (int amount, int generation))> buldingIncome = new List<(string type, (int amount, int generation))>();
    List<(string type, int amount)> inventory = new List<(string type, int amount)>();

    //Standard av alla texture widths i mitt spel. Alla textures är egengjorda med hjälp av DigDig
    int texWidth = 25;

    public Player(bool useSave)
    {
        if (useSave)
        {
            //OM DET FINNS ETT SAVE GAME
        }
        else if (Directory.Exists(@"SaveData\Textures"))
        {
            if (Directory.GetFiles(@"SaveData\Textures").Length > 0)
            {
                string[] paths = Directory.GetFiles(@"SaveData\Textures");
                for (int i = 0; i < paths.Length; i++)
                {
                    Texture2D texture = Raylib.LoadTexture(paths[i]);
                    string name = Path.GetFileNameWithoutExtension(paths[i]);
                    baseTextures.Add((texture, name));
                    inventory.Add((name, 0));
                    buldingIncome.Add((name, (0, (int)Math.Pow(i, 2) + 4)));
                }
                inventory[0] = (inventory[0].type, 1);
            }
            else
            {
                Console.WriteLine("There is no textures in the texture folder!");
            }
        }
        else
        {
            Console.Write("You should not see this message! Something went really wrong...");
        }
    }

    public void MoneyPerIntervalUpdater()
    {
        moneyPerInterval = 0;
        foreach ((string, (int, int)) obj in buldingIncome)
        {
            (int, int) amountAndGeneration = obj.Item2;
            if (amountAndGeneration.Item1 > 0)
            {
                moneyPerInterval += amountAndGeneration.Item1 * amountAndGeneration.Item2;
            }
            Console.WriteLine("New moneyPerInterval is now: {0}", moneyPerInterval);
        }
    }

    public void DisplaySelectedItem()
    {
        if (readyToPlace)
        {
            Vector2 itemRenderPos = Raylib.GetMousePosition();

            Raylib.DrawTexture(baseTextures[selectedItem].texture, (int)itemRenderPos.X - texWidth / 2, (int)itemRenderPos.Y - texWidth / 2, Color.WHITE);

        }
    }

    public string ChangeTileTypeToSelectedItem(string buildingBefore)
    {
        int index = 0;
        for (int i = 0; i < inventory.Count; i++)
        {
            (string, int) tempObj = inventory[i];
            if (tempObj.Item1 == buildingBefore)
            {
                index = i;
            }
        }
        (string, int) obj = inventory[index];



        if (readyToPlace && obj.Item2 > 0)
        {
            return baseTextures[selectedItem].name;
        }
        else
        {
            return buildingBefore;
        }
    }

    public void ChangeInventory(string type, int changeAmount)
    {
        int index = 0;
        for (int i = 0; i < inventory.Count; i++)
        {
            (string, int) tempObj = inventory[i];
            if (tempObj.Item1 == type)
            {
                index = i;
            }
        }

        (string, int) obj = inventory[index];
        obj.Item2 += changeAmount;

        if (obj.Item2 < 0)
        {
            obj.Item2 = 0;
        }

        inventory[index] = obj;
    }

    public string GetMoney()
    {
        return money.ToString();
    }

    public void MoneyTick()
    {
        money += moneyPerInterval;
    }

    public void SwitchSelectedItem(int indexChanger)
    {
        selectedItem += indexChanger;
        if (selectedItem < 0)
        {
            selectedItem = baseTextures.Count - 1;
        }
        else if (selectedItem > baseTextures.Count - 1)
        {
            selectedItem = 0;
        }
    }

    public void AddRemoveTileFromListOfIncomes(string type, int amount)
    {
        for (int i = 0; i < buldingIncome.ToArray().GetLength(0); i++)
        {
            (string, (int, int)) tempObj = buldingIncome[i];
            if (tempObj.Item1 == type)
            {
                (int, int) obj = buldingIncome[i].Item2;
                obj.Item1 += amount;
                (string, (int, int)) newVals = (buldingIncome[i].type, (obj));
                buldingIncome[i] = newVals;
            }
        }
    }

    public void ShowInventory()
    {
        //Jag blev lärd av Hugo's kod att man kan göra en break i en while loop för att breaka ut ur den, nice!
        while (true)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_I))
            {
                break;
            }

            Raylib.BeginDrawing();

            Raylib.ClearBackground(Color.BLACK);


            Raylib.EndDrawing();
        }
    }
}