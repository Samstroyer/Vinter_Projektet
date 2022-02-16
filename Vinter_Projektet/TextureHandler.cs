using System;
using Raylib_cs;
using System.IO;
using System.Collections.Generic;

class TextureHandler
{
    List<List<Texture2D>> textures = new List<List<Texture2D>>();

    public TextureHandler(int[] sizes)
    {
        if (sizes.Length > 0 && Directory.Exists(@"SaveData\Textures"))
        {
            if (Directory.GetFiles(@"SaveData\Textures").Length > 0)
            {
                string[] paths = Directory.GetFiles(@"SaveData\Textures");
                for (int i = 0; i < paths.Length; i++)
                {
                    Image img = Raylib.LoadImage(paths[i]);

                    foreach (int size in sizes)
                    {
                        Raylib.ImageResize(ref img, size, size);
                        textures
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("There are no 'valid textures'/'any textures' to load!");
        }
    }
}