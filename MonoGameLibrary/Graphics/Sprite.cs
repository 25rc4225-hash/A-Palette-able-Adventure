using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class Sprite
{
    /// Gets or Sets the source texture region represented by this sprite.
    public TextureRegion Region { get; set; }

    /// Gets or Sets the color mask to apply when rendering this sprite.
    /// Default value is Color.White
    public Color Color { get; set; } = Color.White;

    /// Gets or Sets the amount of rotation, in radians, to apply when rendering this sprite.
    /// Default value is 0.0f
    public float Rotation { get; set; } = 0.0f;

    /// Gets or Sets the scale factor to apply to the x- and y-axes when rendering this sprite.
    /// Default value is Vector2.One
    public Vector2 Scale { get; set; } = Vector2.One;

    /// Gets or Sets the xy-coordinate origin point, relative to the top-left corner, of this sprite.
    /// Default value is Vector2.Zero
    public Vector2 Origin { get; set; } = Vector2.Zero;

    /// Gets or Sets the sprite effects to apply when rendering this sprite.
    /// Default value is SpriteEffects.None
    public SpriteEffects Effects { get; set; } = SpriteEffects.None;

    /// Gets or Sets the layer depth to apply when rendering this sprite.
    /// Default value is 0.0f
    public float LayerDepth { get; set; } = 0.0f;

    /// Gets the width, in pixels, of this sprite. 
    /// Width is calculated by multiplying the width of the source texture region by the x-axis scale factor.
    public float Width => Region.Width * Scale.X;

    /// Gets the height, in pixels, of this sprite.
    /// Height is calculated by multiplying the height of the source texture region by the y-axis scale factor.
    public float Height => Region.Height * Scale.Y;
    
    /// Creates a new sprite.
    public Sprite() { }

    /// Creates a new sprite using the specified source texture region.
    public Sprite(TextureRegion region)
    {
        Region = region;
    }
    
    /// Sets the origin of this sprite to the center.
    public void CenterOrigin()
    {
        Origin = new Vector2(Region.Width, Region.Height) * 0.5f;
    }

    /// Submit this sprite for drawing to the current batch.
    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        Region.Draw(spriteBatch, position, Color, Rotation, Origin, Scale, Effects, LayerDepth);
    }
}