using System;
using Raylib_cs;
using System.IO;
using System.Collections.Generic;

class Player
{
    //N och B är coola tangenter, dem sitter bredvid varandra och det kan stå för Next och Back!
    //Så det är så man switchar tiles i spelet som man kan placera!

    List<(Texture2D texture, string name)> textures = new List<(Texture2D texture, string name)>();

    public Player()
    {
        if (false)
        {
            //OM DET FINNS ETT SAVE GAME
        }
        else if (Directory.Exists(@"SaveData\Textures") && Directory.GetFiles(@"SaveData\Textures").Length > 0)
        {
            string[] names = Directory.GetFiles(@"SaveData\Textures");
            for (int i = 0; i < Directory.GetFiles(@"SaveData\Textures").Length; i++)
            {
                textures.Add((Raylib.LoadTexture(names[i]), names[i]));
                Console.WriteLine(textures[i].name);
            }
        }
    }

    public void DisplaySelectedItem()
    {

    }
}