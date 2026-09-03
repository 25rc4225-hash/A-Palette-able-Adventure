namespace MonoGameLibrary.Graphics;

public class Tileset
{
    private readonly TextureRegion[] _tiles;

    /// Gets the width, in pixels, of each tile in this tileset.
    public int TileWidth { get; }

    /// Gets the height, in pixels, of each tile in this tileset.
    public int TileHeight { get; }

    /// Gets the total number of columns in this tileset.
    public int Columns { get; }

    /// Gets the total number of rows in this tileset.
    public int Rows { get; }

    /// Gets the total number of tiles in this tileset.
    public int Count { get; }

    /// Creates a new tileset based on the given texture region with the specified
    /// tile width and height.
    public Tileset(TextureRegion textureRegion, int tileWidth, int tileHeight)
    {
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        Columns = textureRegion.Width / tileWidth;
        Rows = textureRegion.Height / tileHeight;
        Count = Columns * Rows;

        // Create the texture regions that make up each individual tile
        _tiles = new TextureRegion[Count];

        for (int i = 0; i < Count; i++)
        {
            int x = i % Columns * tileWidth;
            int y = i / Columns * tileHeight;
            _tiles[i] = new TextureRegion(textureRegion.Texture, textureRegion.SourceRectangle.X + x, textureRegion.SourceRectangle.Y + y, tileWidth, tileHeight);
        }
    }

    /// Gets the texture region for the tile from this tileset at the given index.
    public TextureRegion GetTile(int index) => _tiles[index];

    /// Gets the texture region for the tile from this tileset at the given location.
    public TextureRegion GetTile(int column, int row)
    {
        int index = row * Columns + column;
        return GetTile(index);
    }
}