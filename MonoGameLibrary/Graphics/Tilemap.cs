using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class Tilemap
{
    private readonly Tileset _tileset;

    private readonly int[] _tiles;

    private readonly int[] _rotates;

    /// Gets the total number of rows in this tilemap.
    public int Rows { get; }

    /// Gets the total number of columns in this tilemap.
    public int Columns { get; }

    /// Gets the total number of tiles in this tilemap.
    public int Count { get; }

    /// Gets or Sets the scale factor to draw each tile at.
    public Vector2 Scale { get; set; }

    /// Gets the width, in pixels, each tile is drawn at.
    public float TileWidth => _tileset.TileWidth * Scale.X;

    /// Gets the height, in pixels, each tile is drawn at.
    public float TileHeight => _tileset.TileHeight * Scale.Y;

    /// Creates a new tilemap.
    public Tilemap(Tileset tileset, int columns, int rows)
    {
        _tileset = tileset;
        Rows = rows;
        Columns = columns;
        Count = Columns * Rows;
        Scale = Vector2.One;
        _tiles = new int[Count];
        _rotates = new int[Count];
    }

    /// Sets the tile at the given index in this tilemap to use the tile from
    /// the tileset at the specified tileset id.
    public void SetTile(int index, int tilesetID, int degree)
    {
        _tiles[index] = tilesetID;
        _rotates[index] = degree;
    }

    /// Sets the tile at the given column and row in this tilemap to use the tile
    /// from the tileset at the specified tileset id.
    public void SetTile(int column, int row, int tilesetID, int degree)
    {
        int index = row * Columns + column;
        SetTile(index, tilesetID, degree);
    }

    /// Gets the texture region of the tile from this tilemap at the specified index.
    public TextureRegion GetTile(int index)
    {
        return _tileset.GetTile(_tiles[index]);
    }

    /// Gets the texture region of the tile from this tilemap at the specified
    /// column and row.
    public TextureRegion GetTile(int column, int row)
    {
        int index = row * Columns + column;
        return GetTile(index);
    }

    /// Gets the rotation of the tile from this tilemap at the specified index.
    public float GetRotate(int index)
    {
        return _tiles[index] / 360;
    }

    /// Gets the rotation of the tile from this tilemap at the specified
    /// column and row.
    public float GetRotate(int column, int row)
    {
        int index = row * Columns + column;
        return GetRotate(index);
    }

    /// Draws this tilemap using the given sprite batch.
    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < Count; i++)
        {
            int tilesetIndex = _tiles[i];
            TextureRegion tile = _tileset.GetTile(tilesetIndex);
            float rotation = _rotates[i];

            int x = i % Columns;
            int y = i / Columns;

            Vector2 position = new Vector2((x + 0.5f) * TileWidth, (y + 0.5f) * TileHeight);
            tile.Draw(spriteBatch, position, Color.White, MathHelper.ToRadians(rotation), new Vector2(TileWidth,TileHeight) / Scale * 0.5f, Scale, SpriteEffects.None, 1.0f);
        }
    }

    /// Creates a new tilemap based on a tilemap xml configuration file.
    public static Tilemap FromFile(ContentManager content, string filename)
    {
        string filePath = Path.Combine(content.RootDirectory, filename);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                // The <Tileset> element contains the information about the tileset
                // used by the tilemap.
                //
                // Example
                // <Tileset region="0 0 100 100" tileWidth="10" tileHeight="10">contentPath</Tileset>
                //
                // The region attribute represents the x, y, width, and height
                // components of the boundary for the texture region within the
                // texture at the contentPath specified.
                //
                // the tileWidth and tileHeight attributes specify the width and
                // height of each tile in the tileset.
                //
                // the contentPath value is the contentPath to the texture to
                // load that contains the tileset
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
                Texture2D texture = content.Load<Texture2D>(contentPath);

                // Create the texture region from the texture
                TextureRegion textureRegion = new TextureRegion(texture, x, y, width, height);

                // Create the tileset using the texture region
                Tileset tileset = new Tileset(textureRegion, tileWidth, tileHeight);

                // The <Tiles> element contains lines of strings where each line
                // represents a row in the tilemap.  Each line is a space
                // separated string where each element represents a column in that
                // row.  The value of the column is the id of the tile in the
                // tileset to draw for that location.
                //
                // Example:
                // <Tiles>
                //      00 01 01 02
                //      03 04 04 05
                //      03 04 04 05
                //      06 07 07 08
                // </Tiles>
                XElement tilesElement = root.Element("Tiles");

                // Split the value of the tiles data into rows by splitting on
                // the new line character
                string[] rows = tilesElement.Value.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);

                // Split the value of the first row to determine the total number of columns
                int columnCount = rows[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;

                // Create the tilemap
                Tilemap tilemap = new Tilemap(tileset, columnCount, rows.Length);

                // Process each row
                for (int row = 0; row < rows.Length; row++)
                {
                    // Split the row into individual columns
                    string[] columns = rows[row].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                    // Process each column of the current row
                    for (int column = 0; column < columnCount; column++)
                    {
                        // Get the tileset index for this location
                        int tilesetIndex = int.Parse(columns[column]);

                        // Add that region to the tilemap at the row and column location
                        tilemap.SetTile(column, row, tilesetIndex);
                    }
                }

                return tilemap;
            }
        }
    }
}