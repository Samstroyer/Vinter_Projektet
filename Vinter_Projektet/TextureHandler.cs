using System;
using Raylib_cs;
using System.IO;
using System.Collections.Generic;

class TextureHandler
{
    List<List<Texture2D>> textures = new List<List<Texture2D>>();
    //Index of? För att hitta vilken plats det är på :p
    List<string> nameIndexer = new List<string>();
    List<int> sizesToIndex = new List<int>();

    public TextureHandler(int[] sizes)
    {
        sizesToIndex.AddRange(sizes);
        if (sizes.Length > 0 && Directory.Exists(@"SaveData\Textures"))
        {
            if (Directory.GetFiles(@"SaveData\Textures").Length > 0)
            {
                string[] paths = Directory.GetFiles(@"SaveData\Textures");
                for (int i = 0; i < paths.Length; i++)
                {
                    nameIndexer.Add(Path.GetFileNameWithoutExtension(paths[i]));
                    textures.Add(new List<Texture2D>());

                    foreach (int size in sizes)
                    {
                        Image img = Raylib.LoadImage(paths[i]);
                        Raylib.ImageResize(ref img, size, size);
                        Texture2D texture = Raylib.LoadTextureFromImage(img);
                        textures[i].Add(texture);
                        Raylib.UnloadImage(img);
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("There are no 'valid textures'/'any textures' to load!");
        }
    }

    public void RenderBuilding(int x, int y, int size, string type)
    {
        int firstDimensionIndex = nameIndexer.IndexOf(type);
        int secondDimensionIndex = sizesToIndex.IndexOf(size);
        Raylib.DrawTexture(textures[firstDimensionIndex][secondDimensionIndex], x, y, Color.WHITE);
    }
}