using System;
using System.Numerics;
using Raylib_cs;
using System.IO;
using System.Collections.Generic;

class Player
{
    int selectedItem = 0;
    BigInteger money = 0;
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
                    buldingIncome.Add((name, (0, 0)));
                }
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

    public void DisplaySelectedItem()
    {
        if (readyToPlace)
        {
            Vector2 itemRenderPos = Raylib.GetMousePosition();

            Raylib.DrawTexture(baseTextures[selectedItem].texture, (int)itemRenderPos.X - texWidth / 2, (int)itemRenderPos.Y - texWidth / 2, Color.WHITE);

        }
    }

    public string ChangeTileTypeToSelectedItem()
    {
        if (readyToPlace)
        {
            return baseTextures[selectedItem].name;
        }
        else
        {
            return null;
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
        inventory[index] = obj;
    }

    public string GetMoney()
    {
        return money.ToString();
    }

    public void ChangeMoney(double amount)
    {
        money += ((int)amount);
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

    public double TileWorth(string type)
    {
        
    }
}