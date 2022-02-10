using System;
using System.Numerics;
using Raylib_cs;
using System.IO;
using System.Collections.Generic;

class Player
{
    //N och B är coola tangenter, dem sitter bredvid varandra och det kan stå för Next och Back!
    //Så det är så man switchar tiles i spelet som man kan placera!

    List<(Texture2D texture, string name)> items = new List<(Texture2D texture, string name)>();
    int selectedItem = 0;
    public bool readyToPlace = false;

    //Standard av alla texture widths i mitt spel. Alla textures är egengjorda med hjälp av DigDig
    int texWidth = 25;

    public Player()
    {
        if (false)
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
                    items.Add((texture, name));
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

            Raylib.DrawTexture(items[selectedItem].texture, (int)itemRenderPos.X - texWidth / 2, (int)itemRenderPos.Y - texWidth / 2, Color.WHITE);

        }
    }

    public (Texture2D, string) ChangeTileTypeToSelectedItem()
    {
        return items[selectedItem];
    }

    public void SwitchSelectedItem(int indexChanger)
    {
        selectedItem += indexChanger;
        if (selectedItem < 0)
        {
            selectedItem = items.Count - 1;
        }
        else if (selectedItem > items.Count - 1)
        {
            selectedItem = 0;
        }
    }
}