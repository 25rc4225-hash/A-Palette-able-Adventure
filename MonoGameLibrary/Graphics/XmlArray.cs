using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace MonoGameLibrary.Graphics
{
    public class Layout
    {
        private static int[,] map;
        private static List<string[]> entities;
        private static ContentManager _content;
        private static string _filename;
        private static Tileset _tileset;
        private static Tilemap _tilemap;
        public Layout(ContentManager content, string filename)
        {
            _content = content;
            _filename = filename;
        }
        public int[,] GetMap()
        {
            XmlArray();
            return map;
        }
        public List<string[]> GetEntites()
        {
            return entities;
        }
        public void XmlArray()
        {
            string filePath = Path.Combine(_content.RootDirectory, _filename);

            using (Stream stream = TitleContainer.OpenStream(filePath))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    XDocument doc = XDocument.Load(reader);
                    XElement root = doc.Root;

                    XElement tilesetElement = root.Element("Tileset");

                    string regionAttribute = tilesetElement.Attribute("region").Value;
                    string[] split = regionAttribute.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    int x = int.Parse(split[0]);
                    int y = int.Parse(split[1]);
                    int width = int.Parse(split[2]);
                    int height = int.Parse(split[3]);

                    int tileWidth = int.Parse(tilesetElement.Attribute("tileWidth").Value);
                    int tileHeight = int.Parse(tilesetElement.Attribute("tileHeight").Value);
                    string contentPath = tilesetElement.Value;

                    // Load the texture 2d at the content path
                    Texture2D texture = _content.Load<Texture2D>(contentPath);

                    // Create the texture region from the texture
                    TextureRegion textureRegion = new TextureRegion(texture, x, y, width, height);

                    // Create the tileset using the texture region
                    _tileset = new Tileset(textureRegion, tileWidth, tileHeight);

                    XElement tilesElement = root.Element("Tiles");

                    // Split the value of the tiles data into rows by splitting on
                    // the new line character
                    string[] rows = tilesElement.Value.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);

                    // Split the value of the first row to determine the total number of columns
                    int columnCount = rows[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;

                    map = new int[rows.Length, columnCount];

                    for (int wide = 0; wide < rows[0].Length; wide++)
                    {
                        if (rows[0][wide] != ' ')
                        {
                            bool seven = false;
                            for (int tall = 0; tall < rows.Length; tall++)
                            {
                                string value = rows[tall][wide].ToString();
                                if (value == "7")
                                {
                                    seven = true;
                                }
                                if (seven)
                                {
                                    map[tall, wide / 2] = 1;
                                }
                                else
                                {
                                    if (value == "0" || value == "1")
                                    {
                                        map[tall, wide / 2] = int.Parse(value);
                                    }
                                    else
                                    {
                                        entities.Add(new string[] { value, wide.ToString(), tall.ToString() });
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public Tilemap CreateTileMap()
        {
            int width = map.GetLength(1);
            int height = map.GetLength(0);

            // Create the tilemap
            _tilemap = new Tilemap(_tileset, width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int tileIndex = 0;
                    int rotation = 0;
                    if (map[y, x] == 0)
                    {
                        tileIndex = 27;
                    }
                    else if (map[y, x] == 1)
                    {
                        if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                        {
                            if (x == 0 && y == 0)
                            {
                                if ((map[y + 1, x] == 1))
                                {
                                    tileIndex = 19;
                                }
                                else
                                {
                                    tileIndex = 12;
                                }
                            }
                            else if (x == width - 1 && y == 0)
                            {
                                tileIndex = 21;
                            }
                            else if (x == 0 && y == height - 1)
                            {
                                if (map[y - 1, x + 1] == 1)
                                {
                                    tileIndex = 11;
                                }
                                else
                                {
                                    tileIndex = 12;
                                }
                            }
                        }
                        else
                        {
                            bool one = map[y - 1, x - 1] == 1;
                            bool two = map[y - 1, x] == 1;
                            bool three = map[y - 1, x + 1] == 1;
                            bool four = map[y, x - 1] == 1;
                            bool six = map[y, x + 1] == 1;
                            bool seven = map[y + 1, x - 1] == 1;
                            bool eight = map[y + 1, x] == 1;
                            bool nine = map[y + 1, x + 1] == 1;

                            // Solitary island check
                            if (one && !two && !four)
                            {
                                one = false;
                            }
                            if (three && !two && !six)
                            {
                                three = false;
                            }
                            if (seven && !four && !eight)
                            {
                                seven = false;
                            }
                            if (nine && !six && !eight)
                            {
                                nine = false;
                            }


                            bool[] boolCount = { one, two, three, four, six, seven, eight, nine };
                            int bools = boolCount.Count(c => c == true);
                            if (bools == 0)
                            {
                                tileIndex = 26;
                            }
                            else if (bools == 1)
                            {
                                if (two)
                                {
                                    tileIndex = 13;
                                }
                                if (four)
                                {
                                    tileIndex = 23;
                                }
                                if (six)
                                {
                                    tileIndex = 25;
                                }
                                if (eight)
                                {
                                    tileIndex = 24;
                                }
                            }
                            else if (bools == 2)
                            {
                                if (one)
                                {
                                    if (two)
                                    {
                                        tileIndex = 13;
                                    }
                                    else if (four)
                                    {
                                        tileIndex = 25;
                                    }
                                }
                                else if (two)
                                {
                                    if (three)
                                    {
                                        tileIndex = 13;
                                    }
                                    else if (six)
                                    {
                                        tileIndex = 12;
                                    }
                                    else if (eight)
                                    {
                                        tileIndex = 10;
                                    }
                                }
                                else if (three)
                                {
                                    tileIndex = 23;
                                }
                                else if (four)
                                {
                                    if (six)
                                    {
                                        tileIndex = 10;
                                        rotation = 90;
                                    }
                                    else if (eight)
                                    {
                                        tileIndex = 25;
                                    }
                                }
                                else if (six)
                                {
                                    tileIndex = 23;
                                }
                                else if (seven)
                                {
                                    tileIndex = 24;
                                }
                                else if (eight)
                                {
                                    tileIndex = 24;
                                }

                            }
                            else if (bools == 3)
                            {
                                if (one)
                                {
                                    if (two)
                                    {
                                        if (three)
                                        {
                                            tileIndex = 13;
                                        }
                                        else if (four)
                                        {
                                            tileIndex = 11;
                                            rotation = 270;
                                        }
                                        else if (six)
                                        {
                                            tileIndex = 12;
                                        }
                                        else if (eight)
                                        {
                                            tileIndex = 10;
                                        }
                                    }
                                    else if (four)
                                    {
                                        if (six)
                                        {
                                            tileIndex = 10;
                                            rotation = 90;
                                        }
                                        else if (seven)
                                        {
                                            tileIndex = 25;
                                        }
                                        else if (eight)
                                        {
                                            tileIndex = 12;
                                            rotation = 180;
                                        }
                                    }
                                }
                                else if (two)
                                {
                                    if (three)
                                    {
                                        if (four)
                                        {
                                            tileIndex = 12;
                                            rotation = 270;
                                        }
                                        else if (six)
                                        {
                                            tileIndex = 11;
                                        }
                                        else if (eight)
                                        {
                                            tileIndex = 10;
                                        }
                                    }
                                    else if (four)
                                    {
                                        if (six)
                                        {
                                            tileIndex = 9;
                                            rotation = 270;
                                        }
                                        else if (seven)
                                        {
                                            tileIndex = 12;
                                            rotation = 270;
                                        }
                                        else if (eight)
                                        {
                                            tileIndex = 9;
                                            rotation = 180;
                                        }
                                    }
                                    else if (six)
                                    {
                                        if (eight)
                                        {
                                            tileIndex = 9;
                                        }
                                        else if (nine)
                                        {
                                            tileIndex = 12;
                                        }
                                    }
                                    else if (seven)
                                    {
                                        tileIndex = 10;
                                    }
                                    else if (eight)
                                    {
                                        tileIndex = 10;
                                    }
                                }
                                else if (three)
                                {
                                    if (four)
                                    {
                                        tileIndex = 18;
                                    }
                                    else if (six)
                                    {
                                        if (eight)
                                        {
                                            tileIndex = 12;
                                            rotation = 90;
                                        }
                                        else if (nine)
                                        {
                                            tileIndex = 23;
                                        }
                                    }
                                }
                                else if (four)
                                {
                                    if (six)
                                    {
                                        if (seven)
                                        {
                                            tileIndex = 18;
                                        }
                                        else if (eight)
                                        {
                                            tileIndex = 17;
                                        }
                                        else if (nine)
                                        {
                                            tileIndex = 18;
                                        }
                                    }
                                    else if (seven)
                                    {
                                        tileIndex = 21;
                                    }
                                    else if (eight)
                                    {
                                        tileIndex = 22;
                                    }
                                }
                                else if (six)
                                {
                                    if (seven)
                                    {
                                        tileIndex = 12;
                                        rotation = 90;
                                    }
                                    else if (eight)
                                    {
                                        tileIndex = 19;
                                    }
                                }
                                else if (seven)
                                {
                                    tileIndex = 24;
                                }
                            }
                            else if (bools == 4)
                            {
                                if (one)
                                {
                                    if (two)
                                    {
                                        if (three)
                                        {
                                            if (four)
                                            {
                                                tileIndex = 11;
                                                rotation = 270;
                                            }
                                            else if (six)
                                            {
                                                tileIndex = 11;
                                            }
                                            else if (eight)
                                            {
                                                tileIndex = 10;
                                            }
                                        }
                                        else if (four)
                                        {
                                            if (six)
                                            {
                                                tileIndex = 8;
                                                rotation = 270;
                                            }
                                            else if (seven)
                                            {
                                                tileIndex = 11;
                                                rotation = 270;
                                            }
                                            else if (eight)
                                            {
                                                tileIndex = 7;
                                                rotation = 180;
                                            }
                                        }
                                        else if (six)
                                        {
                                            if (eight)
                                            {
                                                tileIndex = 9;
                                            }
                                            else if (nine)
                                            {
                                                tileIndex = 12;
                                            }
                                        }
                                        else if (seven)
                                        {
                                            tileIndex = 10;
                                        }
                                        else if (eight)
                                        {
                                            tileIndex = 10;
                                        }
                                    }
                                    else if (three)
                                    {
                                        tileIndex = 18;
                                    }
                                    else if (four)
                                    {
                                        if (seven)
                                        {
                                            tileIndex = 18;
                                        }
                                        else if (eight)
                                        {
                                            tileIndex = 17;
                                        }
                                        else if (nine)
                                        {
                                            tileIndex = 18;
                                        }
                                    }
                                }
                                else if (two)
                                {
                                    if (three)
                                    {
                                        if (four)
                                        {
                                            if (six)
                                            {
                                                tileIndex = 7;
                                                rotation = 270;
                                            }
                                            else if (seven)
                                            {
                                                tileIndex = 12;
                                                rotation = 270;
                                            }
                                            else if (eight)
                                            {
                                                tileIndex = 9;
                                                rotation = 180;
                                            }
                                        }
                                        else if (six)
                                        {
                                            if (eight)
                                            {
                                                tileIndex = 8;
                                            }
                                            else if (nine)
                                            {
                                                tileIndex = 11;
                                            }
                                        }
                                        else if (seven)
                                        {
                                            tileIndex = 10;
                                        }
                                        else if (eight)
                                        {
                                            tileIndex = 10;
                                        }
                                    }
                                    else if (four)
                                    {
                                        if (six)
                                        {
                                            if (seven)
                                            {
                                                tileIndex = 9;
                                                rotation = 270;
                                            }
                                            else if (eight)
                                            {
                                                tileIndex = 5;
                                            }
                                            else if (nine)
                                            {
                                                tileIndex = 9;
                                                rotation = 270;
                                            }
                                        }
                                        else if (seven)
                                        {
                                            tileIndex = 8;
                                            rotation = 180;
                                        }
                                        else if (eight)
                                        {
                                            tileIndex = 9;
                                            rotation = 180;
                                        }
                                    }
                                    else if (six)
                                    {
                                        if (seven)
                                        {
                                            tileIndex = 9;
                                        }
                                        else if (eight)
                                        {
                                            tileIndex = 9;
                                        }
                                    }
                                    else if (seven)
                                    {
                                        tileIndex = 10;
                                    }
                                }
                                else if (three)
                                {
                                    if (four)
                                    {
                                        if (seven)
                                        {
                                            tileIndex = 18;
                                        }
                                        else if (eight)
                                        {
                                            tileIndex = 17;
                                        }
                                        else if (nine)
                                        {
                                            tileIndex = 18;
                                        }
                                    }
                                    else if (six)
                                    {
                                        if (seven)
                                        {
                                            tileIndex = 20;
                                        }
                                        else if (eight)
                                        {
                                            tileIndex = 19;
                                        }
                                    }
                                }
                                else if (four)
                                {
                                    if (six)
                                    {
                                        if (seven)
                                        {
                                            if (eight)
                                            {
                                                tileIndex = 15;
                                            }
                                            else if (nine)
                                            {
                                                tileIndex = 18;
                                            }
                                        }
                                        else if (eight)
                                        {
                                            tileIndex = 16;
                                        }
                                    }
                                    else if (seven)
                                    {
                                        tileIndex = 21;
                                    }
                                }
                                else if (six)
                                {
                                    tileIndex = 19;
                                }
                            }
                            else if (bools == 5)
                            {
                                if (!one)
                                {
                                    if (!two)
                                    {
                                        if (!three)
                                        {
                                            tileIndex = 14;
                                        }
                                        else if (!four)
                                        {
                                            tileIndex = 19;
                                        }
                                        else if (!seven)
                                        {
                                            tileIndex = 16;
                                        }
                                        else if (!eight)
                                        {
                                            tileIndex = 18;
                                        }
                                        else if (!nine)
                                        {
                                            tileIndex = 15;
                                        }
                                    }
                                    else if (!three)
                                    {
                                        if (!four)
                                        {
                                            tileIndex = 7;
                                        }
                                        else if (!six)
                                        {
                                            tileIndex = 8;
                                        }
                                        else if (!seven)
                                        {
                                            tileIndex = 4;
                                        }
                                        else if (!eight)
                                        {
                                            tileIndex = 9;
                                            rotation = 270;
                                        }
                                        else if (!nine)
                                        {
                                            tileIndex = 4;
                                            rotation = 90;
                                        }
                                    }
                                    else if (!four)
                                    {
                                        if (!six)
                                        {
                                            tileIndex = 10;
                                        }
                                        else if (!seven)
                                        {
                                            tileIndex = 6;
                                        }
                                        else if (!nine)
                                        {
                                            tileIndex = 8;
                                        }
                                    }
                                    else if (!six)
                                    {
                                        if (!seven)
                                        {
                                            tileIndex = 9;
                                            rotation = 180;
                                        }
                                        else if (!nine)
                                        {
                                            tileIndex = 8;
                                            rotation = 180;
                                        }
                                    }
                                    else if (!seven)
                                    {
                                        if (!eight)
                                        {
                                            tileIndex = 7;
                                            rotation = 270;
                                        }
                                        else if (!nine)
                                        {
                                            tileIndex = 8;
                                            rotation = 180;
                                        }
                                    }
                                    else if (!eight)
                                    {
                                        tileIndex = 7;
                                        rotation = 270;
                                    }
                                }
                                else if (!two)
                                {
                                    if (!three)
                                    {
                                        if (!six)
                                        {
                                            tileIndex = 21;
                                        }
                                        else if (!seven)
                                        {
                                            tileIndex = 16;
                                        }
                                        else if (!eight)
                                        {
                                            tileIndex = 18;
                                        }
                                        else if (!nine)
                                        {
                                            tileIndex = 15;
                                        }
                                    }
                                    else if (!seven)
                                    {
                                        if (!eight)
                                        {
                                            tileIndex = 18;
                                        }
                                        else if (!nine)
                                        {
                                            tileIndex = 17;
                                        }
                                    }
                                    else if (!eight)
                                    {
                                        tileIndex = 18;
                                    }
                                }
                                else if (!three)
                                {
                                    if (!four)
                                    {
                                        if (!six)
                                        {
                                            tileIndex = 10;
                                        }
                                        else if (!seven)
                                        {
                                            tileIndex = 7;
                                        }
                                        else if (!nine)
                                        {
                                            tileIndex = 9;
                                        }
                                    }
                                    else if (!six)
                                    {
                                        if (!seven)
                                        {
                                            tileIndex = 7;
                                            rotation = 180;
                                        }
                                        else if (!nine)
                                        {
                                            tileIndex = 6;
                                            rotation = 180;
                                        }
                                    }
                                    else if (!seven)
                                    {
                                        if (!eight)
                                        {
                                            tileIndex = 8;
                                            rotation = 270;
                                        }
                                        else if (!nine)
                                        {
                                            tileIndex = 4;
                                            rotation = 180;
                                        }
                                    }
                                    else if (!eight)
                                    {
                                        tileIndex = 8;
                                        rotation = 270;
                                    }
                                }
                                else if (!four)
                                {
                                    if (!six)
                                    {
                                        if (!seven)
                                        {
                                            tileIndex = 10;
                                        }
                                        else if (!nine)
                                        {
                                            tileIndex = 10;
                                        }
                                    }
                                    else if (!seven)
                                    {
                                        if (!eight)
                                        {
                                            tileIndex = 11;
                                        }
                                        else if (!nine)
                                        {
                                            tileIndex = 8;
                                        }
                                    }
                                }
                                else if (!six)
                                {
                                    if (!seven)
                                    {
                                        tileIndex = 7;
                                        rotation = 180;
                                    }
                                    else if (!eight)
                                    {
                                        tileIndex = 11;
                                        rotation = 270;
                                    }
                                }
                                else if (!seven)
                                {
                                    tileIndex = 6;
                                    rotation = 270;
                                }
                            }
                            else if (bools == 6)
                            {
                                if (!one)
                                {
                                    if (!two)
                                    {
                                        tileIndex = 14;
                                    }
                                    else if (!three)
                                    {
                                        tileIndex = 2;
                                    }
                                    else if (!four)
                                    {
                                        tileIndex = 6;
                                    }
                                    else if (!six)
                                    {
                                        tileIndex = 8;
                                        rotation = 180;
                                    }
                                    else if (!seven)
                                    {
                                        tileIndex = 2;
                                        rotation = 270;
                                    }
                                    else if (!eight)
                                    {
                                        tileIndex = 7;
                                        rotation = 270;
                                    }
                                    else if (!nine)
                                    {
                                        tileIndex = 3;
                                    }
                                }
                                else if (!two)
                                {
                                    if (!three)
                                    {
                                        tileIndex = 14;
                                    }
                                    else if (!seven)
                                    {
                                        tileIndex = 16;
                                    }
                                    else if (!eight)
                                    {
                                        tileIndex = 18;
                                    }
                                    else if (!nine)
                                    {
                                        tileIndex = 15;
                                    }
                                }
                                else if (!three)
                                {
                                    if (!four)
                                    {
                                        tileIndex = 7;
                                    }
                                    else if (!six)
                                    {
                                        tileIndex = 6;
                                        rotation = 180;
                                    }
                                    else if (!seven)
                                    {
                                        tileIndex = 3;
                                        rotation = 90;
                                    }
                                    else if (!eight)
                                    {
                                        tileIndex = 8;
                                        rotation = 270;
                                    }
                                    else if (!nine)
                                    {
                                        tileIndex = 2;
                                        rotation = 90;
                                    }
                                }
                                else if (!four)
                                {
                                    if (!six)
                                    {
                                        tileIndex = 10;
                                    }
                                    else if (!seven)
                                    {
                                        tileIndex = 6;
                                    }
                                    else if (!nine)
                                    {
                                        tileIndex = 8;
                                    }
                                }
                                else if (!six)
                                {
                                    if (!seven)
                                    {
                                        tileIndex = 7;
                                        rotation = 180;
                                    }
                                    else if (!nine)
                                    {
                                        tileIndex = 6;
                                        rotation = 180;
                                    }
                                }
                                else if (!seven)
                                {
                                    if (!eight)
                                    {
                                        tileIndex = 6;
                                        rotation = 270;
                                    }
                                    else if (!nine)
                                    {
                                        tileIndex = 2;
                                        rotation = 180;
                                    }
                                }
                                else if (!eight)
                                {
                                    tileIndex = 6;
                                    rotation = 270;
                                }
                            }
                            else if (bools == 7)
                            {
                                if (!one)
                                {
                                    tileIndex = 1;
                                }
                                else if (!two)
                                {
                                    tileIndex = 14;
                                }
                                else if (!three)
                                {
                                    tileIndex = 1;
                                    rotation = 90;
                                }
                                else if (!four)
                                {
                                    tileIndex = 6;
                                }
                                else if (!six)
                                {
                                    tileIndex = 6;
                                    rotation = 180;
                                }
                                else if (!seven)
                                {
                                    tileIndex = 1;
                                    rotation = 270;
                                }
                                else if (!eight)
                                {
                                    tileIndex = 6;
                                    rotation = 270;
                                }
                                else if (!nine)
                                {
                                    tileIndex = 1;
                                    rotation = 180;
                                }
                            }
                            else if (bools == 8)
                            {
                                tileIndex = 0;
                            }
                        }
                        if (tileIndex == 0)
                        {
                            Random rng = new Random();
                            rotation = rng.Next(0, 4) * 90;
                        }
                    }


                    _tilemap.SetTile(x, y, tileIndex, rotation);
                }
            }
            return _tilemap;
        }
    }
}


